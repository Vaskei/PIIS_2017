using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using e_Index.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using SEUS.Models;
// to stavimo da mozemo vidjeti classu koja nam je potrebna
using SEUS.EXCELKlase;
// ovo je za Upisane
using SEUS.NepolozeniiUpisani;
// ovo je da bismo generirali excel dokument
using Microsoft.AspNetCore.Hosting;
// sljedeci su za generiranje upisivanje i izgled dokumenta tj celija
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.FileProviders;
using Microsoft.Win32;

namespace SEUS.Controllers
{
    public class IspisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // sljedeca public metoda poziva iz ispis Ispis_nepolozenih_kolegija.. kada se klikne na
        // ispis pa ispis nepolozenih kolegija tada se pojavi stranica na kojoj cemo napraviti
        // odredeni sadrzaj tj. view za ispis nepolozenih..
        // isto to vrijedi i za ostale metode ispod.
        public IActionResult Ispis_nepolozenih_kolegija()
        {
            ViewData["Message"] = "Ispis podataka";

            return View();
        }

        public IActionResult Ispis_podataka_o_studentu()
        {
            ViewData["Message"] = "Ispis podataka";

            return View();
        }


        public IActionResult Ispis_upisanih_kolegija()
        {
            ViewData["Message"] = "Ispis podataka";

            return View();
        }

        // ovo je poziv metode za excel nepolozene kolegije
        public IActionResult Excel_Nepolozeni()
        {
            Excel_Nepolozeni_Model ex = new Excel_Nepolozeni_Model();
            ViewBag.listaNepolozenih = ex.FindAll();

            return View();
        }
        public IActionResult PreuzetoNepolozeni()
        {
            return View("Preuzeto");
        }

    
        public IActionResult Slanje_na_mail(string prima, string opis, string name)
        {
             /*var message = new MimeMessage();
             message.From.Add(new MailboxAddress("mailbot@digits.dk"));
             message.To.Add(new MailboxAddress("ingo@digits.dl"));
             message.Subject = name;
             message.Body = new TextPart("html")
             {
                 Text = "Od: " + name + " <br>" +
                 "Informacije o kontaktu: " + prima +"<br>" + 
                 "Poruka: " + opis
             };

             using (var client = new SmtpClient())
             {
                 client.Connect("smtp.gmail.com", 587);
                 client.Authenticate("mailbot@digits.dk", "kimschip");
                 client.Send(message);
                 client.Disconnect(false);
           
        } */
            ViewData["Message"] = "Slanje na mail";
            return View();
        }
        

        // *************************************
        // *************************************
        // ovo je za ExcelPodaciStudent
        // znaci od tuda do dole je sve za spajanje s bazom i dohvat podataka
        // *************************************
        // *************************************

        private readonly AppSettings _appSettings;
        private readonly ILogger log;
        private readonly IHostingEnvironment _hostingEnvironment;

        // promjeniti stp u excelcontroller
        // dodavanje IHostingEnvironment kako bismo mogli exportati excel dokument
        public IspisController(IOptions<AppSettings> appSettings, ILogger<IspisController> log, IHostingEnvironment hostingEnvironment)
        {
            _appSettings = appSettings.Value;
            _hostingEnvironment = hostingEnvironment;
            this.log = log;
        }

