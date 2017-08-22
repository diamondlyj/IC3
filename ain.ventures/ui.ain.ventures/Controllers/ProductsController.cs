using AIn.Ventures.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Projects
        [HttpGet, Route("Product/{GUID}")]
        public ActionResult Index(Guid GUID)
        {
            JWToken token = new JWToken();
            Guid RootGuid = GUID;

            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"username", "john5.doe@ain.ventures"},
                    {"password", "somePwd3456!"},
                };
                var tokenResponse = client.PostAsync("https://localhost:4243/Token", new FormUrlEncodedContent(form)).Result;

                token = tokenResponse.Content.ReadAsAsync<JWToken>(new[] { new JsonMediaTypeFormatter() }).Result;
            }

            using (var client = new HttpClient())
            {

                /*var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "john5.doe@ain.ventures"},
                   {"password", "somePwd3456!"},
               };
                var tokenResponse = client.GetStringAsync("https://localhost:4243/Token").Result;

                token = JsonConvert.DeserializeObject<Token>(tokenResponse);*/

                var queryJson = client.GetStringAsync(String.Concat("https://localhost:4243/Products/", RootGuid)).Result;
                var RootComponent = JsonConvert.DeserializeObject<Project>(queryJson);



                return View(RootComponent);
            }
        }
    }
}