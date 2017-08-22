using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace AIn.Ventures.UI.Controllers
{
    public class DataController : ApiController
    {
        // GET: Data
        public IHttpActionResult Index()
        {
            return Ok();
        }
    }
}