        // PODACI O STUDENTU
        // *************************************
        // *************************************
        // OVO JE ZA EXCEL PODATKE O STUDENTU
        // *************************************
        // *************************************
        [HttpGet]
        public async Task<ActionResult> ExcelPodaci(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Pregled podataka studenta pokrenuto!!");

            // url - model EXCELPodaciStudent.cs
            string URLzaPodatke = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
            // to su zapravo modeli koji su izgenerirani iz json.a te dohvacaju podatke

            // ovo je class u EXCELPodacioStudentu.. koji trenutno sadrzava odredene podatke koje mi trebamo npr. ime prezime.. get i set metode
            StudentPodaci Ispis = null;

            // provjera da li je upisan jbbag ili nije.. te vraca view 
            if (String.IsNullOrEmpty(jmbag))
            {
                return View("EXCELPodacioStudentu", Ispis);
            }

            string studentId = jmbag;

            // ova varijabla nam je za spajanje na server!
            var client = new HttpClient();
            // pomocu ovih podataka ispod u zagradi mozemo staviti na link koji nam je dat na dokumentaciji da se prijavimo.....
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // settigsi za rjesavanje problema sa null-om kod dohvata JSON-a
            var settingsZaJSON = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            try
            {
                // klasa u modelu ExcelPodacioStudentu.cs
                Ispis = new StudentPodaci();
                string test;

                // tu vidimo da koristimo onaj gore URL koji smo si zadali iz dokumentacije...
                // ukoliko trebamo ostale klase koristiti i ukoliko je URL drugacijistavimo gore i pozovemo novi using s tim URL-om
                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaPodatke, studentId)))
                {
                    // streamreader je za citanje podataka te koristimo UTF8 da moze citati hrvatske znakove...
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();
                    // var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                    // studentPodaci.studentJSON = test;
                    // SEUS . ispisanimodel je namespace od EXCELPodaciStudent pomocu kojeg cemo vuci podatke..
                    // ta tu varijable podaciOStudentu se zapravo fokusira na ROOTOBJECT kaj znaci najmanji po hierarhiji..
                    // zato ispod u if-u pisemo podaciOStudentu (ta varijabla)._embedded. i tak dalje kj trebamo...
                    SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test, settingsZaJSON);

                    // to je objasnjeno 3 linije iznad u komentaru
                    // Ispis.ime.. taj ispis je iz nase klase EXCELPodacioStudentu.. to je nasa klasa u koju smo savili get i set metode
                    // to nije klasa iz json-a
                    if (podaciOStudentu != null)
                    {
                        // ispis.ime je da se u objekt koji ima get i set metodu. Pomocu set metode upisemo potrebno tj. nakon =.
                        Ispis.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                        Ispis.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                        Ispis.email = podaciOStudentu._embedded.elektronickiIdentitet.email;
                        Ispis.jmbag = podaciOStudentu._embedded.osobniPodaci.jmbag;
                        Ispis.jmbg = podaciOStudentu._embedded.osobniPodaci.jmbg;
                        Ispis.oib = podaciOStudentu._embedded.osobniPodaci.oib;
                        Ispis.datumrodenja = podaciOStudentu._embedded.osobniPodaci.datumRodjenja;
                        Ispis.spol = podaciOStudentu._embedded.osobniPodaci.spol;
                        Ispis.godinazavrsetkaskole = podaciOStudentu._embedded.osobniPodaci.godinaZavrsetkaSkola;
                        Ispis.imemajke = podaciOStudentu._embedded.osobniPodaci.imeMajka;
                        Ispis.imeoca = podaciOStudentu._embedded.osobniPodaci.imeOtac;
                    }
                }
                // OVO JE ZA EXCEL EXPORT
                //***********************************************************
                string getDownloadFolderPath = "C:/Users/Public/Downloads";
                string sWebRootFolder = getDownloadFolderPath;

                // @"C:\test1.xlsx";
                // upisuje ime i prezime u naziv dulkumenta
                string sFileName = @"Podaci"+Ispis.ime+Ispis.prezime+".xlsx";
                // url bi trebali ukoliko bi pozivali kasnije.. u njega se bi spremalo sve potrebno
                //string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                try
                {
                    if (file.Exists)
                    {
                        file.Delete();
                        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                    }
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        // add a new worksheet to the empty workbook
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Podaci");
                        //ovo je da se posloze malo podaci...
                        worksheet.Column(1).Width = 25;
                        worksheet.Column(2).Width = 30;
                            
                        // ovo je za pocetno kod upisa u excel...
                        worksheet.Cells[1, 1].Value = "Podaci o " + Ispis.ime + Ispis.prezime;
                        worksheet.Cells[2, 1].Value = "Ime:";
                        worksheet.Cells[3, 1].Value = "Prezime:";
                        worksheet.Cells[4, 1].Value = "e-mail:";
                        worksheet.Cells[5, 1].Value = "jmbag";
                        worksheet.Cells[6, 1].Value = "jmbg:";
                        worksheet.Cells[7, 1].Value = "OIB:";
                        worksheet.Cells[8, 1].Value = "Spol:";
                        worksheet.Cells[9, 1].Value = "Godina zavr�etka �kole:";
                        worksheet.Cells[10, 1].Value = "Ime oca:";
                        worksheet.Cells[11, 1].Value = "Ime majke:";
                        // sljedecim naredbama se upisuju na mjesta b2...... sljedeci podaci nakon =
                        worksheet.Cells["B2"].Value = Ispis.ime;
                        worksheet.Cells["B3"].Value = Ispis.prezime;
                        worksheet.Cells["B4"].Value = Ispis.email;
                        worksheet.Cells["B5"].Value = Ispis.jmbag;
                        worksheet.Cells["B6"].Value = Ispis.jmbg;
                        worksheet.Cells["B7"].Value = Ispis.oib;
                        worksheet.Cells["B8"].Value = Ispis.spol;
                        worksheet.Cells["B9"].Value = Ispis.godinazavrsetkaskole;
                        worksheet.Cells["B10"].Value = Ispis.imeoca;
                        worksheet.Cells["B11"].Value = Ispis.imemajke;


                        // DEMO PRIMJER
                        //worksheet.Cells["A2"].Value = 1000;
                        //worksheet.Cells["B2"].Value = "uuuuu";
                        //worksheet.Cells["C2"].Value = "M";
                        //worksheet.Cells["D2"].Value = 5000;
                        //worksheet.Cells["A3"].Value = 1001;
                        //worksheet.Cells["B3"].Value = "uuuuu";
                        //worksheet.Cells["C3"].Value = "M";
                        //worksheet.Cells["D3"].Value = 10000;
                        //worksheet.Cells["A4"].Value = 1002;
                        //worksheet.Cells["B4"].Value = "lllll";
                        //worksheet.Cells["C4"].Value = "F";
                        //worksheet.Cells["D4"].Value = 5000;

                        // ovo je za izgled EXCEL dokumenta
                        // 1 je prvi pa 1 je ko A 11 je 11 red pa 2 je B... kordinate.. sve izmedu primjeni stype

                        //using (var cells = worksheet.Cells[1, 1, 11, 2])
                        //{
                        //cells.Style.Font.Bold = true;
                        //cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        // ovo je nemoguce jer style jos nije postavljen.. to bi trebli drugacije iskodirati
                        //cells.Style.Border.Top.Color.SetColor(Color.Red);


                        //}
                        package.Save(); // spremanje dokumenta.. bez toda package tj dokument se nece spremiti na racunalo
                        //package.SaveAs(new FileInfo(@"\Downloads"));

                    }
                    //***********************************************************
                    
                }
                catch (Exception ee)
                {
                    ViewBag.Error = ee.Message;
                }

                //try {
                //    downloadFile(sWebRootFolder);

                //    FileResult downloadFile(string filePath)
                //    {
                //        IFileProvider provider = new PhysicalFileProvider(filePath);
                //        IFileInfo fileInfo = provider.GetFileInfo(sFileName);
                //        var readStream = fileInfo.CreateReadStream();
                //        var mimeType = "application/vnd.ms-excel";
                //        return File(readStream, mimeType, sFileName);
                //    }
                //}
                //catch (Exception ee)
                //{
                //    ViewBag.Error = ee.Message;
                //}


            }
            // catch koji omogucuje vracanje potrebnog u slucaju greske
            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostojeci JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            // taj Ispis nije tu stavljen jer je folder u kojem se nalazi view pod tim imenom
            // to je variabla od nase klase u kojoj imamo get i set metode.. to samo prosljedujemo unutrado view-a
            return View("EXCELPodacioStudentu", Ispis);

        }
        // *************************************
        // *************************************
        // OVO JE ZA WORD PODATKE O STUDENTU
        // *************************************
        // *************************************
        [HttpGet]
        public async Task<ActionResult> WordPodaci(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Pregled podataka studenta pokrenuto");

            // url - model EXCELPodaciStudent.cs
            string URLzaPodatke = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
            // to su zapravo modeli koji su izgenerirani iz json.a te dohvacaju podatke

            // ovo je class u EXCELPodacioStudentu.. koji trenutno sadrzava odredene podatke koje mi trebamo npr. ime prezime.. get i set metode
            StudentPodaci Ispis = null;

            // provjera da li je upisan jbbag ili nije.. te vraca view 
            if (String.IsNullOrEmpty(jmbag))
            {
                return View("WordPodacioStudentu", Ispis);
            }

            string studentId = jmbag;

            // ova varijabla nam je za spajanje na server!
            var client = new HttpClient();
            // pomocu ovih podataka ispod u zagradi mozemo staviti na link koji nam je dat na dokumentaciji da se prijavimo.....
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // settigsi za rjesavanje problema sa null-om kod dohvata JSON-a
            var settingsZaJSON = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            try
            {
                // klasa u modelu ExcelPodacioStudentu.cs
                Ispis = new StudentPodaci();
                string test;

                // tu vidimo da koristimo onaj gore URL koji smo si zadali iz dokumentacije...
                // ukoliko trebamo ostale klase koristiti i ukoliko je URL drugacijistavimo gore i pozovemo novi using s tim URL-om
                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaPodatke, studentId)))
                {
                    // streamreader je za citanje podataka te koristimo UTF8 da moze citati hrvatske znakove...
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();
                    // var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                    // studentPodaci.studentJSON = test;
                    // SEUS . ispisanimodel je namespace od EXCELPodaciStudent pomocu kojeg cemo vuci podatke..
                    // ta tu varijable podaciOStudentu se zapravo fokusira na ROOTOBJECT kaj znaci najmanji po hierarhiji..
                    // zato ispod u if-u pisemo podaciOStudentu (ta varijabla)._embedded. i tak dalje kj trebamo...
                    SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test, settingsZaJSON);

                    // to je objasnjeno 3 linije iznad u komentaru
                    // Ispis.ime.. taj ispis je iz nase klase EXCELPodacioStudentu.. to je nasa klasa u koju smo savili get i set metode
                    // to nije klasa iz json-a
                    if (podaciOStudentu != null)
                    {
                        Ispis.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                        Ispis.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                        Ispis.email = podaciOStudentu._embedded.elektronickiIdentitet.email;
                        Ispis.jmbag = podaciOStudentu._embedded.osobniPodaci.jmbag;
                        Ispis.jmbg = podaciOStudentu._embedded.osobniPodaci.jmbg;
                        Ispis.oib = podaciOStudentu._embedded.osobniPodaci.oib;
                        Ispis.datumrodenja = podaciOStudentu._embedded.osobniPodaci.datumRodjenja;
                        Ispis.spol = podaciOStudentu._embedded.osobniPodaci.spol;
                        Ispis.godinazavrsetkaskole = podaciOStudentu._embedded.osobniPodaci.godinaZavrsetkaSkola;
                        Ispis.imemajke = podaciOStudentu._embedded.osobniPodaci.imeMajka;
                        Ispis.imeoca = podaciOStudentu._embedded.osobniPodaci.imeOtac;
                    }
                }
                // TRANSFORM TO EXPORT WORD
                //***********************************************************
                string getDownloadFolderPath()
                {
                    return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                }
                string sWebRootFolder = getDownloadFolderPath();

                // @"C:\test1.xlsx";
                // upisuje ime i prezime u naziv dulkumenta
                string sFileName = @"Podaci" + Ispis.ime + Ispis.prezime + ".docx";
                // url bi trebali ukoliko bi pozivali kasnije.. u njega se bi spremalo sve potrebno
                //string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                //try
                //{
                //    if (file.Exists)
                //    {
                //        file.Delete();
                //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                //    }
                //    using (ExcelPackage package = new ExcelPackage(file))
                //    {
                //        // add a new worksheet to the empty workbook
                //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Podaci");
                //        //ovo je da se posloze malo podaci...
                //        worksheet.Column(1).Width = 25;
                //        worksheet.Column(2).Width = 30;

                //        worksheet.Cells[1, 1].Value = "Podaci o " + Ispis.ime + Ispis.prezime;
                //        worksheet.Cells[2, 1].Value = "Ime:";
                //        worksheet.Cells[3, 1].Value = "Prezime:";
                //        worksheet.Cells[4, 1].Value = "e-mail:";
                //        worksheet.Cells[5, 1].Value = "jmbag";
                //        worksheet.Cells[6, 1].Value = "jmbg:";
                //        worksheet.Cells[7, 1].Value = "OIB:";
                //        worksheet.Cells[8, 1].Value = "Spol:";
                //        worksheet.Cells[9, 1].Value = "Godina zavr�etka �kole:";
                //        worksheet.Cells[10, 1].Value = "Ime oca:";
                //        worksheet.Cells[11, 1].Value = "Ime majke:";

                //        worksheet.Cells["B2"].Value = Ispis.ime;
                //        worksheet.Cells["B3"].Value = Ispis.prezime;
                //        worksheet.Cells["B4"].Value = Ispis.email;
                //        worksheet.Cells["B5"].Value = Ispis.jmbag;
                //        worksheet.Cells["B6"].Value = Ispis.jmbg;
                //        worksheet.Cells["B7"].Value = Ispis.oib;
                //        worksheet.Cells["B8"].Value = Ispis.spol;
                //        worksheet.Cells["B9"].Value = Ispis.godinazavrsetkaskole;
                //        worksheet.Cells["B10"].Value = Ispis.imeoca;
                //        worksheet.Cells["B11"].Value = Ispis.imemajke;


                //        // DEMO PRIMJER
                //        //worksheet.Cells["A2"].Value = 1000;
                //        //worksheet.Cells["B2"].Value = "uuuuu";
                //        //worksheet.Cells["C2"].Value = "M";
                //        //worksheet.Cells["D2"].Value = 5000;
                //        //worksheet.Cells["A3"].Value = 1001;
                //        //worksheet.Cells["B3"].Value = "uuuuu";
                //        //worksheet.Cells["C3"].Value = "M";
                //        //worksheet.Cells["D3"].Value = 10000;
                //        //worksheet.Cells["A4"].Value = 1002;
                //        //worksheet.Cells["B4"].Value = "lllll";
                //        //worksheet.Cells["C4"].Value = "F";
                //        //worksheet.Cells["D4"].Value = 5000;

                //        // ovo je za izgled EXCEL dokumenta
                //        // 1 je prvi pa 1 je ko A 11 je 11 red pa 2 je B... kordinate.. sve izmedu primjeni stype

                //        //using (var cells = worksheet.Cells[1, 1, 11, 2])
                //        //{
                //        //cells.Style.Font.Bold = true;
                //        //cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //        //cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                //        // ovo je nemoguce jer style jos nije postavljen.. to bi trebli drugacije iskodirati
                //        //cells.Style.Border.Top.Color.SetColor(Color.Red);


                //        //}
                //        package.Save(); // spremanje dokumenta

                //        //package.SaveAs(new FileInfo(@"C:\\Desktop"));
                //    }
                //    //***********************************************************

                //}
                //catch (Exception ee)
                //{
                //    ViewBag.Error = ee.Message;
                //}

            }
            // catch koji omogucuje vracanje potrebnog u slucaju greske
            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostojeci JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            // taj Ispis nije tu stavljen jer je folder u kojem se nalazi view pod tim imenom
            // to je variabla od nase klase u kojoj imamo get i set metode.. to samo prosljedujemo unutrado view-a
            return View("WordPodacioStudentu", Ispis);

        }
        // *************************************
        // *************************************
        // OVO JE ZA PDF PODATKE O STUDENTU
        // *************************************
        // *************************************

        [HttpGet]
        public async Task<ActionResult> PdfPodaci(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Pregled podataka studenta pokrenuto");

            // url - model EXCELPodaciStudent.cs
            string URLzaPodatke = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
            // to su zapravo modeli koji su izgenerirani iz json.a te dohvacaju podatke

            // ovo je class u EXCELPodacioStudentu.. koji trenutno sadrzava odredene podatke koje mi trebamo npr. ime prezime.. get i set metode
            StudentPodaci Ispis = null;

            // provjera da li je upisan jbbag ili nije.. te vraca view 
            if (String.IsNullOrEmpty(jmbag))
            {
                return View("PdfPodacioStudentu", Ispis);
            }

            string studentId = jmbag;

            // ova varijabla nam je za spajanje na server!
            var client = new HttpClient();
            // pomocu ovih podataka ispod u zagradi mozemo staviti na link koji nam je dat na dokumentaciji da se prijavimo.....
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // settigsi za rjesavanje problema sa null-om kod dohvata JSON-a
            var settingsZaJSON = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            try
            {
                // klasa u modelu ExcelPodacioStudentu.cs
                Ispis = new StudentPodaci();
                string test;

                // tu vidimo da koristimo onaj gore URL koji smo si zadali iz dokumentacije...
                // ukoliko trebamo ostale klase koristiti i ukoliko je URL drugacijistavimo gore i pozovemo novi using s tim URL-om
                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaPodatke, studentId)))
                {
                    // streamreader je za citanje podataka te koristimo UTF8 da moze citati hrvatske znakove...
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();
                    // var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                    // studentPodaci.studentJSON = test;
                    // SEUS . ispisanimodel je namespace od EXCELPodaciStudent pomocu kojeg cemo vuci podatke..
                    // ta tu varijable podaciOStudentu se zapravo fokusira na ROOTOBJECT kaj znaci najmanji po hierarhiji..
                    // zato ispod u if-u pisemo podaciOStudentu (ta varijabla)._embedded. i tak dalje kj trebamo...
                    SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test, settingsZaJSON);

                    // to je objasnjeno 3 linije iznad u komentaru
                    // Ispis.ime.. taj ispis je iz nase klase EXCELPodacioStudentu.. to je nasa klasa u koju smo savili get i set metode
                    // to nije klasa iz json-a
                    if (podaciOStudentu != null)
                    {
                        Ispis.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                        Ispis.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                        Ispis.email = podaciOStudentu._embedded.elektronickiIdentitet.email;
                        Ispis.jmbag = podaciOStudentu._embedded.osobniPodaci.jmbag;
                        Ispis.jmbg = podaciOStudentu._embedded.osobniPodaci.jmbg;
                        Ispis.oib = podaciOStudentu._embedded.osobniPodaci.oib;
                        Ispis.datumrodenja = podaciOStudentu._embedded.osobniPodaci.datumRodjenja;
                        Ispis.spol = podaciOStudentu._embedded.osobniPodaci.spol;
                        Ispis.godinazavrsetkaskole = podaciOStudentu._embedded.osobniPodaci.godinaZavrsetkaSkola;
                        Ispis.imemajke = podaciOStudentu._embedded.osobniPodaci.imeMajka;
                        Ispis.imeoca = podaciOStudentu._embedded.osobniPodaci.imeOtac;
                    }
                }
                // TRANSFORM TO EXPORT WORD
                //***********************************************************
                string getDownloadFolderPath()
                {
                    return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                }
                string sWebRootFolder = getDownloadFolderPath();
                // @"C:\test1.xlsx";
                // upisuje ime i prezime u naziv dulkumenta
                string sFileName = @"Podaci" + Ispis.ime + Ispis.prezime + ".docx";
                // url bi trebali ukoliko bi pozivali kasnije.. u njega se bi spremalo sve potrebno
                //string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

            }
            // catch koji omogucuje vracanje potrebnog u slucaju greske
            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostojeci JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            // taj Ispis nije tu stavljen jer je folder u kojem se nalazi view pod tim imenom
            // to je variabla od nase klase u kojoj imamo get i set metode.. to samo prosljedujemo unutrado view-a
            return View("PdfPodacioStudentu", Ispis);

        }



        //UPISANI KOLEGIJI
        //UPISANI KOLEGIJI
        // *************************************
        // *************************************
        // to je Action Result koji se poziva na pritisak Excel gumba za Upisane kolegije
        // *************************************
        // *************************************
        [HttpGet]
        public async Task<ActionResult> ExcelUpisani(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Ispis upisanih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("EXCELUpisaniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {
                        //studentInfo.Naziv = studomat._embedded.student.prezime + " " + studomat._embedded.student.ime;

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var kolegij in studij._embedded.polozeniPredmeti)
                            {
                                //Provjera semestra
                                tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                if (tempSemestar == null)
                                {
                                    tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                    studijInfo.Semestri.Add(tempSemestar);
                                }

                                //Dodavanje kolegija u semestar
                                tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                if (kolegij._embedded.ispit != null)
                                {
                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                    tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                }


                                tempSemestar.Kolegiji.Add(tempKolegij);
                                if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                {
                                    tempKolegij.Polozen = true;
                                }
                            }

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {
                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    //Dodavanje kolegija u semestar
                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);


                            // OVO JE ZA EXCEL EXPORT UPISANIH KOLEGIJA
                            //***********************************************************
                            //***********************************************************
                            // ovo je da izvucem ime koje je potrebno za prikaz kod imena dokumenta...
                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
                            // napraviti novi objekt tipa student podaci 
                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                // streamreader je za citanje podataka te koristimo UTF8 da moze citati hrvatske znakove...
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                // var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                                // studentPodaci.studentJSON = test;
                                // SEUS . ispisanimodel je namespace od EXCELPodaciStudent pomocu kojeg cemo vuci podatke..
                                // ta tu varijable podaciOStudentu se zapravo fokusira na ROOTOBJECT kaj znaci najmanji po hierarhiji..
                                // zato ispod u if-u pisemo podaciOStudentu (ta varijabla)._embedded. i tak dalje kj trebamo...
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);

                                // to je objasnjeno 3 linije iznad u komentaru
                                // Ispis.ime.. taj ispis je iz nase klase EXCELPodacioStudentu.. to je nasa klasa u koju smo savili get i set metode
                                // to nije klasa iz json-a
                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            // webroot je referenca da sprema u wwwroot folder unutar projekta
                            string getDownloadFolderPath = "C:/Users/Public/Downloads";

                            string sWebRootFolder = getDownloadFolderPath;

                            // sljedece je za naziv dokumanta... odma se upise ime i prezime tako da se moze download-ati vise 
                            // dokumenata i da se zna ciji su podaci...
                            string sFileName = @"UpisaniKolegiji"+ime.ime+ime.prezime+".xlsx";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                                using (ExcelPackage package = new ExcelPackage(file))
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("UpisaniKolegiji");
                                    //postavljanje za postavljanje sirine �elija potrebnih da se podaci lijepo posloze jer
                                    // inace su podaci unutar celija pa se ne vide dobro!
                                    worksheet.Column(1).Width = 42;
                                    worksheet.Column(2).Width = 20;
                                    worksheet.Column(3).Width = 25;
                                    worksheet.Column(4).Width = 24;
                                    // ova varijabla je za pomoc kod upisivanja u excel-file tj oznacava u kojem redku se nalazi program trenutno..
                                    int temp1 = 2;

                                    foreach (var studijj in studomat._embedded.upisaniElementiStruktureStudija)
                                    {
                                        // ovo je pocetno izgled i namjenjeno ispisu u excel dokument.
                                        worksheet.Cells[1, 1].Value = "Studij: " + studijj.naziv ;
                                        worksheet.Cells[2, 1].Value = "Kolegij ";
                                        worksheet.Cells[2, 2].Value = "Predavac ";
                                        worksheet.Cells[2, 3].Value = "Polozeno (DA/NE)";
                                        worksheet.Cells[2, 4].Value = "Ocjena";

                                        // ovo je foreach za predmete, tj za polozene predmete 
                                        foreach (var polozeni in studijj._embedded.polozeniPredmeti)
                                        {
                                            // uvecava se za 1 jer pisemo onda u cells a+1.. i onda ide svaki put u drugi red....
                                            temp1++;
                                            // ovo upisuje naziv od predmeta
                                            worksheet.Cells["A"+temp1].Value = polozeni._embedded.predmet.naziv;
                                            // ova privremena varijabla je stvaranje klase kolegij da ne pisemo punu putanju.
                                            // Tada je tempkolegij vec me�iran na naziv,jer pisemo polozeni-_embedded.predmet.naziv
                                            tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                            tempKolegij.Naziv = polozeni._embedded.predmet.naziv;

                                            if (polozeni._embedded.ispit != null)
                                            {
                                                // provjera ukoliko nema imena i prezimena tada pise NEPOZNATO
                                                // probao sam i null i false i sve.. NE RADI!...
                                                // provjeravamo da ukoliko cjenjivac nema ime ni prezime da se upise "NEPOZNATO" da nije prazna �elija
                                                if (polozeni._embedded.ispit._embedded.ocjenjivac.ime != null && polozeni._embedded.ispit._embedded.ocjenjivac.prezime != null)
                                                {
                                                    // ukoliko ima ime i prezime tada se upisuju u celiju u excel na sljedece mjesto B + temp1(koji se uveca kod ulaska u foreach petlju za 1)
                                                    worksheet.Cells["B" + temp1].Value = polozeni._embedded.ispit._embedded.ocjenjivac.prezime + " " + polozeni._embedded.ispit._embedded.ocjenjivac.ime;
                                                }

                                            }
                                            else
                                            {
                                                // ispisuje se kada ocjenjivac nema ime ni prezime tako da ne bude prazno mjesto
                                                worksheet.Cells["B" + temp1].Value = "NEPOZNATO";
                                            }
                                            // ovo je za polozene i nepolozene ali ne radi dobro :(
                                            //worksheet.Cells["C"+ temp1].Value = polozeni._embedded.predmet._embedded.status;

                                            // ovo provjerava da li je kolegij polozen.. ako je pise da ako ne onda ne :D
                                            // RADI
                                            if (tempKolegij.Polozen == false)
                                            {
                                                worksheet.Cells["C" + temp1].Value = "DA";
                                            }
                                            else
                                            {
                                                worksheet.Cells["C" + temp1].Value = "NE";
                                            }
                                            // ovaj try je da se ocjene mogu ispisati u excel ...
                                            try
                                            {
                                                var www = JObject.FromObject(polozeni);
                                                string w2 = www.ToString();

                                                e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);
                                                tempKolegij = new SEUS.EXCELKlase.Kolegij();

                                                tempKolegij.Naziv = kolegij._embedded.predmet.naziv;

                                                if (kolegij._embedded.ispit != null)
                                                {
                                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                                    // u D kolonu upisuje 
                                                    worksheet.Cells["D" + temp1].Value = kolegij._embedded.ispit.ocjena;

                                                }
                                            }

                                            catch (Exception e)
                                            {
                                                ViewBag.Error = e.Message;
                                            }
                                            // ovo je za ocjenu
                                            //worksheet.Cells["D" + temp1].Value = tempKolegij.Ocjena;
                                            //worksheet.Cells["D" + temp1].Value = polozeni._embedded.ispit.ocjena;

                                        }

                                        // ovo je za NEPOLOZENE predmete ali ne radi!!
                                        foreach (var nepolozeni in studijj._embedded.nepolozeniPredmeti)
                                        {
                                            try
                                            {
                                            }
                                            catch (Exception e)
                                            {
                                                ViewBag.Error = e.Message;
                                            }
                                        }

                                        // ovo je za izgled tabele!
                                        using (var cells = worksheet.Cells[2, 1, 2, 4])
                                        {
                                            cells.Style.Font.Bold = true;
                                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                        }

                                    }

                                    package.Save();
                                }
                            }
                            catch(Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("EXCELUpisaniKolegiji", studentInfo);
        }
        // *************************************
        // *************************************
        // OVO JE ZA WORD UPISANE KOLEGIJE
        // *************************************
        // *************************************
        [HttpGet]
        public async Task<ActionResult> WordUpisani(string jmbag)
        {
            log.LogWarning("Ispis upisanih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("WordUpisaniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {
                      
                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var kolegij in studij._embedded.polozeniPredmeti)
                            {
                                tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                if (tempSemestar == null)
                                {
                                    tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                    studijInfo.Semestri.Add(tempSemestar);
                                }

                                tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                if (kolegij._embedded.ispit != null)
                                {
                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                    tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                }


                                tempSemestar.Kolegiji.Add(tempKolegij);
                                if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                {
                                    tempKolegij.Polozen = true;
                                }
                            }

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {
                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);

                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
                            // napraviti novi objekt tipa student podaci 
                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);
                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            // webroot je referenca da sprema u wwwroot folder unutar projekta
                            string getDownloadFolderPath = "C:/Users/Public/Downloads";
                            string sWebRootFolder = getDownloadFolderPath;
                            string sFileName = @"UpisaniKolegiji.doc";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                            }
                            catch (Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("WordUpisaniKolegiji", studentInfo);
        }

        // *************************************
        // *************************************
        // OVO JE ZA Pdf UPISANE KOLEGIJE
        // *************************************
        // *************************************
        [HttpGet]
        public async Task<ActionResult> PdfUpisani(string jmbag)
        {
            log.LogWarning("Ispis upisanih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("PdfUpisaniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var kolegij in studij._embedded.polozeniPredmeti)
                            {
                                tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                if (tempSemestar == null)
                                {
                                    tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                    studijInfo.Semestri.Add(tempSemestar);
                                }

                                tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                if (kolegij._embedded.ispit != null)
                                {
                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                    tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                }


                                tempSemestar.Kolegiji.Add(tempKolegij);
                                if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                {
                                    tempKolegij.Polozen = true;
                                }
                            }

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {
                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);

                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";
                            // napraviti novi objekt tipa student podaci 
                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);
                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            // webroot je referenca da sprema u wwwroot folder unutar projekta
                            string getDownloadFolderPath()
                            {
                                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                            }
                            string sWebRootFolder = getDownloadFolderPath();
                            string sFileName = @"UpisaniKolegiji.pdf";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                            }
                            catch (Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("PdfUpisaniKolegiji", studentInfo);

        }

        // OVO JE ZA NEPOLOZENE
        // *************************************
        // *************************************
        // ovo je za Excel nepolozene kolegije..
        // *************************************
        // *************************************

        [HttpGet]
        public async Task<ActionResult> ExcelNepolozeni(string jmbag)
        {
            log.LogWarning("Ispis Nepolozenih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("EXCELNepolozeniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {
                                    
                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);

                            // OVO JE ZA EXCEL EXPORT NEPOLOZENIH KOLEGIJA
                            //***********************************************************
                            //***********************************************************
                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";

                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);

                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            string getDownloadFolderPath = "C:/Users/Public/Downloads";
                            //string getDownloadFolderPath()
                            //{
                            //    return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                            //}
                            string sWebRootFolder = getDownloadFolderPath;


                            string sFileName = @"NepolozeniKolegiji"+ime.ime+ime.prezime+".xlsx";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                                using (ExcelPackage package = new ExcelPackage(file))
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("NepolozeniKolegiji");
                                    //postavljanje za postavljanje sirine �elija potrebnih da se podaci lijepo posloze jer
                                    // inace su podaci unutar celija pa se ne vide dobro!
                                    worksheet.Column(1).Width = 42;
                                    worksheet.Column(2).Width = 20;
                                    worksheet.Column(3).Width = 25;
                                    worksheet.Column(4).Width = 24;

                                    int temp2 = 2;

                                    foreach (var studijjj in studomat._embedded.upisaniElementiStruktureStudija)
                                    {

                                        worksheet.Cells[1, 1].Value = "Studij: " + studijjj.naziv;
                                        worksheet.Cells[2, 1].Value = "Kolegij ";
                                        worksheet.Cells[2, 2].Value = "Predavac ";

                                        // ovo je foreach za predmete, tj za polozene predmete 
                                        foreach (var nepolozeni in studijjj._embedded.nepolozeniPredmeti)
                                        {
                                            temp2++;

                                            worksheet.Cells["A" + temp2].Value = nepolozeni.ToString();

                                            tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                            tempKolegij.Naziv = nepolozeni.ToString();

                  

                                        }
                                        
                                        // ovo je za izgled tabele!
                                        using (var cells = worksheet.Cells[2, 1, 2, 2])
                                        {
                                            cells.Style.Font.Bold = true;
                                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                        }

                                    }

                                    package.Save();
                                }
                            }
                            catch (Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("EXCELNepolozeniKolegiji", studentInfo);
        }
        // *************************************
        // *************************************
        // OVO JE ZA WORD NAPOLOZENE KOLEGIJE
        // *************************************
        // *************************************

        [HttpGet]
        public async Task<ActionResult> WordNepolozeni(string jmbag)
        {
            log.LogWarning("Ispis Nepolozenih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("WordNepolozeniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {

                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);

                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";

                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);

                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            string getDownloadFolderPath()
                            {
                                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                            }
                            string sWebRootFolder = getDownloadFolderPath();

                            string sFileName = @"NepolozeniKolegiji.doc";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                            }
                            catch (Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("WordNepolozeniKolegiji", studentInfo);
        }

        // *************************************
        // *************************************
        // OVO JE ZA PDF NAPOLOZENE KOLEGIJE
        // *************************************
        // *************************************

        [HttpGet]
        public async Task<ActionResult> PdfNepolozeni(string jmbag)
        {
            log.LogWarning("Ispis Nepolozenih kolegija pokrenuto!");

            string URLzaupisane = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            SEUS.EXCELKlase.StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("PdfNepolozeniKolegiji", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new SEUS.EXCELKlase.StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URLzaupisane, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    SEUS.EXCELKlase.Semestar tempSemestar;
                    SEUS.EXCELKlase.Kolegij tempKolegij;

                    if (studomat != null)
                    {

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            SEUS.EXCELKlase.Studij studijInfo = new SEUS.EXCELKlase.Studij() { Naziv = studij.naziv };

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {

                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new SEUS.EXCELKlase.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    tempKolegij = new SEUS.EXCELKlase.Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch (Exception ee)
                                {
                                    ViewBag.Error = ee.Message;
                                }
                            }
                            studentInfo.Studiji.Add(studijInfo);

                            string zaime = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";

                            StudentPodaci ime = new StudentPodaci();

                            using (var task = await client.GetStreamAsync(String.Format(zaime, studentId)))
                            {
                                StreamReader citanje = new StreamReader(task, Encoding.UTF8);
                                test = citanje.ReadToEnd();
                                SEUS.IspisaniModel.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.IspisaniModel.RootObject>(test);

                                if (podaciOStudentu != null)
                                {
                                    ime.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                                    ime.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                                }
                            }
                            string getDownloadFolderPath()
                            {
                                return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
                            }
                            string sWebRootFolder = getDownloadFolderPath();

                            string sFileName = @"NepolozeniKolegiji.pdf";
                            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

                            try
                            {
                                if (file.Exists)
                                {
                                    file.Delete();
                                    file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                                }
                            }
                            catch (Exception ee)
                            {
                                ViewBag.Error = ee.Message;
                            }
                        }
                    }
                }
            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoje�i JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("PdfNepolozeniKolegiji", studentInfo);
        }


    }

}




