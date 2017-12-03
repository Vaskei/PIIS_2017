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

        string URL = "https://www.isvu.hr/apiproba/vu/313/student/v2/jmbag/{0}";


            Student studentPodaci = null;

        if (String.IsNullOrEmpty(jmbag))
        {
            return View("Podaci", studentPodaci);
        }

        string studentId = jmbag;

        var client = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes("d1.mev:okay965228siqu");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        try
        {
            studentPodaci = new Student();
            string test;

            using (var streamTask = await client.GetStreamAsync(String.Format(URL, studentId)))
            {
                StreamReader reader = new StreamReader(streamTask, Encoding.UTF8);
                test = reader.ReadToEnd();

                SEUS.Podatkomat.Student podaciOStudentu = JsonConvert.DeserializeObject<SEUS.Podatkomat.Student>(test);

                    // Semestar tempSemestar;
                    // Kolegij tempKolegij;
                    if (podaciOStudentu != null)
                    {

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

        return View("Podaci", studentPodaci);
    }
}
}