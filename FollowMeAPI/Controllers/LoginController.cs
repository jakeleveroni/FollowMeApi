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
        [Route("{username}/{password}")]
        public FollowMeSession LogInUser(string userName, string password)
        {
            FollowMeSession session = new FollowMeSession(userName, password);
            SessionManager.AddNewSession(session);

            return session;
        }
    }
}
