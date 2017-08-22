using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;

using System.Data.SqlClient;

namespace api.ain.ventures.Controllers
{
    public class ProjectsController : ApiController
    {
        [HttpPost, Route("Project")]
        [Authorize]
        public IHttpActionResult CreateProject([FromBody]AIn.Ventures.Models.Project project)
        {
            Guid guid = Guid.NewGuid();

            AInVentureEntities ent = new AInVentureEntities();

            Guid u = new Guid(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            ent.CreateProject(guid, project.Name, project.Description, 100, u);

            return Ok(guid);
        }

        [HttpDelete, Route("Project/{GUID}")]
        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteProject(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteProject(GUID);
            ent.SaveChanges();

            return Ok();
        }


        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Project/{GUID}")]
        public IHttpActionResult UpdateProject(Guid GUID, AIn.Ventures.Models.Project project)
        {
            AInVentureEntities ent = new AInVentureEntities();

            ent.UpdateProject(GUID, project.Name, project.Description);
            ent.SaveChanges();

            return Ok("OK");
        }

        [HttpGet, Route("Project/{GUID}")]
        [Authorize]
        [AIn.Ventures.BaseLibrary.ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        public IHttpActionResult GetProject(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
                        
            IEnumerator<GetProject_Result> ienumerator = ent.GetProject(GUID).GetEnumerator();

            AIn.Ventures.Models.Project proj = new AIn.Ventures.Models.Project();

           
            if (ienumerator.MoveNext())
            {
                proj.GUID = ienumerator.Current.ProjectGUID;
                proj.Name = ienumerator.Current.ProjectName;
                proj.Description = (string)ienumerator.Current.Description;
                proj.Shares = ienumerator.Current.Shares; ;

                IEnumerator<GetUsers_ByProjectGUID_Result> unum = ent.GetUsers_ByProjectGUID(GUID).GetEnumerator();

                while (unum.MoveNext())
                {
                    AIn.Ventures.Models.Collaborator coll = new AIn.Ventures.Models.Collaborator();
                    coll.GUID = unum.Current.GUID;
                    coll.EmailAddress = unum.Current.EmailAddress;
                    coll.Surname = unum.Current.Surname;
                    coll.GivenNames = unum.Current.GivenNames;
                    coll.Rights = (Right)unum.Current.Role;

                    proj.Collaborators.Add(coll);

                }

                IEnumerator<GetStakeholders_ByProjectGUID_Result> wnum = ent.GetStakeholders_ByProjectGUID(GUID).GetEnumerator();

                while (wnum.MoveNext())
                {
                    AIn.Ventures.Models.Stakeholder sh = new AIn.Ventures.Models.Stakeholder();
                    sh.GUID = wnum.Current.GUID;
                    sh.IsUser = wnum.Current.IsUser;
                    sh.Shares = (int)wnum.Current.Shares;
                    proj.Stakeholders.Add(sh);
                }
                
            }
            
            return Ok(proj);
        }

        [HttpGet, Route("Project")]
        [Authorize]

        public IHttpActionResult GetProjects()
        {
            Guid u = new Guid(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            List<AIn.Ventures.Models.Project> projects = new List<AIn.Ventures.Models.Project>();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetProjects_ByUserGUID";
            cmd.Parameters.Add(new SqlParameter("@UserGUID", u));

            try
            {
                ent.Database.Connection.Open();
                System.Data.Common.DbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AIn.Ventures.Models.Project p = new AIn.Ventures.Models.Project()
                    {
                        Name = reader["Name"].ToString(),
                        GUID = (Guid)reader["ProjectGUID"],
                        Shares = (double)reader["Shares"],
                        Description = reader["Description"].ToString()
                    };

                    projects.Add(p);
                }


                if (reader.NextResult())
                {
                    IEnumerator<AIn.Ventures.Models.Project> num = projects.GetEnumerator();
                    if (num.MoveNext())
                    {

                        Guid pguid = new Guid();

                        while (reader.Read())
                        {
                            pguid = (Guid)reader["ProjectGUID"];

                            while (pguid != num.Current.GUID && num.MoveNext())
                            {
                                //Moves to project that user should be added to.
                            }

                            AIn.Ventures.Models.Collaborator c = new AIn.Ventures.Models.Collaborator()
                            {
                                GivenNames = reader["GivenNames"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString(),
                                Rights = (Right)reader["Role"],
                                GUID = (Guid)reader["GUID"]
                            };

                            num.Current.Collaborators.Add(c);
                        }
                    }
                }

                if (reader.NextResult())
                {
                    IEnumerator<AIn.Ventures.Models.Project> num = projects.GetEnumerator();
                    if (num.MoveNext())
                    {

                        Guid pguid = new Guid();

                        while (reader.Read())
                        {
                            pguid = (Guid)reader["ProjectGUID"];

                            while (pguid != num.Current.GUID && num.MoveNext())
                            {
                                //Moves to project that user should be added to.
                            }

                            AIn.Ventures.Models.Component prod = new AIn.Ventures.Models.Component()
                            {
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                GUID = (Guid)reader["ObjectGUID"]
                            };

                            num.Current.Components.Add(prod);
                        }
                    }
                }

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(projects);
        }

    }
}

