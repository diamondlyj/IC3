using AIn.Ventures.BaseLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace api2.ain.ventures.Controllers
{
    /// <summary>
    /// Represents test controller that should be removed.
    /// </summary>
    public class ProjectController : ApiController
    {
        [Authorize]
        [HttpGet, Route("Projects")]
        public IHttpActionResult Get()
        {
            Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            /*
            AInVentureEntities entities = new AInVentureEntities();

            List<Project> projects = new List<Project>();

            List<Project> ps = new List<Project>();

            IEnumerator<GetProjects_Result> enumerator = entities.GetProjects().GetEnumerator();

            while (enumerator.MoveNext())
            {
                //enumerator.Current.
                Project p = new Project();
                p.ProjectName = enumerator.Current.ProjectName;
                ps.Add(p);
            }

            return Ok(ps);
            */

            return Ok(u);
        }

        [HttpGet, Route("Projects/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(id * id);
        }
    }
}
