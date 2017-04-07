using System.Web.Http;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("dev")]
    public class DevController : ApiController
    {
		[HttpGet]
		[Route("version")]
		public string GetVerstion()
		{
			return WebApiConfig.Version;
		}
    }
}
