using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        [HttpGet, Route("qpoint/search/{Query}")]
        public ActionResult UpdateQpointQuery(string Query)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                var result = client.GetAsync("https://localhost:4243/Data/query").Result;
                HttpResponseMessage response = client.GetAsync("QPoint/search/'"+Query+"'").Result;
                List<AIn.Ventures.Models.QPoint> qpoints = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                return View("~/Views/SearchResults.cshtml", qpoints);
            }

        }
    }
}