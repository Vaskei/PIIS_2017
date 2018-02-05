using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using e_Index.Misc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;


namespace SEUS.PregledOcjena.Controllers
{

    public class Informacije : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public Informacije(IOptions<AppSettings> appSettings, ILogger<Informacije> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }


        [HttpGet]
        public async Task<ActionResult> Info(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("Projekt pokrenut");

            string URL = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("Informacije", studentInfo);
            }

            string studentId = jmbag;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                studentInfo = new StudentInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URL, studentId)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    e_Index.Studomat.RootObject studomat = JsonConvert.DeserializeObject<e_Index.Studomat.RootObject>(test);

                    Semestar tempSemestar;
                    Kolegij tempKolegij;

                    if (studomat != null)
                    {
                        studentInfo.Naziv = studomat._embedded.student.prezime + " " + studomat._embedded.student.ime;

                        int ectsUkupno = 0;
                        int ectsOsvojeno = 0;

                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {
                            ectsUkupno = 0;
                            ectsOsvojeno = 0;

                            Studij studijInfo = new Studij() { Naziv = studij.naziv };

                            foreach (var kolegij in studij._embedded.polozeniPredmeti)
                            {
                                //Provjera semestra
                                tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                if (tempSemestar == null)
                                {
                                    tempSemestar = new Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                    studijInfo.Semestri.Add(tempSemestar);
                                }

                                //Dodavanje kolegija u semestar
                                tempKolegij = new Kolegij();
                                tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                tempKolegij.ECTS = (int)kolegij._embedded.predmet.ectsBodovi;
                                ectsUkupno += tempKolegij.ECTS;
                                if (kolegij._embedded.ispit != null)
                                {
                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                    tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                }


                                tempSemestar.Kolegiji.Add(tempKolegij);
                                if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                {
                                    ectsOsvojeno += tempKolegij.ECTS;
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
                                        tempSemestar = new e_Index.Misc.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }

                                    //Dodavanje kolegija u semestar
                                    tempKolegij = new Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    tempKolegij.ECTS = (int)kolegij._embedded.predmet.ectsBodovi;
                                    ectsUkupno += tempKolegij.ECTS;
                                    if (kolegij._embedded.ispit != null)
                                    {
                                        tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                        tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;

                                        ectsOsvojeno += tempKolegij.ECTS;
                                    }

                                    tempSemestar.Kolegiji.Add(tempKolegij);
                                    if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                    {
                                        ectsOsvojeno += tempKolegij.ECTS;
                                        tempKolegij.Polozen = true;
                                    }
                                }
                                catch
                                {

                                }
                            }

                            studijInfo.ECTS_Osvojeno = ectsOsvojeno;
                            studijInfo.ECTS_Ukupno = ectsUkupno;

                            studentInfo.Studiji.Add(studijInfo);
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
                    ViewBag.Error = "Nepostojeþi JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("Informacije", studentInfo);
        }
        public IActionResult kolegij(string ime)
        {
            if (ime == "Matematika I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72282.shtml");
            }
            else if (ime == "Tjelesna i zdravstvena kultura")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72285.shtml");
            }
            else if (ime == "Engleski jezik I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72280.shtml");
            }
            else if (ime == "Ekonomika i organizacija poslovnih sustava")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72287.shtml");
            }
            else if (ime == "Fizika")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72281.shtml");
            }
            else if (ime == "Osnove elektrotehnike i elektronike")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72283.shtml");
            }
            else if (ime == "Engleski jezik II")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72288.shtml");
            }
            else if (ime == "Tjelesna i zdravstvena kultura II")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72291.shtml");
            }
            else if (ime == "Matematika II")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72289.shtml");
            }
            else if (ime == "Primjena raèunala")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72284.shtml");
            }
            else if (ime == "Programiranje")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72290.shtml");
            }
            else if (ime == "Digitalni elektronièki sklopovi")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72286.shtml");
            }
            else if (ime == "Vje¹tine komuniciranja")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72297.shtml");
            }
            else if (ime == "Tjelesna i zdravstvena kultura III")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72295.shtml");
            }
            else if (ime == "Algoritmi i strukture podataka")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72292.shtml");
            }
            else if (ime == "Objektno orijentirano programiranje I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72328.shtml");
            }
            else if (ime == "Vjerojatnost i statistika")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72296.shtml");
            }
            else if (ime == "Arhitektura raèunala")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72293.shtml");
            }
            else if (ime == "Programski alati u programiranju")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72304.shtml");
            }
            else if (ime == "Tjelesna i zdravstvena kultura IV")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72306.shtml");
            }
            else if (ime == "Baze podataka I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72298.shtml");
            }
            else if (ime == "Operacijski sustavi")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72294.shtml");
            }
            else if (ime == "Raèunalne mre¾e")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72305.shtml");
            }

            else if (ime == "Sigurnost raèunalnih umre¾enih sustava")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72333.shtml");
            }
            else if (ime == "Administracija raèunalnih mre¾a")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72332.shtml");
            }
            else if (ime == "Seminar M")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred76159.shtml");
            }
            else if (ime == "Menad¾ment")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72322.shtml");
            }
            else if (ime == "Praksa")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred167171.shtml");
            }
            else if (ime == "Zavr¹ni rad")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred167169.shtml");
            }

            else if (ime == "Tjelesna i zdravstvena kultura III")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72295.shtml");
            }
            else if (ime == "Objektno orijentirano programiranje I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72328.shtml");
            }
            else if (ime == "Vje¹tine komuniciranja")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72297.shtml");
            }
            else if (ime == "Arhitektura raèunala")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72293.shtml");
            }
            else if (ime == "Algoritmi i strukture podataka")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72292.shtml");
            }
            else if (ime == "Vjerojatnost i statistika")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72296.shtml");
            }
            else if (ime == "Tjelesna i zdravstvena kultura IV")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72306.shtml");
            }
            else if (ime == "Operacijski sustavi")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72294.shtml");
            }
            else if (ime == "Programski alati u programiranju")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72304.shtml");
            }
            else if (ime == "Raèunalne mre¾e")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72305.shtml");
            }
            else if (ime == "Baze podataka I")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72298.shtml");
            }
            else if (ime == "Programsko in¾enjerstvo i informacijski sustavi")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72330.shtml");
            }
            else if (ime == "Seminar A")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred76160.shtml");
            }
            else if (ime == "Objektno orijentirano programiranje II")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72329.shtml");
            }
            else if (ime == "Menad¾ment")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred72322.shtml");
            }
            else if (ime == "Praksa")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred167171.shtml");
            }
            else if (ime == "Zavr¹ni rad A")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred76162.shtml");
            }
            else if (ime == "Zavr¹ni rad")
            {
                return Redirect("http://www.isvu.hr/javno/hr/vu313/nasprog/2017/pred167169.shtml");
            }
            else
            {
                return View("Informacije");
            }
        }
    }
}
