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
    public class PurchasingController : Controller
    {
        [HttpGet, Route("AvailableProducts/{GUID}/{ParentGUID}/AvailableProduct/{Amount}")]
        //[Authorize]
        public ActionResult GetAvailableProducts(Guid GUID, Guid ParentGUID, int Amount)
        {
            // debug - change these when qpoint database is populated
            var RealGUID = GUID; //debug - remove
            GUID = new Guid("c4d4b4ec-ba2b-4af1-a113-2dff66ae03c3"); //debug - remove
            JsonResult res = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            using (HttpClient client = new HttpClient())
            {
                OrderParameters op = new OrderParameters();
                op.Qpoint = new AIn.Ventures.Models.QPoint();
                op.Amount = Amount;
                //op.PriceMin = 4.20;
                //op.PriceMax = 90.01;
                op.SourceName = "ebay";
                List<AvailableProduct> r = new List<AvailableProduct>();
                var response = client.PostAsJsonAsync<OrderParameters>("https://localhost:4243/Component/" + GUID.ToString() + "/Source", op).Result;
                r = response.Content.ReadAsAsync<List<AvailableProduct>>(new[] { new JsonMediaTypeFormatter() }).Result;
                TempData["ObjectGUID"] = RealGUID.ToString(); //debug - change to `GUID.ToString()`
                TempData["ParentGUID"] = ParentGUID.ToString();
                return View("~/Views/Component/_AvailableProduct.cshtml", r);

                //res.Data = r;

                //return res;
            }
        }
    }
}
