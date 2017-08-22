using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Web.Http;

using AIn.Ventures;
using AIn.Ventures.BaseLibrary;
using AIn.Ventures.Models;
using Newtonsoft.Json;

namespace api.ain.ventures.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class TestController : ApiController
    {
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

        private HttpClient CreateClient()
        {
            JWToken token = GetToken();

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:4243");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

            return client;
        }


        [HttpGet, Route("Test/Project")]
        public IHttpActionResult GetProject()
        {

            JWToken token = GetToken();

            using (var client = CreateClient())
            {
                HttpResponseMessage response = client.GetAsync("/Projects/820bf26f-4d0b-4630-9cc0-16205ed93759").Result;
                AIn.Ventures.Models.Project p = response.Content.ReadAsAsync<AIn.Ventures.Models.Project>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(p);
            }
        }

        [HttpGet, Route("Test/Projects/Create")]
        public IHttpActionResult CreateProject()
        {

            using (var client = CreateClient())
            {
                Random rand = new Random();
                int n = rand.Next();

                var form = new Dictionary<string, string>
                {
                    {"Name", "Test Project " +  n.ToString()},
                    {"Description", "Some description for " + n.ToString() + "."}
                };

                var response = client.PostAsync("https://localhost:4243/Projects", new FormUrlEncodedContent(form)).Result;

                Guid g = response.Content.ReadAsAsync<Guid>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(g);
            }
        }

        [HttpGet, Route("Test/Projects/{GUID}/Delete")]
        public IHttpActionResult UpdateProject(Guid GUID)
        {

            using (var client = CreateClient())
            {
                var response = client.DeleteAsync("https://localhost:4243/Projects/" + GUID.ToString());

                string r = response.Result.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }
        }

        [HttpGet, Route("Test/Projects/Update")]
        public IHttpActionResult UpdateProject()
        {

            using (var client = CreateClient())
            {
                AIn.Ventures.Models.Project p = new AIn.Ventures.Models.Project();
                p.Name = "Capital 2";
                p.Description = "Das Kapital by Karl Marx revisited.";
                p.GUID = new Guid("b3344b98-1d96-4e8b-aee0-e8907da83e8a");

                var response = client.PutAsJsonAsync<AIn.Ventures.Models.Project>("https://localhost:4243/Projects/b3344b98-1d96-4e8b-aee0-e8907da83e8a", p);

                string r = response.Result.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }
        }

        [HttpGet, Route("Test/Pwd")]
        public IHttpActionResult ChangePwd()
        {

            using (var client = CreateClient())
            {

                Random rand = new Random();
                int n = rand.Next();

                var form = new Dictionary<string, string>
                {
                    {"OldPwd", "pwdPwd311!"},
                    {"NewPwd", "pwdPwd321!"}
                };

                var response = client.PostAsync("https://localhost:4243/Users/9A8737D7-95F8-4EE8-B98B-3A72A8D907C9/Pwd", new FormUrlEncodedContent(form)).Result;

                //return Ok();

                bool b = response.Content.ReadAsAsync<bool>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(b);
            }
        }


        [HttpGet, Route("Things/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }

        [HttpGet, Route("Test/QPoint")]
        //[Authorize]
        public IHttpActionResult TestQPoint()
        {
            using (var client = CreateClient())
            {
                SearchParameters p = new SearchParameters();
                p.Categories.Add("Electronic Component");

                p.Properties.Add("Ohm", "100");

                var response = client.PostAsJsonAsync<SearchParameters>("https://localhost:4243/QPoint", p).Result;

                List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                //string r = response.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }

        }

        [HttpGet, Route("Test/Order/{GUID}/{Supplier}")]
        //[Authorize]
        public IHttpActionResult TestOrder(Guid GUID, string Supplier)
        {
            using (var client = CreateClient())
            {
                OrderParameters op = new OrderParameters();
                //op.Categories.Add(new KeyValuePair<string, string>("boom", "thingy"));
                op.Qpoint = new AIn.Ventures.Models.QPoint();
                op.Amount = 42;
                op.PriceMin = 4.20;
                op.PriceMax = 90.01;
                op.SourceName = Supplier;

                var result = client.PostAsJsonAsync<OrderParameters>("https://localhost:4243/Component/" + GUID.ToString() + "/Source", op).Result;

                //List<AvailableProduct> r = response.Content.ReadAsAsync<List<AvailableProduct>>(new[] { new JsonMediaTypeFormatter() }).Result;
                var r = result.Content.ReadAsStringAsync();

                return Ok(r);

                //return Ok(response.StatusCode);
            }
        }

        [HttpGet, Route("Test/Module/New")]
        //[Authorize]
        public IHttpActionResult CreateModule()
        {
            using (var client = CreateClient())
            {
                var ParGUID = new Guid("5ffc9801-89ea-4edf-b2fd-008c0e3bbe7d");
                var newModule = new AIn.Ventures.Models.Component();
                newModule.ParentGUID = ParGUID;
                newModule.ProjectGUID = new Guid("5ffc9801-89ea-4edf-b2fd-008c0e3bbe7d");
                newModule.GUID = Guid.NewGuid();
                newModule.Description = "Test description";
                newModule.Name = "Test name";
                newModule.Price = 0;
                newModule.SKU = "";
                newModule.Supplier = "";
                newModule.Manufacturer = "";

                var result = client.PostAsJsonAsync<AIn.Ventures.Models.Component>("https://localhost:4243/Module", newModule).Result;

                var r = result.Content.ReadAsStringAsync();

                return Ok(r);
            }
        }

        
        [HttpGet, Route("Test/CreateFromQPoint")]
        //[Authorize]
        public IHttpActionResult TestCreateFromQPoint()
        {
            using (var client = CreateClient())
            {
                var response = client.PutAsync("https://localhost:4243/Component/05c4d90d-ad46-4940-bc5b-0006190ccf56/C4D4B4EC-BA2B-4AF1-A113-2DFF66AE03C3/1", new StringContent("")).Result;

                //List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                string r = response.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }

        }

        [HttpGet, Route("Test/AssignSource")]
        //[Authorize]
        public IHttpActionResult TestAssignSource()
        {
            using (var client = CreateClient())
            {
                var response = client.PostAsync("https://localhost:4243/Component/05c4d90d-ad46-4940-bc5b-0006190ccf56/C4D4B4EC-BA2B-4AF1-A113-2DFF66AE03C3/Ebay/someitemidX1234Y", new StringContent("")).Result;

                //List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.SourceProduct r = response.Content.ReadAsAsync<AIn.Ventures.Models.SourceProduct>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }

        }

        [HttpGet, Route("Test/User")]
        //[Authorize]
        public IHttpActionResult GetUser()
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("https://localhost:4243/User").Result;

                //List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.User r = response.Content.ReadAsAsync<AIn.Ventures.Models.User>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }

        }

        [HttpGet, Route("Test/Projects")]
        //[Authorize]
        public IHttpActionResult GetProjects()
        {

            using (var client = CreateClient())
            {
                var response = client.GetAsync("https://localhost:4243/Project").Result;

                List<AIn.Ventures.Models.Project> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.Project>>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(r);
            }

        }

        [HttpGet, Route("Test/Component/New")]
        //[Authorize]
        public IHttpActionResult CreateComponent()
        {
            using (var client = CreateClient())
            {
                var ParGUID = new Guid("27acbb7c-075a-49bd-9b43-000eae3e6b6f");
                var newComponent = new AIn.Ventures.Models.Component();
                newComponent.ParentGUID = ParGUID;
                newComponent.ProjectGUID = new Guid("820BF26F-4D0B-4630-9CC0-16205ED93759");
                newComponent.GUID = Guid.NewGuid();
                newComponent.Description = "Test description";
                newComponent.Name = "Test name";
                newComponent.Price = 0;
                newComponent.SKU = "";
                newComponent.Supplier = "";
                newComponent.Manufacturer = "";


                var result = client.PostAsJsonAsync<AIn.Ventures.Models.Component>("https://localhost:4243//{AmountInParent}", newComponent).Result;

                var r = result.Content.ReadAsStringAsync();

                return Ok(r);
            }
        }

    }
}
