using FollowMeAPI.Sessions;
using System.Collections.Generic;
using System.Web.Http;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("dev")]
    public class DevController : ApiController
    {
        [HttpPost]
        [Route("flush")]
        public void FlushTokens()
        {
            SessionManager.FlushSessions();
        }

        [HttpGet]
        [Route("list-tokens")]
        public List<string> ListTokens()
        {
            return SessionManager.GetActiveTokens();
        }

		[HttpGet]
		[Route("version")]
		public string GetVerstion()
		{
			return WebApiConfig.Version;
		}
    }
}
