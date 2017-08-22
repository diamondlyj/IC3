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
    public class StakeholdersController : Controller
    {
        // GET: Stakeholders
        public ActionResult Index(Guid GUID) //Project GUID
        {
            Guid RootGuid = GUID;
            JWToken token = new JWToken();
            Project RootProject;
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
                client.BaseAddress = new Uri("https://localhost:4243");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

                var queryJson = client.GetStringAsync(String.Concat("https://localhost:4243/Stakeholders/", RootGuid)).Result;
                RootProject = JsonConvert.DeserializeObject<Project>(queryJson);
            }
            
            return View(RootProject);
        }

    }
}