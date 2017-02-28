﻿using System;
using System.Web.Http;
using System.Net.Http;
using System.Linq;
using FollowMeDataBase.Models;
using FollowMeAPI.Sessions;
using Utility;
using System.Collections.Generic;

namespace FollowMeAPI.Controllers
{
    // This class will define all the routes in our API
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public HttpResponseMessage GetUserModel()
        {
            string userId = null;
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();

			if (Request.Headers.Contains("Token"))
			{
				token = Request.Headers.GetValues("Token").FirstOrDefault();
			}

            if (SessionManager.ValidateSession(token))
            {
                if (Request.Headers.Contains("guid"))
                {
                    userId = Request.Headers.GetValues("guid").FirstOrDefault();
				}

                if (userId != null)
                {
					response.Headers.Add("guid", userId);
					
                    try
                    {
                        var str = WebApiApplication.db.GetUser(userId);
                        Tools.logger.Debug("Serialized Object : " + str);
						response.Headers.Add("UserModel", str);
						response.StatusCode = System.Net.HttpStatusCode.OK;
						return response;
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[GET USER][ERROR] : Could not get the user, " + ex.Message);
						throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
					}
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("new")]
		public HttpResponseMessage PostUserModel([FromBody] UserModel jsonUser)
        {
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

			if (SessionManager.ValidateSession(token))
			{
				try
				{
					WebApiApplication.db.AddNewUser(jsonUser);
					response.Headers.Add("UserModel", jsonUser.ToString());
					response.StatusCode = System.Net.HttpStatusCode.OK;
					return response;

				}
				catch (Exception ex)
				{
					Tools.logger.Error("[POST USER][ERROR] : Could not post user to db" + ex.Message);
					throw new HttpResponseException(System.Net.HttpStatusCode.ExpectationFailed);
				}
			}

			throw new HttpResponseException(System.Net.HttpStatusCode.ExpectationFailed);
		}

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage DeleteUserModel()
        {
            string userId = string.Empty;
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

			if (SessionManager.ValidateSession(token))
			{
				if (Request.Headers.Contains("guid"))
				{
					userId = Request.Headers.GetValues("guid").FirstOrDefault();
				}

				if (userId != null)
				{
					try
					{
						WebApiApplication.db.RemoveUser(userId);
						response.StatusCode = System.Net.HttpStatusCode.OK;
						return response;
					}
					catch (Exception ex)
					{
						Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
						throw new HttpResponseException(System.Net.HttpStatusCode.ExpectationFailed);
					}
				}
			}

			throw new HttpResponseException(System.Net.HttpStatusCode.ExpectationFailed);
        }

        [HttpPatch]
        [Route("update")]
        public HttpResponseMessage PatchUserModel()
        {
            string userId = null;
            string token = null;
            Dictionary<string, string> kvp = new Dictionary<string, string>();
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
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
                                WebApiApplication.db.UpdateUser(userId, entry.Value, updateType);
								response.StatusCode = System.Net.HttpStatusCode.OK;
                                return response;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user from db, " + ex.Message);
                    }
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }
    }
}