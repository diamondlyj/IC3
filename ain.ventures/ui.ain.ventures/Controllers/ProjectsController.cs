using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using AIn.Ventures.Models;
namespace AIn.Ventures.UI.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            using (var client = AccountController.CreateClient())
            {
                var response = client.GetAsync("https://localhost:4243/Project").Result;

                List<AIn.Ventures.Models.Project> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.Project>>(new[] { new JsonMediaTypeFormatter() }).Result;

                return View(r);
            }



            ///*
            // * Debug return
            // */

            //List<Project> p = new List<Project>();
            //Project pr = new Project
            //{
            //    GUID = Guid.Parse("820bf26f-4d0b-4630-9cc0-16205ed93759"),
            //    Name = "Project A",
            //    Collaborators = new List<Collaborator>(),
            //    Components = new List<Component>(),
            //    Description = "Lorem Ipsum nunc ut pulrvinar odio"
            //};
            //Component c = getComponent(token, "05c4d90d-ad46-4940-bc5b-0006190ccf56");
            //c.Name = "Product A1";
            //pr.Components.Add(c);
            //c = getComponent(token, "a29253c7-0202-4bee-aeaa-0020d94b1ffa");
            //c.Name = "Product A2";
            //pr.Components.Add(c);
            //c = getComponent(token, "2b2d5a69-f3fc-4456-ba9d-002f5398bd2b");
            //c.Name = "Product A3";
            //pr.Components.Add(c);
            //var col = new Collaborator { GivenNames = "Nelson 'Big Head'", Surname = "Bighetti", GUID = Guid.Parse("f30b8c65-892d-436f-923d-4e0303b3be0c") };
            //pr.Collaborators.Add(col);
            //col = new Collaborator { GivenNames = "Jian", Surname = "Yang", GUID = Guid.Parse("bbaf3622-dc03-4098-bd93-74d73cf8eb15") };
            //pr.Collaborators.Add(col);
            //col = new Collaborator { GivenNames = "Erlich", Surname = "Bachman", GUID = Guid.Parse("5fd9c9dd-e39b-4e0e-8c28-12650eadfd8e") };
            //pr.Collaborators.Add(col);
            //p.Add(pr);
            //pr = new Project
            //{
            //    GUID = Guid.Parse("978bc038-cd0b-46ca-9b8d-2c5236c58358"),
            //    Name = "Project B",
            //    Collaborators = new List<Collaborator>(),
            //    Components = new List<Component>(),
            //    Description = "Dolor sit Quisque imperdiet"
            //};
            //c = getComponent(token, "e327895c-5fb5-4919-8bd1-0154c126c514");
            //pr.Components.Add(c);
            //c.Name = "Product B1";
            //col = new Collaborator { GivenNames = "Richard", Surname = "Hendricks", GUID = Guid.Parse("52bade6f-8494-406b-98a1-6051a8eca757") };
            //pr.Collaborators.Add(col);
            //col = new Collaborator { GivenNames = "Erlich", Surname = "Bachman", GUID = Guid.Parse("5fd9c9dd-e39b-4e0e-8c28-12650eadfd8e") };
            //pr.Collaborators.Add(col);
            //col = new Collaborator { GivenNames = "Dinesh", Surname = "Chugtai", GUID = Guid.Parse("f5dc6adb-7644-4e5f-aaa7-ed47fd4d3885") };
            //pr.Collaborators.Add(col);
            //col = new Collaborator { GivenNames = "Gavin", Surname = "Belson", GUID = Guid.Parse("c3f427a9-bf8e-4417-80e5-6910cbbe1d8d") };
            //pr.Collaborators.Add(col);
            //p.Add(pr);
            //return View(p);
        }

        private Component getComponent(JWToken token, string GUID)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

                var queryJson = client.GetStringAsync(String.Concat("https://localhost:4243/Module/", GUID)).Result;
                var Cmp = JsonConvert.DeserializeObject<Component>(queryJson);
                Cmp.GUID = Guid.Parse(GUID);
                //RootComponent = new Component { GUID = RootGuid, Name = "", AmountInParent = 1, ComponentType = "Product", Description = "", Manufacturer = "", ParentGUID = Guid.NewGuid(), Price = 0, SKU = "", Supplier = "" };

                return Cmp;
            }
            

        }
        private JWToken GetToken()
        {
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "john.doe797@ain.ventures"},
                   {"password", "pwdPwd321!"},
               };
                var tokenResponse = client.PostAsync("https://localhost:4243/Token", new FormUrlEncodedContent(form)).Result;

                return tokenResponse.Content.ReadAsAsync<JWToken>(new[] { new JsonMediaTypeFormatter() }).Result;

            }

        }

    }


}