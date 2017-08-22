using AIn.Ventures.BaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class TestController : Controller
    {

        [Route("Test/Login")]
        public ActionResult Login()
        {
            string s = "Not authenticated";

            Token token = new Token();

            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "john.doe797@ain.ventures"},
                   {"password", "pwdPwd321!"},
               };
                var tokenResponse = client.PostAsync("https://localhost:4243/Token", new FormUrlEncodedContent(form)).Result;

                token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                if( tokenResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    s = "Successfully autheticated.";
                    Session["Token"] = token;
                }
                else
                {
                    s = tokenResponse.ReasonPhrase;
                }
            }

            return View((object)s);

            //clientid:866984666766-0k8oal9sarvqe0c9q17consrephdsibk.apps.googleusercontent.com
            //clientsecret:jQxcITo9TQ5KZ4NRZHsy-bcp

        }

        [Route("Test/Secure")]
        public ActionResult Secure()
        {
            BaseLibrary.Component component = new BaseLibrary.Component();

            JWToken token = (JWToken)Session["Token"];

            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri("https://localhost:4243");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

                //make request
                HttpResponseMessage response = client.GetAsync("Modules").Result;

                component = response.Content.ReadAsAsync<BaseLibrary.Component>(new[] { new JsonMediaTypeFormatter() }).Result;
            }

            component.Name = token.AccessToken;

            return View(component);
        }

        [Route("Test/Catalog")]
        public ActionResult Catalog()
        {
            //
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                HttpResponseMessage response = client.GetAsync("Catalog").Result;

                AIn.Ventures.Models.Catalog catalog = response.Content.ReadAsAsync<AIn.Ventures.Models.Catalog>(new[] { new JsonMediaTypeFormatter() }).Result;

                return View("~/Views/Test/Catalog.cshtml",catalog);
            }            
        }

        [Route("Test/D3")]
        public ActionResult D3()
        {
            AIn.Ventures.Models.Category topCat = new AIn.Ventures.Models.Category("top");
            AIn.Ventures.Models.Category lorem = new AIn.Ventures.Models.Category("Lorem");
            topCat.Categories.Add(lorem);
            AIn.Ventures.Models.Category ipsum = new AIn.Ventures.Models.Category("Ipsum");
            topCat.Categories.Add(ipsum);
            AIn.Ventures.Models.Category dolorsit = new AIn.Ventures.Models.Category("Dolor Sit");
            lorem.Categories.Add(dolorsit);
            AIn.Ventures.Models.Category amet = new AIn.Ventures.Models.Category("Amet");
            lorem.Categories.Add(amet);
            AIn.Ventures.Models.Category consectitur = new AIn.Ventures.Models.Category("Consectitur");
            lorem.Categories.Add(consectitur);
            AIn.Ventures.Models.Category adipiscing = new AIn.Ventures.Models.Category("Adipiscing");
            ipsum.Categories.Add(adipiscing);
            AIn.Ventures.Models.Category elit = new AIn.Ventures.Models.Category("Elit Curabitur");
            ipsum.Categories.Add(elit);
            AIn.Ventures.Models.Category rhoncus = new AIn.Ventures.Models.Category("Rhoncus quam");
            ipsum.Categories.Add(rhoncus);
            AIn.Ventures.Models.Category inluctus = new AIn.Ventures.Models.Category("In Luctus");
            elit.Categories.Add(inluctus);
            AIn.Ventures.Models.Category viverra = new AIn.Ventures.Models.Category("Viverra Etiam");
            elit.Categories.Add(viverra);
            AIn.Ventures.Models.Category quis = new AIn.Ventures.Models.Category("quis consequat");
            elit.Categories.Add(quis);


            return View(topCat);
        }
    }

    
}
