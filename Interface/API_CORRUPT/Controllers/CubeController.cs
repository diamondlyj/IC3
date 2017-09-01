using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient;
using AI.V2.BaseLibrary;
using System.Runtime.Serialization;
using System.Data;

namespace API.Controllers
{
    public class CubeController : ApiController
    {
        // GET: Cube
        public IHttpActionResult GetCategoryInfo()
        {
            return Ok();
        }

        public IHttpActionResult GetCube()
        {
            return Ok();
        }
        public IHttpActionResult GetDynamicCube()
        {
            return Ok();
        }

        public IHttpActionResult Search(string tokenValue, string domain, int position, int count, string orderBy, bool matchSubstrings)
        {
            return Ok();

        }
    }
}