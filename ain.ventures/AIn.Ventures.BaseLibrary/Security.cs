using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace AIn.Ventures.BaseLibrary
{
    /// <summary>
    /// When added as an attribute to a method with a route, this filter 
    /// ensure that the user has the required authority to perform the actions associated with
    /// the route.
    /// </summary>
 
    public class ActionRightsFilter: ActionFilterAttribute
    {
        public Entity SecuredEntity { get; set; }
        public Right RequiredRight { get; set; }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (RequiredRight != Right.None)
            {
                Guid g = (Guid)actionContext.ActionArguments["GUID"];
                Guid u = new Guid(((ClaimsIdentity)actionContext.RequestContext.Principal.Identity).Claims.FirstOrDefault(x => x.Type == "GUID").Value);

                AInVentureEntities ent = new AInVentureEntities();
                int rights = 0;

                int? r = null;

                switch (SecuredEntity)
                {
                    case Entity.Component:
                        r = ent.Component_GetRights(g, u).FirstOrDefault();
                        break;
                    default:
                        r = ent.Project_GetRights(g, u).FirstOrDefault();
                        break;
                }

                if (r != null) rights = (int)r;

                int b = (int)RequiredRight;

                if ((rights & b) != b)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }
            }

            base.OnActionExecuting(actionContext);
        }

    }

    /// <summary>
    /// Enumertor used to determine how authrity to perform an action should be determined.
    /// </summary>
    public enum Entity : int
    {
        None = 0, 
        Project = 1,
        Component = 2,
        User = 3
    }

    /// <summary>
    /// Rights are determined by usings 32 bits.
    /// 
    /// The meaning of the rights are listed below.
    /// 
    ///     Destroy:    Can permanently destroy instances in the system.
    ///     Delete:     Can remove an entity from the active system and place it in archive.
    ///     Edit:       Can change an entity as represented in the system.
    ///     View:       Is allowed to see an entity.
    ///     Publish:    Can market an entity (including through integrated 3rd party API's).
    ///     Transact:   Can sell and purchase entities (including through integrated 3rd party API's).
    ///     Value:      Can set the price of an entity.
    ///     Divest:     Can create shares and assign and revoke ownership of an entity.
    /// 
    /// </summary>
    [Flags]
    public enum Right: int
    {
        None = 0,
        AssignRights = 1,
        Destroy = 2,
        Delete = 4,
        Edit = 8,
        Comment = 16,
        View = 32,
        Publish = 64,
        Transact = 128,
        Value = 256,
        Divest = 512
    }
}
