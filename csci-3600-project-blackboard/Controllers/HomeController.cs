using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using csci_3600_project_the_struggle.Data;

namespace csci_3600_project_the_struggle.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<AspNetUser> users;
            using (DataModel dm = new DataModel())
            {
                users = dm.AspNetUsers.ToList();
            }

            ViewBag.User = users;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}