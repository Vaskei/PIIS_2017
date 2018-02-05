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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using SEUS.Models;

namespace SEUS.Izostanci.Controllers
{
    public class Izostanci : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public Izostanci(IOptions<AppSettings> appSettings, ILogger<Izostanci> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }

        public IActionResult Index(){
            return View();
        }

        [HttpGet]
        public IActionResult Profesori(string jmbag)
        {   
            if(jmbag!=null)
                ViewBag.v= "vrati";
            return View();
        }

        public IActionResult Studenti()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ZaProfesore(string jmbag)
        {
            int predmet = 0;
            List<IzoPredmet> sviPredmeti = new List<IzoPredmet>();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=dev1.mev.hr;" +
                                          "Initial Catalog=piis2018_e7_podaci1;" +
                                          "User id=piis2018_e7_user;" +
                                          "Password=4A9Nktuc4cVx";
            string query = "SELECT * FROM profesori WHERE ime='" + jmbag + "'";
            try
            {
                SqlCommand sqlcommand = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader sqlreader = sqlcommand.ExecuteReader();
                if (sqlreader.HasRows)
                {
                    while (sqlreader.Read())
                    {
                        predmet = (int)sqlreader["predmet"];
                    }
                    connection.Close();
                    query = "SELECT * FROM dolasci WHERE idPredmet=" + predmet;
                    IzoPredmet tPredmet = new IzoPredmet();
                    sqlcommand = new SqlCommand(query, connection);
                    connection.Open();
                    sqlreader = sqlcommand.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {
                            tPredmet.idKorisnik = (int)sqlreader["idKorisnik"];
                            tPredmet.idPredmet = (int)sqlreader["idPredmet"];
                            tPredmet.tjedan1 = (int)sqlreader["tjedan1"];
                            tPredmet.tjedan2 = (int)sqlreader["tjedan2"];
                            tPredmet.tjedan3 = (int)sqlreader["tjedan3"];
                            tPredmet.tjedan4 = (int)sqlreader["tjedan4"];
                            tPredmet.tjedan5 = (int)sqlreader["tjedan5"];
                            tPredmet.tjedan6 = (int)sqlreader["tjedan6"];
                            tPredmet.tjedan7 = (int)sqlreader["tjedan7"];
                            tPredmet.tjedan8 = (int)sqlreader["tjedan8"];
                            tPredmet.tjedan9 = (int)sqlreader["tjedan9"];
                            tPredmet.tjedan10 = (int)sqlreader["tjedan10"];
                            tPredmet.tjedan11 = (int)sqlreader["tjedan11"];
                            tPredmet.tjedan12 = (int)sqlreader["tjedan12"];
                            tPredmet.tjedan13 = (int)sqlreader["tjedan13"];
                            tPredmet.tjedan14 = (int)sqlreader["tjedan14"];
                            tPredmet.tjedan15 = (int)sqlreader["tjedan15"];
                            tPredmet.ttjedan1 = (int)sqlreader["ttjedan1"];
                            tPredmet.ttjedan2 = (int)sqlreader["ttjedan2"];
                            tPredmet.ttjedan3 = (int)sqlreader["ttjedan3"];
                            tPredmet.ttjedan4 = (int)sqlreader["ttjedan4"];
                            tPredmet.ttjedan5 = (int)sqlreader["ttjedan5"];
                            tPredmet.ttjedan6 = (int)sqlreader["ttjedan6"];
                            tPredmet.ttjedan7 = (int)sqlreader["ttjedan7"];
                            tPredmet.ttjedan8 = (int)sqlreader["ttjedan8"];
                            tPredmet.ttjedan9 = (int)sqlreader["ttjedan9"];
                            tPredmet.ttjedan10 = (int)sqlreader["ttjedan10"];
                            tPredmet.ttjedan11 = (int)sqlreader["ttjedan11"];
                            tPredmet.ttjedan12 = (int)sqlreader["ttjedan12"];
                            tPredmet.ttjedan13 = (int)sqlreader["ttjedan13"];
                            tPredmet.ttjedan14 = (int)sqlreader["ttjedan14"];
                            tPredmet.ttjedan15 = (int)sqlreader["ttjedan15"];
                            sviPredmeti.Add(tPredmet);
                        }
                    }

                }              
            }
            catch
            {

            }
            ViewBag.predmeti = sviPredmeti;
            return View("Profesori", sviPredmeti.Last());
        }
        [HttpGet]
        public async Task<ActionResult> Predmeti(string jmbag)
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


                        foreach (var studij in studomat._embedded.upisaniElementiStruktureStudija)
                        {

                            Studij studijInfo = new Studij() { Naziv = studij.naziv };
                            Semestar trenutni = new Semestar();
                            int semestar = 0;
                            List<Kolegij>listKolegija = new List<Kolegij>();

                            foreach (var predmet in studij._embedded.nepolozeniPredmeti)
                            {

                                try
                                {
                                    var www = JObject.FromObject(predmet);
                                    string w2 = www.ToString();

                                    e_Index.Studomat.PolozeniPredmeti kolegij = JsonConvert.DeserializeObject<e_Index.Studomat.PolozeniPredmeti>(w2);
                                    if (kolegij._embedded.predmet._embedded.semestar.redniBroj > semestar)
                                    {
                                        semestar = kolegij._embedded.predmet._embedded.semestar.redniBroj;
                                    }

                                    tempSemestar = studijInfo.Semestri.FirstOrDefault(x => x.Oznaka == kolegij._embedded.predmet._embedded.semestar.redniBroj);
                                    if (tempSemestar == null)
                                    {
                                        tempSemestar = new e_Index.Misc.Semestar() { Oznaka = kolegij._embedded.predmet._embedded.semestar.redniBroj };
                                        studijInfo.Semestri.Add(tempSemestar);
                                    }


                                    //Dodavanje kolegija u semestar
                                    tempKolegij = new Kolegij();
                                    tempKolegij.Naziv = kolegij._embedded.predmet.naziv;
                                    int sifra = kolegij._embedded.predmet.sifra;
                                    tempSemestar.Kolegiji.Add(tempKolegij);

                                    //spajanje na bazu podataka
                                    SqlConnection connection = new SqlConnection();
                                    connection.ConnectionString = "Data Source=dev1.mev.hr;" +
                                                                  "Initial Catalog=piis2018_e7_podaci1;" +
                                                                  "User id=piis2018_e7_user;" +
                                                                  "Password=4A9Nktuc4cVx";
                                    string query = "SELECT * FROM dolasci WHERE idKorisnik="+jmbag +" AND idPredmet="+sifra;
                                    try
                                    {
                                        SqlCommand sqlcommand = new SqlCommand(query, connection);
                                        connection.Open();
                                        SqlDataReader sqlreader = sqlcommand.ExecuteReader();
                                        //Ako je veæ kolegij upisan u bazu podataka
                                        if (sqlreader.HasRows == true)
                                        {
                                            while (sqlreader.Read())
                                            {
                                                ViewData["tjedan1"] = sqlreader["tjedan1"];
                                                ViewData["tjedan2"] = sqlreader["tjedan2"];
                                                ViewData["tjedan3"] = sqlreader["tjedan3"];
                                                ViewData["tjedan4"] = sqlreader["tjedan4"];
                                                ViewData["tjedan5"] = sqlreader["tjedan5"];
                                                ViewData["tjedan6"] = sqlreader["tjedan6"];
                                                ViewData["tjedan7"] = sqlreader["tjedan7"];
                                                ViewData["tjedan8"] = sqlreader["tjedan8"];
                                                ViewData["tjedan9"] = sqlreader["tjedan9"];
                                                ViewData["tjedan10"] = sqlreader["tjedan10"];
                                                ViewData["tjedan11"] = sqlreader["tjedan11"];
                                                ViewData["tjedan12"] = sqlreader["tjedan12"];
                                                ViewData["tjedan13"] = sqlreader["tjedan13"];
                                                ViewData["tjedan14"] = sqlreader["tjedan14"];
                                                ViewData["tjedan15"] = sqlreader["tjedan15"];
                                                ViewData["ttjedan1"] = sqlreader["ttjedan1"];
                                                ViewData["ttjedan2"] = sqlreader["ttjedan2"];
                                                ViewData["ttjedan3"] = sqlreader["ttjedan3"];
                                                ViewData["ttjedan4"] = sqlreader["ttjedan4"];
                                                ViewData["ttjedan5"] = sqlreader["ttjedan5"];
                                                ViewData["ttjedan6"] = sqlreader["ttjedan6"];
                                                ViewData["ttjedan7"] = sqlreader["ttjedan7"];
                                                ViewData["ttjedan8"] = sqlreader["ttjedan8"];
                                                ViewData["ttjedan9"] = sqlreader["ttjedan9"];
                                                ViewData["ttjedan10"] = sqlreader["ttjedan10"];
                                                ViewData["ttjedan11"] = sqlreader["ttjedan11"];
                                                ViewData["ttjedan12"] = sqlreader["ttjedan12"];
                                                ViewData["ttjedan13"] = sqlreader["ttjedan13"];
                                                ViewData["ttjedan14"] = sqlreader["ttjedan14"];
                                                ViewData["ttjedan15"] = sqlreader["ttjedan15"];
                                            }
                                            
                                        }
                                        else //Ako ne postoji zapis u bazi podataka stvori ga
                                        {
                                            using (SqlConnection openCon = new SqlConnection("Data Source=dev1.mev.hr;" +
                                                                  "Initial Catalog=piis2018_e7_podaci1;" +
                                                                  "User id=piis2018_e7_user;" +
                                                                  "Password=4A9Nktuc4cVx"))
                                            {
                                                string saveStaff = "INSERT INTO dolasci (idKorisnik, idPredmet, tjedan1, tjedan2, tjedan3, tjedan4, tjedan5, tjedan6, tjedan7, tjedan8, tjedan9, tjedan10, tjedan11, tjedan12, tjedan13, tjedan14, tjedan15, ttjedan1, ttjedan2, ttjedan3, ttjedan4, ttjedan5, ttjedan6, ttjedan7, ttjedan8, ttjedan9, ttjedan10, ttjedan11, ttjedan12, ttjedan13, ttjedan14, ttjedan15, imePrezime, imePredmeta) VALUES (@korisnik, @predmet, '1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1',@imePrezime,@imePredmeta)";

                                                using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                                                {
                                                    querySaveStaff.Connection = openCon;
                                                    querySaveStaff.Parameters.Add("@korisnik", SqlDbType.VarChar, 50).Value = jmbag;
                                                    querySaveStaff.Parameters.Add("@predmet", SqlDbType.VarChar, 50).Value = sifra;
                                                    querySaveStaff.Parameters.Add("@imePrezime", SqlDbType.VarChar, 50).Value = studentInfo.Naziv;
                                                    querySaveStaff.Parameters.Add("@imePredmeta", SqlDbType.VarChar, 50).Value = tempKolegij.Naziv;
                                                    openCon.Open();
                                                    querySaveStaff.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                        
                                    }
                                    catch (SqlException e)
                                    {
                                        log.LogWarning(e.Message);
                                    }
                                    finally
                                    {
                                        connection.Close();
                                    }
                                }
                                catch (Exception e)
                                {
                                    e.StackTrace.ToString();
                                }
                            }
                            return View("Studenti", studijInfo.Semestri.Last(x => x.Oznaka == semestar));
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
                    ViewBag.Error = "Nepostojeæi JMBAG studenta!";
                }
                else
                    ViewBag.Error = we.Message;
            }
            catch (Exception ee)
            {
                ViewBag.Error = ee.Message;
            }

            return View("Studenti");
        }
        public void dodavanjeUBazu(string jmbag)
        {
            string idKorisnik = jmbag;
        }
    }
}