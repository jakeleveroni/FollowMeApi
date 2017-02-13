using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FollowMeAPI.Sessions;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("auth")]
        public FollowMeSession LogInUser()
        {
            string userName = null;
            string password = null;

            if (Request.Headers.Contains("UserName") && Request.Headers.Contains("Password"))
            {
                userName = Request.Headers.GetValues("UserName").FirstOrDefault();
                password = Request.Headers.GetValues("Password").FirstOrDefault();
            }

            if (userName == null || password == null)
            {
                return null;
            }
            else
            {
                FollowMeSession session = new FollowMeSession(userName, password);
                SessionManager.AddNewSession(session);

                return session;
            }
        }
    }
}
