using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace SEUS.Controllers
{
    public class UserController : Controller
    {
        const string SessionKeyOib = "_s_oib";
        const string SessionKeyMail = "_s_mail";
        const string SessionKeyFirstName = "_s_first_name";
        const string SessionKeyLastName = "_s_first_name";

        //[RequireHttps]
        public IActionResult Login()
        {
            if (!Request.Host.Value.StartsWith("localhost:")) {
                Guid g;
                using (var conn = ConnectionMultiplexer.Connect("localhost:6379"))
                {
                    var db = conn.GetDatabase();
                    g = Guid.NewGuid();

                    db.StringSet(g.ToString(), true);
                }
                return Redirect("http://dev1.mev.hr/seus_auth/index.php?guid=" + g);
            } else {
                HttpContext.Session.SetString(SessionKeyOib, "12345");
                HttpContext.Session.SetString(SessionKeyMail, "lokalna@veza");
                HttpContext.Session.SetString(SessionKeyFirstName, "first_name");
                HttpContext.Session.SetString(SessionKeyLastName, "last_name");
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            return Redirect("http://dev1.mev.hr/seus_auth/logout.php");
        }
        
        [HttpGet]
        public IActionResult CheckLogin()
        {
            string guidIzUrl = HttpContext.Request.Query["guid"].ToString();
            //bool valid = false;
            using (var conn = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                var db = conn.GetDatabase();

                //valid = db.KeyExists(guidIzUrl);
                //test = db.StringGet(guidIzUrl);
                if (db.HashExists(guidIzUrl, "mail")){
                    HttpContext.Session.SetString(SessionKeyMail, db.HashGet(guidIzUrl, "mail"));
                    HttpContext.Session.SetString(SessionKeyOib, db.HashGet(guidIzUrl, "oib"));
                    HttpContext.Session.SetString(SessionKeyFirstName, db.HashGet(guidIzUrl, "first_name"));
                    HttpContext.Session.SetString(SessionKeyLastName, db.HashGet(guidIzUrl, "last_name"));
                    return RedirectToAction("Index", "Home");
                }
            }
            return BadRequest();
        }

        public IActionResult CheckLogout()
        {
            if (HttpContext.Session.GetString(SessionKeyOib) != null)
            {
                HttpContext.Session.Remove(SessionKeyMail);
                HttpContext.Session.Remove(SessionKeyOib);
                HttpContext.Session.Remove(SessionKeyFirstName);
                HttpContext.Session.Remove(SessionKeyLastName);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}