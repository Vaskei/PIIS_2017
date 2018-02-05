using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using e_Index.Misc;
using SEUS.Podatkomat;
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

namespace SEUS.Controllers
{
    public class StudentPodaciController : Controller

    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public StudentPodaciController(IOptions<AppSettings> appSettings, ILogger<StudentPodaciController> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }

        [HttpGet]
        public async Task<ActionResult> Podaci(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Pregled podataka studenta pokrenuto");

            // url - model StudentPodaciSummary.cs
            string URL = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";

            // url - model StudentovStudijSummary.cs
            string URL2 = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/studentovstudij";

            // url - model StudentPrivatnoSummary.cs
            string URL3 = "https://www.isvu.hr/apiproba/vu/313/upisgodine/akademskagodina/{0}/student/jmbag/{1}";

            // url - model StudentovPrelazakSummary.cs
            string URL4 = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/prelazak";

            Student studentPodaci = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("Podaci", studentPodaci);
            }

            string studentId = jmbag;
            int godinaUpisa = 2016;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            //settigsi za rjesavanje problema sa null-om kod dohvata JSON-a
            var settingsZaJSON = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };


            try
            {
                // klasa u modelu StudentPodaciModel.cs
                studentPodaci = new Student();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URL, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();
                    //var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                    //studentPodaci.studentJSON = test;

                    SEUS.PodatkomatKlase.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.PodatkomatKlase.RootObject>(test,settingsZaJSON);

                    if (podaciOStudentu != null)
                    {
                        studentPodaci.ime = podaciOStudentu._embedded.osobniPodaci.ime;
                        studentPodaci.prezime = podaciOStudentu._embedded.osobniPodaci.prezime;
                        studentPodaci.jmbag = podaciOStudentu._embedded.osobniPodaci.jmbag;

                        studentPodaci.nazivStudija = podaciOStudentu._embedded.osobniPodaci._embedded.maticnoVisokoUciliste.naziv;
                        studentPodaci.datumUpisaStudija = podaciOStudentu._embedded.studentNaVisokomUcilistu.datumUpis;
                        studentPodaci.jmbg = podaciOStudentu._embedded.osobniPodaci.jmbg;
                        studentPodaci.oib = podaciOStudentu._embedded.osobniPodaci.oib;
                        studentPodaci.datumRodenja = podaciOStudentu._embedded.osobniPodaci.datumRodjenja;

                        //dohvacanje linka slike -> spremanje slike u byte array -> pretvaranje byte array-a u base64string  - bravo GF :D   
                        string URLslika = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/slika";
                        var responseSlika = await client.GetAsync(String.Format(URLslika, studentId));
                        if (responseSlika.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            studentPodaci.slikaLink = podaciOStudentu._links.student_student_slika.href;
                            byte[] imageArray = await client.GetByteArrayAsync(studentPodaci.slikaLink);
                            studentPodaci.slikaBase64 = Convert.ToBase64String(imageArray);
                        }
                        else
                        {
                            studentPodaci.slikaBase64 = null;   //nema linka, nema slike
                        }

                        // otac i majka
                        studentPodaci.imeOca = podaciOStudentu._embedded.osobniPodaci.imeOtac;
                        studentPodaci.imeMajke = podaciOStudentu._embedded.osobniPodaci.imeMajka;

                        //podaci o ustanovi
                        studentPodaci.nazivUstanove = podaciOStudentu._embedded.studentNaVisokomUcilistu._embedded.studiraVisokoUciliste.naziv;
                        studentPodaci.sifraUstanove = podaciOStudentu._embedded.studentNaVisokomUcilistu._embedded.studiraVisokoUciliste.sifra.ToString();
                        studentPodaci.emailStudenta = podaciOStudentu._embedded.elektronickiIdentitet.email;
                        studentPodaci.emailAktivan = podaciOStudentu._embedded.studentNaVisokomUcilistu.emailAktivan;

                        //podaci u zavrsenoj srednjoj skoli

                        if (podaciOStudentu._embedded.osobniPodaci._embedded.zavrsenaSkola != null)
                        {
                            studentPodaci.srednjaSkola = podaciOStudentu._embedded.osobniPodaci._embedded.zavrsenaSkola.naziv;
                        }
                        else studentPodaci.srednjaSkola = "Nema podataka o srednjoj školi!";

                        if (podaciOStudentu._embedded.osobniPodaci.godinaZavrsetkaSkola != null)
                        { studentPodaci.godinaZavrsetkaSrednje = podaciOStudentu._embedded.osobniPodaci.godinaZavrsetkaSkola; }
                        else studentPodaci.godinaZavrsetkaSrednje = "Nema podatka o završetku škole!";
                        
                        studentPodaci.strukovnoPodrucjeSrednje = (podaciOStudentu._embedded.osobniPodaci._embedded.strukovnoPodrucje != null) ? podaciOStudentu._embedded.osobniPodaci._embedded.strukovnoPodrucje.naziv : "Nema podatka";
                        studentPodaci.smjerSrednjeSkole = (podaciOStudentu._embedded.osobniPodaci._embedded.programIzobrazbe != null) ? podaciOStudentu._embedded.osobniPodaci._embedded.programIzobrazbe.naziv : "Nema podatka.";

                        //Testni podaci
                        //studentPodaci.podaciURL = podaciOStudentu._links.self.href;
                    }
                }

                using (var streamTask = await client.GetStreamAsync(String.Format(URL2, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    SEUS.PodatkomatStudijKlase.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.PodatkomatStudijKlase.RootObject>(test);

                    if (podaciOStudentu != null)
                    {
                        //studentPodaci.studijURL = podaciOStudentu._links.self.href; //Testni podaci

                        godinaUpisa = podaciOStudentu._embedded.studentoviStudiji[0]._embedded.upisNaRazinuStudija.akademskaGodinaUpisa;
                        studentPodaci.nazivPodrucja = podaciOStudentu._embedded.studentoviStudiji[0]._embedded.upisaniElementStruktureStudija.naziv;

                        //iteracija 5
                        foreach (var studij in podaciOStudentu._embedded.studentoviStudiji)
                        {
                            Podatkomat.Studij tempStudij = new Podatkomat.Studij();

                            if (studij != null)
                            {
                                tempStudij.nazivJednogOdStudija = studij._embedded.upisaniElementStruktureStudija.naziv;
                                tempStudij.tipIndexa = studij._embedded.indeks._embedded.tipIndeksa.oznaka;
                                tempStudij.doKadTrajuStudPrava = studij._embedded.studentskaPrava.datumDoKojegTrajuStudentskaPrava;
                                tempStudij.prosjekStudenta = studij.prosjek.ToString();
                                tempStudij.tezinskiProsjekStudenta = studij.tezinskiProsjek.ToString();
                                tempStudij.razinaINazivStudija = studij._embedded.upisNaRazinuStudija._embedded.razinaStudija.naziv;
                                tempStudij.redniBrojStudija = studij._embedded.upisNaRazinuStudija.redniBrojStudija.ToString();

                                studentPodaci.Studiji.Add(tempStudij);
                            }
                            else
                            {
                                tempStudij.nazivJednogOdStudija = "Nema podatka";
                                tempStudij.tipIndexa = "Nema podatka";
                                tempStudij.doKadTrajuStudPrava = "Nema podatka";
                                tempStudij.prosjekStudenta = "Nema podatka";
                                tempStudij.tezinskiProsjekStudenta = "Nema podatka";
                                tempStudij.razinaINazivStudija = "Nema podatka";
                                tempStudij.redniBrojStudija = "Nema podatka";

                                studentPodaci.Studiji.Add(tempStudij);
                            }

                            //godinaUpisa = studij._embedded.upisNaRazinuStudija.akademskaGodinaUpisa;
                            //studentPodaci.nazivPodrucja = studij._embedded.upisaniElementStruktureStudija.naziv;

                            //Testni podaci
                            //studentPodaci.godinaUpisa = godinaUpisa;
                        }
                    }
                }

                using (var streamTask = await client.GetStreamAsync(String.Format(URL3, godinaUpisa, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    SEUS.PodatkomatPrivatnoKlase.RootObject podaciOStudentu = JsonConvert.DeserializeObject<SEUS.PodatkomatPrivatnoKlase.RootObject>(test,settingsZaJSON);

                    Roditelj tempRoditelj;

                    if (podaciOStudentu != null)
                    {
                        studentPodaci.adresaStudenta = podaciOStudentu._embedded.prebivaliste.ulicaIBroj;
                        studentPodaci.mjesto = podaciOStudentu._embedded.prebivaliste._embedded.mjesto.naziv;
                        studentPodaci.postanskiBrStudenta = podaciOStudentu._embedded.prebivaliste._embedded.mjesto.postanskaOznaka;
                        studentPodaci.drzava = podaciOStudentu._embedded.prebivaliste._embedded.drzava.naziv;
                        studentPodaci.uzdrzavatelj = (podaciOStudentu._embedded.uzdrzavatelj != null) ? podaciOStudentu._embedded.uzdrzavatelj._embedded.nacinUzdrzavanja.opis : "Nema podatka.";

                        foreach (var roditelj in podaciOStudentu._embedded.roditelji ?? new List<PodatkomatPrivatnoKlase.Roditelji>())
                        {
                                //JToken tataILImama = roditelj.oznaka;
                                //JToken polozajUZanimanju = roditelj._embedded.polozajUZanimanju.opis;
                                //JToken zanimanje = roditelj._embedded.zanimanje.opis;
                                //JToken postignutoObrazovanje = roditelj._embedded.postignutoObrazovanje.naziv;
                                //JToken strucnaSprema = roditelj._embedded.strucnaSprema.naziv;

                            //Dodavanje roditelja k studentu
                            tempRoditelj = new Roditelj() { otacILImajka =" ", zanimanje=" ", strucnaSprema= " ", polozajUZanimanju =" ", postignutoObrazovanje =" "};

                            if (roditelj != null)
                            {

                                //if (tataILImama.Type != JTokenType.Null)
                                if (roditelj.oznaka != null)
                                { tempRoditelj.otacILImajka = roditelj.oznaka; } else tempRoditelj.otacILImajka = "nema podatka";

                                if (roditelj._embedded == null)
                                {
                                    tempRoditelj.zanimanje = "nema podatka";
                                    tempRoditelj.polozajUZanimanju = "nema podatka";
                                    tempRoditelj.postignutoObrazovanje = "nema podatka";
                                    tempRoditelj.strucnaSprema = "nema podatka";
                                }
                                else

                                {
                                    if (roditelj._embedded.zanimanje != null)
                                    { tempRoditelj.zanimanje = roditelj._embedded.zanimanje.opis; } else tempRoditelj.zanimanje = "nema podatka";

                                    if (roditelj._embedded.polozajUZanimanju != null)
                                    { tempRoditelj.polozajUZanimanju = roditelj._embedded.polozajUZanimanju.opis; } else tempRoditelj.polozajUZanimanju = "nema podatka";

                                    if (roditelj._embedded.postignutoObrazovanje != null)
                                    { tempRoditelj.postignutoObrazovanje = roditelj._embedded.postignutoObrazovanje.naziv; } else tempRoditelj.postignutoObrazovanje = "nema podatka";

                                    if (roditelj._embedded.strucnaSprema != null)
                                    { tempRoditelj.strucnaSprema = roditelj._embedded.strucnaSprema.naziv; } else tempRoditelj.strucnaSprema = "nema podatka";
                                }

                               

                                studentPodaci.Roditelji.Add(tempRoditelj);
                            }
                        }
                    }
                }

                // provjera dali stranica postoji
                var response = await client.GetAsync(String.Format(URL4, studentId));
                // u slucaju da postoji (HttpStatusCode.OK), parsaj podatke
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (var streamTask = await client.GetStreamAsync(String.Format(URL4, studentId)))
                    {
                        StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                        test = reader.ReadToEnd();
                        SEUS.PodatkomatPrelazakKlase.RootObject studentovPrelazak = JsonConvert.DeserializeObject<SEUS.PodatkomatPrelazakKlase.RootObject>(test);
                        if (studentovPrelazak != null)
                        {
                            studentPodaci.prelazakIzUstanove = studentovPrelazak._embedded.studentovPrelazak.organizacijskaJedinica.naziv;
                            studentPodaci.prelazakDatumRjesenja = studentovPrelazak._embedded.rjesenjeOPrelasku.datumRjesenje;
                            studentPodaci.prelazakDatumUpisa = studentovPrelazak._embedded.studentovPrelazak.datumUpisa;
                            studentPodaci.prelazakIskoristenoSemestra = studentovPrelazak._embedded.studentovPrelazak.iskoristenoSemestaraNaTeretMinistarstva.ToString();
                        }

                    }
                }
                else  // u slucaju da stranica ne postoji (student nema prelazaka), vrati dolje navedenu poruku
                {
                    studentPodaci.prelazakIzUstanove = null;
                    //studentPodaci.prelazakDatumRjesenja = "Nema podatka.";
                    //studentPodaci.prelazakDatumUpisa = "Nema podatka.";
                    //studentPodaci.prelazakIskoristenoSemestra = "Nema podatka";
                }

            }

            catch (WebException we)
            {
                var webResponse = we.Response as HttpWebResponse;
                if (webResponse != null &&
                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostojeæi JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("Podaci", studentPodaci);
        }
    }
}