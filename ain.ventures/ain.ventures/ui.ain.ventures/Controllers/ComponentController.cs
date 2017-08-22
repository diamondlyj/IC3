using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using AIn.Ventures.Models;
using System.Diagnostics;

namespace AIn.Ventures.UI.Controllers
{
    public class ComponentController : Controller
    {
        Component RootComponent;

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


        [HttpPost, Route("Module")]
        //[Authorize]
        public ActionResult CreateModule()
        {
            var cp = new Component();
            cp.ComponentType = "Module";
            cp.Name = Request["Name"];
            cp.Description = Request["Description"];
            cp.GUID = Guid.NewGuid();
            cp.ParentGUID = Guid.Parse(Request["ParentGUID"]);
            cp.AmountInParent = 1;
            cp.ProjectGUID = cp.ParentGUID;
            using (var client = CreateClient())
            {
                /*
                var ParGUID = new Guid("27acbb7c-075a-49bd-9b43-000eae3e6b6f");
                var newModule = new AIn.Ventures.Models.Component();
                newModule.ParentGUID = ParGUID;
                newModule.ProjectGUID = new Guid("820BF26F-4D0B-4630-9CC0-16205ED93759");
                newModule.GUID = Guid.NewGuid();
                newModule.Description = "Test description";
                newModule.Name = "Test name";
                newModule.Price = 0;
                newModule.SKU = "";
                newModule.Supplier = "";
                newModule.Manufacturer = "";
                */

                var result = client.PostAsJsonAsync<AIn.Ventures.Models.Component>("https://localhost:4243/Module", cp).Result;

                var r = result.Content.ReadAsStringAsync();

                return RedirectToAction(cp.ParentGUID.ToString(), "Module");
            }
        }
        [HttpPost, Route("Component/{ParentGUID}/{GUID}/{Amount}")]
        //[Authorize]
        public ActionResult CreateComponent(Guid ParentGUID,Guid GUID,int Amount)
        {
            var sparams = new AIn.Ventures.Models.QPoint();
            
            sparams.GUID = GUID;
            //sparams.Name = Name;

         //  sparams.AmountInParent = Amount;
            
            JsonResult res = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
           
            using (var client = new HttpClient())
            {

               var response = client.PostAsync("https://localhost:4243/Component/"+ParentGUID.ToString()+"/"+GUID.ToString()+"/"+Amount.ToString(),new StringContent("")).Result;

             
                //res.Data = "Hello";
               //String r = response.Content.ReadAsAsync<string>(new[] { new JsonMediaTypeFormatter() }).Result;
                res.Data = response.StatusCode.ToString();

                return RedirectToAction(ParentGUID.ToString(), "Module");
            }
        }



        [HttpGet, Route("Module/{GUID}")]
        public ActionResult Index(Guid GUID)
        {



            //Guid RootGuid = new Guid("820bf26f-4d0b-4630-9cc0-16205ed93759");
            /*
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
            */
            
            using (var client = AccountController.CreateClient())
            {
                var queryJson = client.GetStringAsync(String.Concat("https://localhost:4243/Module/", GUID)).Result;
                var Cmp = JsonConvert.DeserializeObject<Component>(queryJson);
                Cmp.GUID = GUID;
                RootComponent = Cmp;
            }
            /*
             * Debug return
             */
            //Component p = new Component
            //{
            //    GUID = Guid.NewGuid(),
            //    AmountInParent = 1,
            //    Children = new List<Component>
            //    {
            //        new Component
            //        {
            //            ComponentType = "Module",
            //            Description = "This goes in the other thing",
            //            Name = "spit",
            //            Manufacturer = "",
            //            MaxPrice = 100,
            //            MinPrice = 20,
            //            ParentGUID = Guid.NewGuid(),
            //            Price = 0,
            //            QPoints = new List<QPoint>(),
            //            SKU = "",
            //            SourceGUID = Guid.NewGuid(),
            //            Supplier = ""

            //        },
            //        new Component
            //        {
            //            ComponentType = "Module",
            //            Description = "This goes in the other thing",
            //            Name = "bubble gum",
            //            Manufacturer = "",
            //            MaxPrice = 100,
            //            MinPrice = 20,
            //            ParentGUID = Guid.NewGuid(),
            //            Price = 0,
            //            QPoints = new List<QPoint>(),
            //            SKU = "",
            //            SourceGUID = Guid.NewGuid(),
            //            Supplier = ""

            //        }


            //    },
            //    ComponentType = "Product",
            //    Description = "This thing rocks!",
            //    Name = "PU235 Immodium Space Modulator",
            //    Manufacturer = "",
            //    MaxPrice = 100,
            //    MinPrice = 20,
            //    ParentGUID = Guid.NewGuid(),
            //    Price = 0,
            //    QPoints = new List<QPoint>(),
            //    SKU = "",
            //    SourceGUID = Guid.NewGuid(),
            //    Supplier = ""
            //};
            //return View(p);

            return View("~/Views/Component/Index.cshtml", RootComponent);
        }

