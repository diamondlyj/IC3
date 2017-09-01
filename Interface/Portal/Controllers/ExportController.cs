using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AI.V2.BaseLibrary;
using Newtonsoft.Json;
using System.Net;

namespace API.Controllers
{
    public class SpreadsheetController : Controller
    {
        /// <summary>
        /// Exports categorical summary of data in csv, pdf or xlsx format
        /// </summary>
        /// <param name="domain">domain of object to search for</param>
        /// <param name="FormatString">"csv", "xlsx" or "pdf"</param>
        /// <returns>pdf, csv or xlsx table with fields "Category" and "Object count"</returns>
        [HttpGet, Route("{domain}/ExportCategorical/{token}/{FormatString}/{position?}/{count?}/{matchSubstrings?}")]
        public ActionResult ExportCategorical(string domain, string token, string FormatString, int position = 0, int count = -1, bool matchSubstrings = false)
        {
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            // Call https://.../Token/Cache (GET) method and get filter in cache.
            var newclient = new HttpClient();
            var newapiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            var newurl = Utilities.MakePath(newapiUrl, "/Token/Cache");
            newclient.DefaultRequestHeaders.Add("Token", token);
            string json = newclient.GetAsync(newurl).Result.Content.ReadAsStringAsync().Result.ToString();
            // result of above call
            //TODO: add PropertyValue later







            AcceptedFormats Format;
            if (!AI.V2.BaseLibrary.AcceptedFormats.TryParse(FormatString, true, out Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + FormatString + " file type");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errors[0]); //debug
                throw new ArgumentException(errors[0]); //debug
                //return View("~/Views/Shared/CustomError.cshtml", errors);
            }
            using (var Client = new HttpClient())
            {
                json = Client.GetAsync(apiUrl + "/Token/Cache").Result.Content.ReadAsStringAsync().Result;
            }

            AI.V2.BaseLibrary.Filter filter = null;

            try
            {

                filter = JsonConvert.DeserializeObject<AI.V2.BaseLibrary.Filter>(json);
            }
            catch (Exception ex)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }



            using (var client = new HttpClient())
            {
                //get object from '/{domain}/Search'
                var url = Utilities.MakePath(apiUrl, domain, "Search", position, count, matchSubstrings);
                string objs = client.PostAsJsonAsync(url, filter).Result.Content.ReadAsStringAsync().Result;

                //throw new Exception("objs is: " + objs); //debug
                if (objs == null)
                {
                    List<string> errors = new List<string>();
                    errors.Add("Object is null");
                    throw new NullReferenceException(errors[0]);
                    //return View("~/Views/Shared/CustomError.cshtml", errors);
                }

                //return Content(objs); //debug
                //don't need to deserialize then serialize again - let objects stay in JSON format
                var response = client.PostAsync(Utilities.MakePath(apiUrl, "ExportCategorical", FormatString), new StringContent(objs, Encoding.UTF8, "application/json"));
                if (response.Result.IsSuccessStatusCode)
                {
                    byte[] filedata = response.Result.Content.ReadAsByteArrayAsync().Result;
                    return File(filedata, DownloadFormatInfo.Formats[Format].ContentType, domain + DownloadFormatInfo.Formats[Format].Extension);

                }
                else
                {
                    var errors = new List<string>();
                    errors.Add(response.Result.Content.ReadAsStringAsync().Result);
                    // TODO: use exeption recording process I created
                    throw new HttpException(errors[0]);
                    //return View("~/Views/Shared/CustomError.cshtml", errors);
                }

            }
        }


        /// <summary>
        /// gets a csv xlsx or pdf file from api for object and returns it
        /// </summary>
        /// <param name="domain">domain of object to search for 
        /// and put results in csv</param>
        /// <returns>csv file of object</returns>
        
        [HttpGet, Route("{domain}/Export/{token}/{FormatString}/{position?}/{count?}/{matchSubstrings?}")]
        public ActionResult Spreadsheet(string domain, String token, string FormatString, int position = 0, int count = -1, bool matchSubstrings = false)
        {
            var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

            // Download token shuld be retrieved from headers.

            // Call https://.../Token/Cache (GET) method and get filter in cache.
                var newclient = new HttpClient();
                var newapiUrl = ConfigurationManager.AppSettings["ApiUrl"];
                var newurl = Utilities.MakePath(newapiUrl,"/Token/Cache");
                newclient.DefaultRequestHeaders.Add("Token", token);
                string json = newclient.GetAsync(newurl).Result.Content.ReadAsStringAsync().Result.ToString();
            // result of above call
            //TODO: add PropertyValue later

     



            AI.V2.BaseLibrary.Filter filter = null;

            try
            {

                filter = JsonConvert.DeserializeObject<AI.V2.BaseLibrary.Filter>(json);
            }
            catch (Exception ex)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }


            AcceptedFormats Format;
            if (!AI.V2.BaseLibrary.AcceptedFormats.TryParse(FormatString, true, out Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + FormatString + " file type");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errors[0]); //debug
                throw new ArgumentException(errors[0]); //debug
                //return View("~/Views/Shared/CustomError.cshtml", errors);
            }



            using (var client = new HttpClient())
            {
                //get object from '/{domain}/Search'

            

                var url = Utilities.MakePath(apiUrl, domain, "Search", position, count, matchSubstrings);
                string objs = client.PostAsJsonAsync(url, filter).Result.Content.ReadAsStringAsync().Result;

                //throw new Exception("objs is: " + objs); //debug
                if (objs == null)
                {
                    List<string> errors = new List<string>();
                    errors.Add("Object is null");
                    throw new NullReferenceException(errors[0]);
                    //return View("~/Views/Shared/CustomError.cshtml", errors);
                }

                //return Content(objs); //debug
                //don't need to deserialize then serialize again - let objects stay in JSON format
                var response = client.PostAsync(Utilities.MakePath(apiUrl, "Export", FormatString), new StringContent(objs, Encoding.UTF8, "application/json"));
                if (response.Result.IsSuccessStatusCode)
                {
                    byte[] filedata = response.Result.Content.ReadAsByteArrayAsync().Result;

                    return File(filedata, DownloadFormatInfo.Formats[Format].ContentType, domain + DownloadFormatInfo.Formats[Format].Extension);

                }
                else
                {
                    var errors = new List<string>();
                    errors.Add(response.Result.Content.ReadAsStringAsync().Result);
                    // TODO: use exeption recording process I created
                    throw new HttpException(errors[0]);
                    //return View("~/Views/Shared/CustomError.cshtml", errors);
                }
            }
        }



    }
}