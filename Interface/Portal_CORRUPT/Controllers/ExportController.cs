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

namespace Portal.Controllers
{
    public class SpreadsheetController : Controller
    {
        private class DownloadFormatInfo
        {
            public string ContentType { get; }
            public string Extension { get; }
            public DownloadFormatInfo(string ContentType, string Extension)
            {
                this.ContentType = ContentType;
                this.Extension = Extension;
            }
        }

        /// <summary>
        /// gets a csv file from api for object and returns it
        /// </summary>
        /// <param name="domain">domain of object to search for 
        /// and put results in csv</param>
        /// <returns>csv file of object</returns>
        [HttpGet, Route("{domain}/Export/{Format?}")]
        public ActionResult Spreadsheet(string domain, string Format = "xlsx") 
        {
            //[tk] replace local DownloadFormatInfo with one from baselibrary
            Dictionary<string, DownloadFormatInfo> acceptedFormats = new Dictionary<string, DownloadFormatInfo>(); ;
            acceptedFormats.Add("csv", new DownloadFormatInfo("text/csv", ".csv"));
            acceptedFormats.Add("xlsx", new DownloadFormatInfo("applications/ms-excel", ".xlsx"));
            acceptedFormats.Add("pdf", new DownloadFormatInfo("application/pdf", ".pdf"));
            if (!acceptedFormats.Keys.Contains(Format))
            {
                var errors = new List<string>();
                errors.Add("Cannot currently export data in " + Format + " file type");
                return View("~/Views/Shared/CustomError.cshtml", errors);
            }
                
            using (var client = new HttpClient()) {
                //get object from '/{domain}/Search'
                var apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
                // TODO: replace with filtered search
                var url = string.Concat(apiUrl, domain, "/Search");
                string objs = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                if (objs == null)
                {
                    List<string> errors = new List<string>();
                    errors.Add ( "Object is null");
                    return View("~/Views/Shared/CustomError.cshtml", errors);
                }
                //return Content(objs); //debug
                //don't need to deserialize then serialize again - let objects stay in JSON format
                var response = client.PostAsync(string.Concat(apiUrl, "Export/", Format), new StringContent(objs, Encoding.UTF8, "application/json"));
                if (response.Result.IsSuccessStatusCode)
                {
                    byte[] filedata = response.Result.Content.ReadAsByteArrayAsync().Result;
                    return File(filedata, acceptedFormats[Format].ContentType, domain + acceptedFormats[Format].Extension);

                }
                else
                {
                    var errors = new List<string>();
                    errors.Add(response.Result.Content.ReadAsStringAsync().Result);
                    // TODO: use exeption recording process I created
                    return View("~/Views/Shared/CustomError.cshtml", errors);
                }
            }
        }
    }
}