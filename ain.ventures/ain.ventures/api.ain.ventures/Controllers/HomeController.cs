using System.Web.Http;

namespace api.ain.ventures.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("")]
        public string Index()
        {
            return "Yes! Web API Started successfully";
        }
    }
}