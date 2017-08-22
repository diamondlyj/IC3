using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Http;
using System.Data.SqlClient;

using AIn.Ventures.BaseLibrary;

namespace api.ain.ventures.Controllers
{
    public class UsersController : ApiController
    {

        [HttpGet, Route("User")]
        [Authorize]
        public IHttpActionResult GetUser()
        {
            AIn.Ventures.Models.User u = new AIn.Ventures.Models.User();

            Guid g = new Guid(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            AInVentureEntities ent = new AInVentureEntities();
            
            IEnumerator<GetUser_ByGUID_Result> inum = ent.GetUser_ByGUID(g).GetEnumerator();

            if (inum.MoveNext())
            {
                u.GUID = inum.Current.GUID;
                u.EmailAddress = inum.Current.EmailAddress;
                u.GivenNames = inum.Current.GivenNames;
                u.Surname = inum.Current.Surname;
            }
            


            var cmd = ent.Database.Connection.CreateCommand();

            cmd.Parameters.Add(new SqlParameter("@UserGUID", g));

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetProjects_ByUserGUID";

            try
            {
                //ent.Database.Connection.Open();

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AIn.Ventures.Models.Project p = new AIn.Ventures.Models.Project()
                    {
                        Name = reader["Name"].ToString(),
                        GUID = (Guid)reader["ProjectGUID"],
                        Description = reader["Description"].ToString(),
                    };

                    u.Projects.Add(p);
                }

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(u);
        }

        /// <summary>
        /// Retrieves immediate social netweork of a user (i.e. Colleages).
        /// </summary>
        /// <param name="GUID">Identifier of user.</param>
        /// <returns></returns>
        [HttpGet, Route("Colleague/{GUID}")]
        //[Authorize]
        //[AIn.Ventures.BaseLibrary.ActionRightsFilter(SecuredEntity = Entity.User, RequiredRight = Right.View)]
        public IHttpActionResult GetColleagues(Guid GUID)//make delete (removecolleagues) and make put (addcolleague)
        {
            AInVentureEntities ent = new AInVentureEntities();
            IEnumerator<GetColleagues_ByUserGUID_Result> inum = ent.GetColleagues_ByUserGUID(GUID).GetEnumerator();
            List<AIn.Ventures.Models.Colleague> colls = new List<AIn.Ventures.Models.Colleague>();
            while (inum.MoveNext())
            {
                AIn.Ventures.Models.Colleague c = new AIn.Ventures.Models.Colleague()
                {
                    GUID = inum.Current.GUID,
                    EmailAddress = inum.Current.EmailAddress,
                    GivenNames = inum.Current.GivenNames,
                    Surname = inum.Current.Surname,
                    Weight = inum.Current.Weight

                };

                colls.Add(c);

            }


            return Ok(colls);

        }

        [HttpDelete, Route("Users/{GUID}/Colleagues/Delete/{ColleagueGUID}")]
        [Authorize]
        //[ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteColleague(Guid GUID, Guid ColleagueGUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteColleague(GUID, ColleagueGUID);
            ent.SaveChanges();

            return Ok("Removed " + ColleagueGUID + " as a colleague of " + GUID + ".");
        }

        [HttpPost, Route("Users/{GUID}/Colleagues/{UserGUID}")]
        //[Authorize]
        public IHttpActionResult CreateColleague(Guid GUID, Guid UserGUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.AddColleague(GUID, UserGUID);
            return Ok("Added " + UserGUID + " as a colleague of " + GUID + ".");
        }

        [HttpGet, Route("User/{GUID}")]
        [AllowAnonymous]
        public IHttpActionResult GetUser(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            AIn.Ventures.Models.User user = new AIn.Ventures.Models.User();
            IEnumerator<GetUser_ByGUID_Result> inum = ent.GetUser_ByGUID(GUID).GetEnumerator();
            IEnumerator<GetProjects_ByUserGUID_Result> projectEnum = ent.GetProjects_ByUserGUID(GUID).GetEnumerator();
            IEnumerator<GetColleagues_ByUserGUID_Result> colleagueEnum = ent.GetColleagues_ByUserGUID(GUID).GetEnumerator();
            if (inum.MoveNext())
            {
                user.GUID = inum.Current.GUID;
                user.EmailAddress = inum.Current.EmailAddress;
                user.GivenNames = inum.Current.GivenNames;
                user.Surname = inum.Current.Surname;
            }
            while (projectEnum.MoveNext())
            {
                AIn.Ventures.Models.Project p = new AIn.Ventures.Models.Project()
                {
                    Name = projectEnum.Current.ProjectName,
                    Description = projectEnum.Current.Description

                };
                user.Projects.Add(p);
            }
            while (colleagueEnum.MoveNext())
            {
                AIn.Ventures.Models.Colleague c = new AIn.Ventures.Models.Colleague()
                {
                    GivenNames = colleagueEnum.Current.GivenNames,
                    GUID = colleagueEnum.Current.GUID,
                    Surname = colleagueEnum.Current.Surname,
                    EmailAddress = colleagueEnum.Current.EmailAddress,
                    Weight = colleagueEnum.Current.Weight

                };
                user.Colleagues.Add(c);
            }
            return Ok(user);
        }
        
        //
    }
}

//Users/(GUID)/Colleauges/(ColleaugeGUID)