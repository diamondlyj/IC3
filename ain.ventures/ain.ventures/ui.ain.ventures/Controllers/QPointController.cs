using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;

namespace AIn.Ventures.UI.Controllers
{
    public class QPointController : Controller
    {
        [HttpPost, Route("QPoint")]
        //[Authorize]
        public ActionResult FindQPoints(List<string> Categories)
        {

            var sparams = new AIn.Ventures.Models.SearchParameters();
            sparams.Categories = Categories;
            JsonResult res = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            /*Debug*/
            sparams.Categories = new List<string>();
            sparams.Categories.Add("Electronic Component.Integrated");
            /*Debug*/

            using (var client = new HttpClient())
            {
            
                /*
                List<string> cats = new List<string>();
                cats.Add("Electronic Component");
                cats.Add("Material.Element.Silicon");
                */

                var response = client.PostAsJsonAsync<AIn.Ventures.Models.SearchParameters>("https://localhost:4243/QPoint", sparams).Result;

                List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                //string r = response.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;

                res.Data = r;

                return res;
            }
        }
        }


    }
