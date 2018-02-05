using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using e_Index.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
//using SEUS.Models;
using SEUS.Modelss;



namespace SEUS.Controllers
{
    public class SkolarineController: Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public SkolarineController(IOptions<AppSettings> appSettings, ILogger<SkolarineController> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }

        [HttpGet]
        public async Task<ActionResult> Evidencija(string jmbag)
        {

            log.LogWarning("Pregled skolarina pokrenut");

            //evidencija školarina
            string URL = "https://www.isvu.hr/apiproba/vu/313/upisgodine/skolarina/student/jmbag/{0}";

            //evidencija transakcija
            string URL1 = "https://www.isvu.hr/apiproba/vu/313/upisgodine/skolarina/student/jmbag/{0}/sifraUpisaGodine/{1}";

            //student informacije
            string URL2 = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";

            SkolarineInfo evidentiraneSkolarine = null;           

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("Evidencija", evidentiraneSkolarine);
            }

            string studentID = jmbag;
            
            List<int> sifraID = new List<int>();

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                //Školarine
                evidentiraneSkolarine = new SkolarineInfo() { Ime="Nema podataka o studentovim školarinama"};          
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URL, studentID)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(test);

                    SkolarineModel tempSkolarine = null;

                    if (rootObject != null)
                    {
                        foreach (var skolarina in rootObject._embedded.skolarine)
                        {
                            tempSkolarine = new SkolarineModel()
                            {
                                sifraUpisaGodine = skolarina.sifraUpisaGodine,
                                akademskaGodina = skolarina.akademskaGodina,
                                nastavnaGodina = skolarina.nastavnaGodina,
                                paralelniStudij = skolarina.paralelniStudij,
                                ukupniSaldo = skolarina.ukupniSaldo,                               
                            };
                           
                            sifraID.Add(tempSkolarine.sifraUpisaGodine);
                            evidentiraneSkolarine.SkolarinaList.Add(tempSkolarine);
                        }                       
                    }                   
                }
               
                //Transakcije
                int i = 0;
                do
                {
                    using (var streamTask = await client.GetStreamAsync(String.Format(URL1, studentID, sifraID[i])))
                    {
                        StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                        test = reader.ReadToEnd();

                        RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(test);

                        TransakcijeModel tempTransakcije = null;

                        if (rootObject != null)
                        {                           
                            evidentiraneSkolarine.Ime = rootObject._embedded.student.ime;
                            evidentiraneSkolarine.Prezime = rootObject._embedded.student.prezime;

                            foreach (var transakcija in rootObject._embedded.transakcije._embedded.transakcije)
                            {
                                tempTransakcije = new TransakcijeModel()
                                {
                                    sifra = transakcija.sifra,
                                    redniBroj = transakcija.redniBroj,
                                    datumTransakcije = transakcija.datumTransakcije,
                                    vrstaTransakcije = transakcija.vrstaTransakcije,
                                    iznosDugovanja = transakcija.iznosDugovanja,
                                    knjizeno = transakcija.knjizeno,
                                    iznosPotrazivanja = transakcija.iznosPotrazivanja,
                                };
                            }
                            evidentiraneSkolarine.TransakcijeList.Add(tempTransakcije);
                        }                        
                    }
                    i++;
                }
                while (i < sifraID.Count());
              
                //student informacije
                using (var streamTask = await client.GetStreamAsync(String.Format(URL2, studentID)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(test);                   

                    if (rootObject != null)
                    {
                        evidentiraneSkolarine.Naziv = rootObject._embedded.osobniPodaci._embedded.maticnoVisokoUciliste.naziv;                       
                    }
                }
            }
            catch (WebException e)
            {
                var webResponse = e.Response as HttpWebResponse;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoječi JMBAG studenta!";
                }
                else
                    ViewBag.Error = e.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("Evidencija", evidentiraneSkolarine);
            
        }
    }
}
