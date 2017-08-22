using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;



namespace api.ain.ventures.Controllers
{
    public class StakeholdersController : ApiController
    {

        [HttpGet, Route("Stakeholders/{GUID}")]
        //[AllowAnonymous]
        //[Authorize]
        // [AIn.Ventures.BaseLibrary.ActionRightsFilter(RequiredRight = Right.View, SecuredEntity = Entity.Component)]
        public IHttpActionResult GetStakeholder(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            //IEnumerator<GetProjects_ByProjectGUID_Result> projectEnum = ent.GetProjects_ByProjectGUID(GUID).GetEnumerator();
            //AIn.Ventures.Models.Stakeholder stake = new AIn.Ventures.Models.Stakeholder();
            //AIn.Ventures.Models.Project proj = new AIn.Ventures.Models.Project();
            AIn.Ventures.Models.Stakeholder sh = new AIn.Ventures.Models.Stakeholder();
            IEnumerator<GetStakeholder_ByGUID_Result> stakeholderEnum = ent.GetStakeholder_ByGUID(GUID).GetEnumerator();


            if (stakeholderEnum.MoveNext())
            {
                sh.GUID = stakeholderEnum.Current.GUID;
                //sh.IsUser = stakeholderEnum.Current.IsUser;
                sh.Shares = (float)stakeholderEnum.Current.Shares;
                sh.ProjectGUID = (Guid)stakeholderEnum.Current.ProjectGUID;

                /**IEnumerator<GetStakeholders_ByProjectGUID_Result> wnum = ent.GetStakeholders_ByProjectGUID(GUID).GetEnumerator();

                while (wnum.MoveNext())
                {
                    AIn.Ventures.Models.Stakeholder sh = new AIn.Ventures.Models.Stakeholder();
                    sh.GUID = wnum.Current.GUID;
                    sh.IsUser = wnum.Current.IsUser;
                    sh.Shares = (float)wnum.Current.Shares;
                    proj.Stakeholders.Add(sh);
                }**/

            }

            return Ok(sh);
        }
    
  
        [HttpPost, Route("Stakeholders")]
        [Authorize]
        public IHttpActionResult CreateStakeholder([FromBody]AIn.Ventures.Models.Stakeholder stakeholders)
        {
            Guid guid = Guid.NewGuid();

            AInVentureEntities ent = new AInVentureEntities();

            byte[] type = new byte[] {0x20,0x20,0x20};
            Guid u = new Guid(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

            ent.CreateStakeholder(guid,type,100, u);

            return Ok(stakeholders);
        }

        [HttpDelete, Route("Stakeholders/Delete/{GUID}")]
        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.Delete)]
        public IHttpActionResult DeleteStakeholders(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            ent.DeleteStakeholders_ByGUID(GUID);
            ent.SaveChanges();

            return Ok();
        }


        /**[Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Stakeholders/Update/{GUID}")]
        public IHttpActionResult UpdateStakeholders(Guid GUID, AIn.Ventures.Models.Stakeholder stakeholder)
        {
            AInVentureEntities ent = new AInVentureEntities();

            ent.UpdateStakeholders(GUID, stakeholder.Shares,stakeholder.GUID);
            ent.SaveChanges();

            return Ok("OK");
        }**/


    }
}