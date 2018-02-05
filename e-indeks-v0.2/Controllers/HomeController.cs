using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SEUS.PregledOcjena.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Popis studenata koji su radili na ovome projektu";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

 

        public IActionResult RangLista()
        {
            ViewData["Message"] = "Rang lista.";

            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
