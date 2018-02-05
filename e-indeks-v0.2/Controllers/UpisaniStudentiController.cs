using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using e_Index.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SEUS.Modelss;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.IO;



namespace SEUS.Controllers
{
    public class UpisaniStudentiController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger log;

        public UpisaniStudentiController(IOptions<AppSettings> appSettings, ILogger<UpisaniStudentiController> log)
        {
            _appSettings = appSettings.Value;
            this.log = log;
        }

        [HttpGet]
        public async Task<ActionResult> Upisani(string godina)
        {

            log.LogWarning("Pregled liste studenta pokrenuto");

            string URL = "https://www.isvu.hr/apiproba/vu/313/student/studentuakademskojgodini/studenti/akademskagodina/{0}";

            StudentiInfo upisaniStudenti = null;

            if (String.IsNullOrEmpty(godina))
            {
                return View("Upisani", upisaniStudenti);
            }

            string godinaID = godina;

            var client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            try
            {
                upisaniStudenti = new StudentiInfo();
                string test;

                using (var streamTask = await client.GetStreamAsync(String.Format(URL, godinaID)))
                {
                    StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                    test = reader.ReadToEnd();

                    RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(test);

                    StudentModel tempStudent;

                    if (rootObject != null)
                    {
                        foreach (var student in rootObject._embedded.studenti)
                        {                          
                                tempStudent = new StudentModel();
                                tempStudent.Ime = student.ime;
                                tempStudent.Prezime = student.prezime;
                                tempStudent.Jmbag = student.jmbag;                              
                                                   
                            upisaniStudenti.StudentiList.Add(tempStudent);
                        }

                        upisaniStudenti.Godina = godinaID;
                        upisaniStudenti.StudentiList.Sort((x, y) => x.Ime.CompareTo(y.Ime));                        
                    }
                }
            }
            catch (WebException e)
            {
                var webResponse = e.Response as HttpWebResponse;
                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    ViewBag.Error = "Nepostoječi ";
                }
                else
                    ViewBag.Error = e.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View("Upisani", upisaniStudenti);

        }
    }
}
