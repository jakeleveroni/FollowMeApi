using System;
using System.Web.Http;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;

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
                var str = WebApiApplication.db.GetUser(userId);
                Logger.logger.Debug("Serialized Object : " + str);
                return str;
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[GET USER][ERROR] : Could not get the user, " + ex.Message);
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
                Logger.logger.Error("[POST USER][ERROR] : Could not post user to db" + ex.Message);
                return false;
            }
        }

        [HttpDelete]
        [Route("{userId:guid}")]
        public bool DeleteUserModel(string userId)
        {
            try
            {
                WebApiApplication.db.RemoveUser(userId);
                return true;
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                return false;
            }
        }

        [HttpPatch]
        [Route("update/{userId:guid}/{key:minlength(1)}/{value:minlength(1)}/")]
        public bool PatchUserModel(string userId, string key, string value)
        {
            UserItemEnums updateType;
            System.Diagnostics.Debug.WriteLine("HIT IT!");
            // convert key to type of updating 
            switch (key)
            {
                case "Name":
                    updateType = UserItemEnums.UpdateName;
                    break;
                case "Email":
                    updateType = UserItemEnums.UpdateEmail;
                    break;
                case "BirthDate":
                    updateType = UserItemEnums.UpdateBirthDate;
                    break;
                case "NumberOfTrips":
                    updateType = UserItemEnums.UpdateNumberOfTrips;
                    break;
                case "Password":
                    updateType = UserItemEnums.UpdatePassword;
                    break;
                case "TotalMilesTraveled":
                    updateType = UserItemEnums.UpdateMilesTraveled;
                    break;
                case "UserName":
                    updateType = UserItemEnums.UpdateUserName;
                    break;
                case "TripIds":
                    updateType = UserItemEnums.UpdateTrips;
                    break;
                default:
                    return false;
            }

            try
            { 
                WebApiApplication.db.UpdateUser(userId, value, updateType);
                return true;
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                return false;
            }
        }
    }
}