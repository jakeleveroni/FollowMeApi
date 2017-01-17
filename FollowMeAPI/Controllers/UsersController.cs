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
                var str = WebApiApplication.db.GetUser(userId);
                System.Diagnostics.Debug.WriteLine("Serialized Object : " + str);
                return str;
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
                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                return false;
            }
        }

        [HttpPatch]
        [Route("update/{userId:guid}/{key:minlength(1)}/{value:minlength(1)}/")]
        public bool PatchUserModel(string userId, string key, string value)
        {
            FollowMeDataBase.UserItemEnums updateType;
            System.Diagnostics.Debug.WriteLine("HIT IT!");
            // convert key to type of updating 
            switch (key)
            {
                case "Name":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateName;
                    break;
                case "Email":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateEmail;
                    break;
                case "BirthDate":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateBirthDate;
                    break;
                case "NumberOfTrips":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateNumberOfTrips;
                    break;
                case "Password":
                    updateType = FollowMeDataBase.UserItemEnums.UpdatePassword;
                    break;
                case "TotalMilesTraveled":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateMilesTraveled;
                    break;
                case "UserName":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateUserName;
                    break;
                case "TripIds":
                    updateType = FollowMeDataBase.UserItemEnums.UpdateTrips;
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
                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                return false;
            }
        }
    }
}