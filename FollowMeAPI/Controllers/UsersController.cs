using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FollowMeDataBase.Models;
using Newtonsoft.Json;


namespace FollowMeAPI.Controllers
{
    // This class will define all the routes in our API
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        [Route("{userId:guid}")]
        public string GetUserModel(string userId)
        {
            try
            {
                Guid result;
                if (Guid.TryParse(userId,out result))
                {
                    var str = WebApiApplication.db.GetUser(result);
                    System.Diagnostics.Debug.WriteLine("Serialized Object : " + str);
                    return str;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could Not Serialized Object");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exceptions thrown");
                System.Diagnostics.Debug.WriteLine("[GET USER][ERROR] : Could not get the user, " + ex.Message);
                return string.Empty;
            }
        }

        [HttpPost]
        [Route("{userJson}")]
        public bool PostUserModel(string jsonUser)
        {
            try
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(jsonUser);
                WebApiApplication.db.AddNewUser(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[POST USER][ERROR] : Could not post user to db" + ex.Message);
                return false;
            }
        }

        [HttpDelete]
        [Route("{userId:maxlength(36)}")]
        public bool DeleteUserModel(string userId)
        {
            try
            {
                Guid result;
                
                if (Guid.TryParse(userId, out result))
                {
                    WebApiApplication.db.RemoveUser(result);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                return false;
            }
        }
    }
}