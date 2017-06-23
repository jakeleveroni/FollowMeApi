using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FollowMeAPI.DataModels;
using Newtonsoft.Json;

namespace FollowMeAPI.Controllers
{
    // This class will define all the routes in our API
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public UserModel GetUserModel()
        {
            string userId = null;

            if (Request.Headers.Contains("guid"))
            {
                userId = Request.Headers.GetValues("guid").FirstOrDefault();
			}

            if (userId != null)
            {
                try
                {
                    var str = WebApiApplication.Db.GetUser(userId);
                    Tools.logger.Debug("Serialized Object : " + str);
					return JsonConvert.DeserializeObject<UserModel>(str);
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[GET USER][ERROR] : Could not get the user, " + ex.Message);
				}
            }

            return new UserModel(false);
        }

        [HttpPost]
        [Route("new")]
		public UserModel PostUserModel([FromBody] UserModel jsonUser)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            try
			{
                if (jsonUser != null)
				    WebApiApplication.Db.AddNewUser(jsonUser);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[POST USER][ERROR] : Could not post user to Db" + ex.Message);
			}

            return jsonUser;
        }

        [HttpDelete]
        [Route("delete")]
        public bool DeleteUserModel()
        {
            string userId = string.Empty;

			if (Request.Headers.Contains("guid"))
			{
				userId = Request.Headers.GetValues("guid").FirstOrDefault();
			}

			if (userId != null)
			{
				try
				{
					WebApiApplication.Db.RemoveUser(userId);
					return true;
				}
				catch (Exception ex)
				{
					Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user from Db, " + ex.Message);
				}
			}

            return false;
        }

		[HttpPatch]
        [Route("update")]
        public bool PatchUserModel()
        {
            string userId = null;
            Dictionary<string, string> kvp = new Dictionary<string, string>();


            if (Request.Headers.Contains("guid"))
            {
                userId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (userId == null)
            {
				throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
            }

            // the header is parsed out this way because the header collection is implemented in 
            // a fucking stupid way where you cant get the keys of the header, they are just hard coded string 
            // comparisons on the header collection. Fucking unbelievable...
            if (Request.Headers.Contains("Name"))
            {
                kvp.Add("Name", Request.Headers.GetValues("Name").FirstOrDefault());
            }
            if (Request.Headers.Contains("Email"))
            {
                kvp.Add("Email", Request.Headers.GetValues("Email").FirstOrDefault());
            }
            if (Request.Headers.Contains("BirthDate"))
            {
                kvp.Add("BirthDate", Request.Headers.GetValues("BirthDate").FirstOrDefault());
            }
            if (Request.Headers.Contains("NumberOfTrips"))
            {
                kvp.Add("NumberOfTrips", Request.Headers.GetValues("NumberOfTrips").FirstOrDefault());
            }
            if (Request.Headers.Contains("Password"))
            {
                kvp.Add("Password", Request.Headers.GetValues("Password").FirstOrDefault());
            }
            if (Request.Headers.Contains("TotalMilesTraveled"))
            {
                kvp.Add("TotalMilesTraveled", Request.Headers.GetValues("TotalMilesTraveled").FirstOrDefault());
            }
            if (Request.Headers.Contains("UserName"))
            {
                kvp.Add("UserName", Request.Headers.GetValues("UserName").FirstOrDefault());
            }
            if (Request.Headers.Contains("TripIds"))
            {
                kvp.Add("TripIds", Request.Headers.GetValues("TripIds").FirstOrDefault());
            }

            if (kvp.Count > 0)
            {
                try
                {

                    foreach (KeyValuePair<string, string> entry in kvp)
                    {
                        UserItemEnums updateType = Tools.GetUserUpdateEnum(entry.Key);

                        if (updateType != UserItemEnums.InvalidUpdate && entry.Value != null)
                        {
                            WebApiApplication.Db.UpdateUser(userId, entry.Value, updateType);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[UPDATE USER][ERROR] : Could not update user in Db, " + ex.Message);
                }
            }

            return false;
        }
    }
}