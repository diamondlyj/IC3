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
    public class ManageUIController : Controller
    {
        // GET: ManageUI
        public ActionResult Index()
        {
            return View();
        }

        [Route("Test/Manage")]
        public ActionResult Filter()
        {
            //
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                HttpResponseMessage response = client.GetAsync("Data/qpoint").Result;
                HttpResponseMessage cresponse = client.GetAsync("Catalog").Result;
                AIn.Ventures.Models.QpointCategory filter = response.Content.ReadAsAsync<AIn.Ventures.Models.QpointCategory>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.Category menu = cresponse.Content.ReadAsAsync<AIn.Ventures.Models.Category>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.UI.Models.ManageUIModel m = new Models.ManageUIModel();
                m.Categories = filter;
                m.Catalogs = menu;
                return View("~/Views/Manage/AddFilter.cshtml", m);
            }
        }
        [Route("Test/box")]
        public ActionResult Querybox()
        {

            return View("~/Views/Manage/QueryBox.cshtml");
        }

        [Route("Test/qpoint")]
        [AllowAnonymous]
        public ActionResult Qpoint()
        {

            return View("~/Views/Manage/AddNewQpoint.cshtml");
        }

        [HttpGet, Route("Data/query/{GUID}")]
        [AllowAnonymous]
        public ActionResult EditQpoint(Guid GUID)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                HttpResponseMessage response = client.GetAsync("Data/qpoint1/" + GUID).Result;
               // TempData["result"] = response.Content.ReadAsStringAsync().Result;
               AIn.Ventures.Models.QPoint point = response.Content.ReadAsAsync<AIn.Ventures.Models.QPoint>(new[] { new JsonMediaTypeFormatter() }).Result;
                return View("~/Views/Manage/QueryBox.cshtml", point);
            }

        }

        [HttpPost, Route("Data/update")]
        public ActionResult UpdateQpointQuery()
        {
            using (var client = new HttpClient())
            {
                var qp = new AIn.Ventures.Models.Source();
                qp.Query = Request["Query"];
                qp.GUID = Guid.Parse(Request["GUID"]);
                client.BaseAddress = new Uri("https://localhost:4243");
                var result = client.PostAsJsonAsync<AIn.Ventures.Models.Source>("https://localhost:4243/Data/query", qp).Result;
                HttpResponseMessage response = client.GetAsync("Data/qpoint").Result;
                HttpResponseMessage cresponse = client.GetAsync("Catalog").Result;
                AIn.Ventures.Models.QpointCategory filter = response.Content.ReadAsAsync<AIn.Ventures.Models.QpointCategory>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.Category menu = cresponse.Content.ReadAsAsync<AIn.Ventures.Models.Category>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.UI.Models.ManageUIModel m = new Models.ManageUIModel();
                m.Categories = filter;
                m.Catalogs = menu;
                return View("~/Views/Manage/AddFilter.cshtml", m);
            }
           
        }

        [HttpPost, Route("Data/qpoint/new")]
        public ActionResult CreatNewQpoint()
        {
            using (var client = new HttpClient())
            {
                var qp = new AIn.Ventures.Models.QPoint();
                qp.Name = (String)Request["QpointName"];
                qp.GUID = Guid.NewGuid();
                qp.Description = (String)Request["QpointDescription"];
                client.BaseAddress = new Uri("https://localhost:4243");
                var result = client.PostAsJsonAsync<AIn.Ventures.Models.QPoint>("Data/qpoint/new", qp).Result;
                var r = result.Content.ReadAsStringAsync();
                
                HttpResponseMessage response = client.GetAsync("Data/qpoint").Result;
                HttpResponseMessage cresponse = client.GetAsync("Catalog").Result;
                AIn.Ventures.Models.QpointCategory filter = response.Content.ReadAsAsync<AIn.Ventures.Models.QpointCategory>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.Category menu = cresponse.Content.ReadAsAsync<AIn.Ventures.Models.Category>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.UI.Models.ManageUIModel m = new Models.ManageUIModel();
                m.Categories = filter;
                m.Catalogs = menu;
                return View("~/Views/Manage/AddFilter.cshtml", m);
            }
        }
        [HttpGet, Route("Data/label/{GUID}")]
        [AllowAnonymous]
        public ActionResult EditLabel(Guid GUID)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:4243");
                HttpResponseMessage response = client.GetAsync("Data/qpoint1/" + GUID).Result;
                // TempData["result"] = response.Content.ReadAsStringAsync().Result;
                AIn.Ventures.Models.QPoint point = response.Content.ReadAsAsync<AIn.Ventures.Models.QPoint>(new[] { new JsonMediaTypeFormatter() }).Result;
                return View("~/Views/Manage/AddNewLabel.cshtml", point);
            }

        }
        [HttpPost, Route("Data/Label/new")]
        public ActionResult CreatNewLabel()
        {
            using (var client = new HttpClient())
            {
                var qp = new AIn.Ventures.Models.Label();
                qp.Name = (String)Request["LabelName"];
                qp.CategoryGUID = Guid.NewGuid();
                qp.QpointGUID = Guid.Parse(Request["GUID"]);
                client.BaseAddress = new Uri("https://localhost:4243");
                var result = client.PostAsJsonAsync<AIn.Ventures.Models.Label>("Data/qpoint/new", qp).Result;
                var r = result.Content.ReadAsStringAsync();

                HttpResponseMessage response = client.GetAsync("Data/qpoint").Result;
                HttpResponseMessage cresponse = client.GetAsync("Catalog").Result;
                AIn.Ventures.Models.QpointCategory filter = response.Content.ReadAsAsync<AIn.Ventures.Models.QpointCategory>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.Models.Category menu = cresponse.Content.ReadAsAsync<AIn.Ventures.Models.Category>(new[] { new JsonMediaTypeFormatter() }).Result;
                AIn.Ventures.UI.Models.ManageUIModel m = new Models.ManageUIModel();
                m.Categories = filter;
                m.Catalogs = menu;
                return View("~/Views/Manage/AddFilter.cshtml", m);
            }
        }

    }
}