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
using System.Data.SqlClient;
using System.Collections.Generic;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SEUS.Controllers
{
    public class RangLista : Controller
    {


        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public RangLista(IOptions<AppSettings> appSettings, ILogger<RangLista> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }



        [HttpGet]
        public async Task<ActionResult> RangListaView(string jmbag)
        {
            log.LogWarning("Pregled rang liste pokrenuto");

            string URL = "https://www.isvu.hr/apiproba/vu/313/student/jmbag/{0}/sumarno";

            StudentInfo studentInfo = null;

            if (String.IsNullOrEmpty(jmbag))
            {
                return View("RangListaView", studentInfo);
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



                    if (studomat != null)
                    {
                        studentInfo.Naziv = studomat._embedded.student.prezime + " " + studomat._embedded.student.ime;

                        Semestar tempSemestar;
                        Kolegij tempKolegij;

                        int ectsUkupno = 0;
                        int ectsOsvojeno = 0;
                        int kolegijEcts = 0;
                        int kolegijOcjena = 0;
                        float TP = 0;


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
                                tempKolegij.ECTS = (int)kolegij._embedded.predmet.ectsBodovi;
                                kolegijEcts = (int)kolegij._embedded.predmet.ectsBodovi;
                                ectsUkupno += tempKolegij.ECTS;

                                if (kolegij._embedded.ispit != null)
                                {
                                    tempKolegij.Ocjena = kolegij._embedded.ispit.ocjena;
                                    kolegijOcjena = Int32.Parse(kolegij._embedded.ispit.ocjena);
                                    tempKolegij.Profesor = kolegij._embedded.ispit._embedded.ocjenjivac.prezime + " " + kolegij._embedded.ispit._embedded.ocjenjivac.ime;
                                    TP = (kolegijOcjena * kolegijEcts) + TP;
                                }


                                tempSemestar.Kolegiji.Add(tempKolegij);
                                if (kolegij._embedded.predmet._embedded.status.sifra == 5)
                                {
                                    ectsOsvojeno += tempKolegij.ECTS;
                                    tempKolegij.Polozen = true;
                                }
                            }

                            try
                            {
                                foreach (var predmet in studij._embedded.nepolozeniPredmeti)
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

                            }
                            catch (WebException we)
                            {
                                var webResponse = we.Response as HttpWebResponse;
                                if (webResponse != null &&
                                    webResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                                {
                                    ViewBag.Error = "Nepostojeći JMBAG studenta test!";
                                }
                                else
                                    ViewBag.Error = we.Message;
                            }

                            studijInfo.ECTS_Osvojeno = ectsOsvojeno;
                            studijInfo.ECTS_Ukupno = ectsUkupno;
                            
                            studentInfo.ectsUkupno = ectsUkupno;
                            studentInfo.ECTSOsvojeno = ectsOsvojeno;

                            studentInfo.TePr = TP / ectsOsvojeno;
                            //godina studija
                            studentInfo.godina = 1;
                            if (ectsUkupno > 70) { studentInfo.godina = 2; }
                            if (ectsUkupno > 130) { studentInfo.godina = 3; }
                            //upis podataka u tablicu ...
                    
                            log.LogWarning("prije provjere " + checkStudentInfo(studentId));
                            if (checkStudentInfo(studentId) >= 1)
                            {
                                log.LogWarning("update provjere " );
                                updateStudentInfo(studentInfo, studentId);
                            }
                           else {
                                log.LogWarning("insert provjere " );
                                insertStudentInfo(studentInfo, studentId);
                            }

                            //dohvat svih prijavljenih studenata
                            studentInfo.SviStudenti = selectAllStudentInfo();
                            List< StudentInfo > stud = selectAllStudentInfo();


                            log.LogWarning("glavna metoda: " + stud.Count);
                            foreach (var StudentInfo in stud)
                            {

                                log.LogWarning("glavna metoda: " + StudentInfo.Naziv);

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.LogWarning("Greška kod čitanja podataka");
                ViewBag.Error = ex.Message;
                studentInfo = null;

            }

            return View("RangListaView", studentInfo);
        }


        [HttpGet]
        public async Task<ActionResult> StudentList(string jmbag)
        {
            log.LogWarning("Pregled studenata liste pokrenuto");

            StudentInfo studentInfo = null;

            string studentId = jmbag;

        
            try
            {
                studentInfo = new StudentInfo();

              
                //dohvat svih prijavljenih studenata
                studentInfo.SviStudenti = selectAllStudentInfo();
                List<StudentInfo> stud = selectAllStudentInfo();


                log.LogWarning("glavna metoda: " + stud.Count);
                foreach (var StudentInfo in stud)
                {

                    log.LogWarning("glavna metoda: " + StudentInfo.Naziv);

                }
                  
                
            }
            catch (Exception ex)
            {

                log.LogWarning("Greška kod čitanja podataka");
                ViewBag.Error = ex.Message;
                studentInfo = null;

            }

            return View("StudentList", studentInfo);
        }


        public void insertStudentInfo(StudentInfo studInfo, string jmbag)
        {

            //Podaci o prosjeku ocjena studenta
            string nazivStudenta = studInfo.Naziv;
            int ectsUkupno = studInfo.ectsUkupno;
            int ectsOsvojeno = studInfo.ECTSOsvojeno;

            string ectsUkOsvojeno = ectsUkupno + "/" + ectsOsvojeno;
            double tePr = studInfo.TePr;
            int godina = studInfo.godina;

            //Spajanje na bazu
            SqlConnection con = new SqlConnection();


            con.ConnectionString = "Data Source=dev1.mev.hr;" +
                                    "Initial Catalog=piis2018_e5_podaci1;" +
                                    "User id=piis2018_e5_user;" +
                                    "Password=uLPeq3Y9H5BV;";

       

            string insertStudInfo = "INSERT INTO tSTUDENTI(naziv, ects, teprosjek,jmbag,godina) VALUES(" +
                                    "'" + nazivStudenta + "'" + "," +  ectsOsvojeno  + "," + "'" + tePr + "'" + "," + jmbag + ","+ godina+ ")";
            try
            {
                SqlCommand sqlcommand = new SqlCommand(insertStudInfo, con);

                log.LogWarning("sql: " + insertStudInfo );

                con.Open(); // otvaranja konekcije
                sqlcommand.ExecuteNonQuery(); // izvršavanje inserta
                con.Close(); // zatavaranje konekcije

            }
            catch (SqlException sexp)
            {
                ViewBag.Error = sexp.Message;
                studInfo = null;
            }
            finally
            {
                studInfo = null;
                con.Close();
            }


            log.LogWarning("addStudInfo: " + nazivStudenta + " " + ectsUkupno + " " + ectsOsvojeno + " " + tePr);

        }


        public void updateStudentInfo(StudentInfo studInfo, string jmbag)
        {

            //Podaci o prosjeku ocjena studenta
            string nazivStudenta = studInfo.Naziv;
            int ectsUkupno = studInfo.ectsUkupno;
            int ectsOsvojeno = studInfo.ECTSOsvojeno;

            string ectsUkOsvojeno = ectsUkupno + "/" + ectsOsvojeno;
            double tePr = studInfo.TePr;

            //Spajanje na bazu
            SqlConnection con = new SqlConnection();


            con.ConnectionString = "Data Source=dev1.mev.hr;" +
                                    "Initial Catalog=piis2018_e5_podaci1;" +
                                    "User id=piis2018_e5_user;" +
                                    "Password=uLPeq3Y9H5BV;";



            string insertStudInfo = "UPDATE tSTUDENTI SET godina = "+studInfo.godina+","+"ects ="+ ectsOsvojeno + ", teprosjek="+ "'" + tePr + "'" +" where jmbag =" + jmbag;
            try
            {
                SqlCommand sqlcommand = new SqlCommand(insertStudInfo, con);

                log.LogWarning("sql: " + insertStudInfo);

                con.Open(); // otvaranja konekcije
                sqlcommand.ExecuteNonQuery(); // izvršavanje inserta
                

            }
            catch (SqlException sexp)
            {
                ViewBag.Error = sexp.Message;
                studInfo = null;
            }
            finally
            {
                studInfo = null;
                con.Close();
            }


            log.LogWarning("updaetStudInfo: " + nazivStudenta + " " + ectsUkupno + " " + ectsOsvojeno + " " + tePr);

        }


        public int checkStudentInfo(string jmbag)
        {

            int result =0;

            //Spajanje na bazu
            SqlConnection con = new SqlConnection();


            con.ConnectionString = "Data Source=dev1.mev.hr;" +
                                    "Initial Catalog=piis2018_e5_podaci1;" +
                                    "User id=piis2018_e5_user;" +
                                    "Password=uLPeq3Y9H5BV;";

            string countStudent = "SELECT COUNT(*) broj_stud FROM tStudenti where jmbag =" + jmbag;
          
            try
            {
                SqlCommand sqlcommand = new SqlCommand(countStudent, con);

                log.LogWarning("sql: " + countStudent);

                con.Open(); // otvaranja konekcije

                SqlDataReader reader = sqlcommand.ExecuteReader();

                               
                while (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
          

            }
            catch (SqlException sexp)
            {
                ViewBag.Error = sexp.Message;
                countStudent = null;
            }
            finally
            {
                countStudent = null;
                con.Close();
            }

            return result;

        }


        public List<StudentInfo> selectAllStudentInfo()
        {
            List<StudentInfo> listStudenti = new List<StudentInfo>();
            StudentInfo tmpstudent = new StudentInfo();
            StudentInfo student = new StudentInfo();
            //Spajanje na bazu
            SqlConnection con = new SqlConnection();
            
        
            con.ConnectionString = "Data Source=dev1.mev.hr;" +
                                    "Initial Catalog=piis2018_e5_podaci1;" +
                                    "User id=piis2018_e5_user;" +
                                    "Password=uLPeq3Y9H5BV;";

            string allStudents = "SELECT id,naziv,ects,teprosjek,godina FROM tStudenti order by teprosjek desc;";
            int count = 0;
            try
            {
                SqlCommand sqlcommand = new SqlCommand(allStudents, con);

                log.LogWarning("sql: " + allStudents);

                con.Open(); // otvaranja konekcije

                SqlDataReader reader = sqlcommand.ExecuteReader();

                log.LogWarning("reader: " + reader.FieldCount);
               
                while (reader.Read())
                {
                    student = new StudentInfo();
                    student.id = reader.GetInt32(0);
                    student.Naziv = reader.GetString(1);
                    student.ECTSOsvojenoB = reader.GetInt64(2);
                    student.TePrB = reader.GetString(3);
                    student.godina = reader.GetInt32(4);

                    tmpstudent.SviStudenti.Insert(count,student);
                    count++;          
                }

                foreach (var StudentInfo in tmpstudent.SviStudenti)
                {

                    log.LogWarning("inmethod: " + StudentInfo.Naziv);

                }

                /*
                foreach (var StudentiInfo in listStudenti)
                {

                    log.LogWarning("readersve: " + StudentiInfo.Naziv);

                }
                */
                  

            }
            catch (SqlException sexp)
            {
                ViewBag.Error = sexp.Message;
                allStudents = null;
            }
            finally
            {
                
                con.Close();

            }

            return tmpstudent.SviStudenti;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
