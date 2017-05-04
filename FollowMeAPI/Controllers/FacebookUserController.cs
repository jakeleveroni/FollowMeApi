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
        [HttpGet]
        [Route("get")]
        public FacebookUser GetFacebookUser()
        {
            string userId = null;

            if (Request.Headers.Contains("facebookid"))
            {
                userId = Request.Headers.GetValues("facebookid").FirstOrDefault();
            }

            if (userId != null)
            {
                try
                {
                    var str = WebApiApplication.Db.GetFacebookUser(userId);
                    Tools.logger.Debug("Serialized Object : " + str);
                    return JsonConvert.DeserializeObject<FacebookUser>(str);
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[GET USER][ERROR] : Could not get the user, " + ex.Message);
                }
            }

            return new FacebookUser(false);
        }

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