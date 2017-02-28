using System.Linq;
using System.Web.Http;
using System.Net.Http;
using FollowMeAPI.Sessions;
using Utility;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("auth")]
        public HttpResponseMessage LogInUser()
        {
            string userName = null;
            string password = null;
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("UserName") && Request.Headers.Contains("Password"))
            {
                userName = Request.Headers.GetValues("UserName").FirstOrDefault();
                password = Request.Headers.GetValues("Password").FirstOrDefault();
            }

            if (userName != null || password != null)
            {
                FollowMeSession session = new FollowMeSession(userName, password, WebApiApplication.db);
                session.CreateUserSession();

                if (session.IsAuthed())
                {

                    if (SessionManager.AddNewSession(session))
                    {
						response.StatusCode = System.Net.HttpStatusCode.OK;
						return response;
                    }
                    else
                    {
						throw new HttpResponseException(System.Net.HttpStatusCode.ExpectationFailed);
					}
                }
                else
                {
					throw new HttpResponseException(System.Net.HttpStatusCode.NonAuthoritativeInformation);

				}
            }
            else
            {
				throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
            }
        }
    }
}
