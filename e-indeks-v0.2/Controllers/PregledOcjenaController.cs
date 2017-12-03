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

namespace SEUS.PregledOcjena.Controllers
{

    public class PregledOcjena : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public PregledOcjena(IOptions<AppSettings> appSettings, ILogger<PregledOcjena> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }


        [HttpGet]
        public async Task<ActionResult> Ocjene(string jmbag)
        // public ActionResult PregledStudomata(string jmbag)
        {
            log.LogWarning("PregledStudomata pokrenuto");

            string URL = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("PregledStudomat", studentInfo);
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
                    ViewBag.Error = "Nepostojeći JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("PregledStudomat", studentInfo);
        }
    }

}