        //Get: Catalog
        [ChildActionOnly]
        [ActionName("_Category")]
        public ActionResult _Catalog()
        {
            var ctlg = new Catalog();
            List<Category> cats;
            JWToken token = new JWToken();
            // Don't need category because right now we just get the whole catalog
            // var CategoryName = Request["CategoryDropdown"];
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

                var queryJson = client.GetStringAsync("https://localhost:4243/Catalog/").Result;
                ctlg = JsonConvert.DeserializeObject<Catalog>(queryJson);
                cats = ctlg.Categories;

            }
            return PartialView(cats);

        }


        // Post: QPoint
        //[ChildActionOnly]
        [ActionName("_QPoint")]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult _QPoint()
        {
            JWToken token = new JWToken();
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
                if (Request.Form.HasKeys())
                {
                    var CategoryString = Request.Form["CategoryListTextArea"];
                    var ParentGUID = Request.Form["ParentGUID"];
                    if (ParentGUID != null)
                    {
                        TempData["ParentGUID"] = ParentGUID;
                    }
                    var sparams = new AIn.Ventures.Models.SearchParameters();
                    if (CategoryString.Length > 0)
                    {
                        sparams.Categories = new List<string>(CategoryString.Split("\r\n".ToCharArray()));
                        sparams.Categories.RemoveAll(isEmpty);
                        var response = client.PostAsJsonAsync<AIn.Ventures.Models.SearchParameters>("https://localhost:4243/QPoint", sparams).Result;

                        List<AIn.Ventures.Models.QPoint> r = response.Content.ReadAsAsync<List<AIn.Ventures.Models.QPoint>>(new[] { new JsonMediaTypeFormatter() }).Result;
                        //adds the existing qpoints to the TempData and returns them if they're there
                        if (TempData["ExistingQueries"] == null) //|| !TempData["ExistingQueries"].GetType().Equals(new List<AIn.Ventures.Models.QPoint>()))
                        {
                            TempData["ExistingQueries"] = r;
                        }
                        else
                        {
                            List<AIn.Ventures.Models.QPoint> eq = (List<AIn.Ventures.Models.QPoint>)TempData["ExistingQueries"];
                            foreach (QPoint qp in eq)
                            {
                                if (!r.Contains(qp))
                                {
                                    r.Add(qp);
                                }
                            }
                            TempData["ExistingQueries"] = r;
                        }


                        try
                        {
                            return PartialView(r);
                        }
                        catch (Exception e)
                        {
                            Debug.Print(e.Message);
                            Debug.Print(e.InnerException.Message);
                        }
                    }
                    TempData["ExistingQueries"] = null;
                }
                TempData["ExistingQueries"] = null;
                try
                {
                    return PartialView();
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                    Debug.Print(e.InnerException.Message);
                }
                return View();
            }
        }
        /// <summary>
        ///     Gets the shopping basket with the correct item in it.
        ///     Only for POST when the item is first added.
        /// </summary>
        /// <params>
        ///     GUID = ParentGUID 
        ///     ObjectGUID = ObjectGUID of Component
        ///     SourceName = eBay, Amazon, etc.
        ///     OrderID from Component = ItemKey of AvailableProduct
        /// </params>
        /// <returns>
        ///     _Commerce partial with availableItem model
        /// </returns> 
        /// _Commerce/{GUID}/{ObjectGUID}/{SourceName}/{OrderID}
        //[ChildActionOnly]
        [ActionName("_Commerce")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult _Commerce()// string ParentGUID, string ObjectGUID, string SourceName, string OrderID)
        {
            //return PartialView(new SourceProduct());
            JWToken token = new JWToken();
            string ParentGUID = Request["ParentGUID"];
            string ObjectGUID = Request["ObjectGUID"];
            string SourceName = Request["SourceName"];
            string OrderID = Request["OrderID"];
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
                var query = client.PostAsync(String.Concat("https://localhost:4243/Component/", ParentGUID, "/", ObjectGUID, "/", SourceName, "/", OrderID),null).Result;
                //[tk] have to do another query to get the actual item because above method doesn't return anything
                var product = query.Content.ReadAsAsync<SourceProduct>(new[] { new JsonMediaTypeFormatter() }).Result;
                //string[] debug = { "ItemKey = " + product.ItemKey };
                //System.IO.File.WriteAllLines(@"C:\Users\Jim Stewart\Desktop\debug.txt", debug);
                return PartialView(product);

            }


        }

        [ActionName("_GetProduct")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult GetProduct(string ParentGUID, string ObjectGUID)
        {
            JWToken token = new JWToken();
            var prod = new SourceProduct();
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
                var query = client.GetAsync(String.Concat("https://localhost:4243/Component/", ParentGUID, "/", ObjectGUID, "/Source")).Result;
                //[tk] have to do another query to get the actual item because above method doesn't return anything
                var product = query.Content.ReadAsAsync<SourceProduct>(new[] { new JsonMediaTypeFormatter() }).Result;
                //string[] debug = { "ItemKey = " + product.ItemKey };
                //System.IO.File.WriteAllLines(@"C:\Users\Jim Stewart\Desktop\debug.txt", debug);
                return PartialView("~/Views/Component/_Commerce.cshtml",product);

            }

        }


        private bool isEmpty(string s)
        {
            return s == "";
        }

    }


}

    