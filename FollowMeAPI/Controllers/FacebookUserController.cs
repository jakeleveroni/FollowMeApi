using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Amazon.DynamoDBv2;
using FollowMeAPI.DataModels;
using Newtonsoft.Json;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("facebookusers")]
    public class FacebookUserController : ApiController
    {
        [HttpPost]
        [Route("new")]
        public FacebookUser PostFacebookUser([FromBody] FacebookUser jsonFacebookUser)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            try
            {
                if (jsonFacebookUser != null)
                {
                    WebApiApplication.Db.AddNewFacebookUser(jsonFacebookUser);
                }
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[POST USER][ERROR] : Could not post user to Db" + ex.Message);
            }

            return jsonFacebookUser;
        }

    }
}