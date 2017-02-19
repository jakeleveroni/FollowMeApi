using System.Linq;
using System.Web.Http;
using FollowMeAPI.Sessions;
using Utility;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("auth")]
        public FolloMeResponse LogInUser()
        {
            string userName = null;
            string password = null;

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
                        return new FolloMeResponse(200, FolloMeErrorCodes.LogInSucceded, "Successfully logged in to the API and gained temporary AWS Credentials");
                    }
                    else
                    {
                        SessionManager.RemoveSession(session);
                        return new FolloMeResponse(404, session.AuthenticationCreds.StatusCode, "Authentication failed, see follow me error code for more information");
                    }
                }
                else
                {
                    return new FolloMeResponse(404, session.AuthenticationCreds.StatusCode, "Authentication failed, see follow me error code for more information");
                }
            }
            else
            {
                return new FolloMeResponse(404, FolloMeErrorCodes.NoCredentialsProvided, "Either the username or password provided were null");
            }
        }
    }
}
