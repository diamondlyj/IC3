using AIn.Ventures.BaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Session["User"]!=null)
                return View();
            //ViewBag.Error = "no user";
            //return RedirectToAction("Index","Login",null);
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