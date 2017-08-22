using AIn.Ventures.Models;
using AIn.Ventures.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserIdAndPw usr)
        {
            ViewBag.Error = "????";

            JWToken token = new JWToken();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"username", usr.EmailAddress},
                    {"password", usr.Password},
                };

                var tokenResponse = client.PostAsync("https://localhost:4243/Token", new FormUrlEncodedContent(form)).Result;
                if (tokenResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Logged in right but something else is wrong.";
                    token = tokenResponse.Content.ReadAsAsync<JWToken>(new[] { new JsonMediaTypeFormatter() }).Result;
                    var userResponse = client.GetAsync("https://localhost:4243/User").Result;


                    var user = userResponse.Content.ReadAsAsync<User>(new[] { new JsonMediaTypeFormatter() }).Result;

                    
                    //var colleaguesResponse = client.GetAsync(String.Concat("https://localhost:4243/Colleagues/", user.GUID)).Result;
                    //var colleagues = colleaguesResponse.Content.ReadAsAsync<List<Colleague>>(new[] { new JsonMediaTypeFormatter() }).Result;
                    //foreach (Colleague c in colleagues)
                    //{
                    //    user.Colleagues.Add(c);
                    //}


                    // Get projects method not working
                    //var projectsResponse = client.GetAsync(String.Concat("https://localhost:4243/Projects/", user.GUID)).Result;
                    //var projects = projectsResponse.Content.ReadAsAsync<List<Project>>(new[] { new JsonMediaTypeFormatter() }).Result;
                    //foreach(Project p in projects)
                    //{
                    //    user.Projects.Add(p);
                    //}
                    Session.Add("User", user);
                    Session.Add("Token", token);
                    return RedirectToAction("Index", "Home", null);

                }
                ViewBag.Error = "Incorrect username or password";
                return RedirectToAction("Index", "Login", null);

            }
        }
    }
}