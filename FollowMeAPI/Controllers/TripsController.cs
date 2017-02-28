using System;
using System.Web.Http;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using FollowMeAPI.Sessions;
using Utility;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("trips")]
    public class TripsController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public HttpResponseMessage GetTripModel()
        {
            string tripId = null;
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                if (tripId != null)
                {
                    try
                    {
                        var str = WebApiApplication.db.GetTrip(tripId);
                        Tools.logger.Debug("Serialized Object : " + str);
						response.Headers.Add("TripModel", str);
						response.StatusCode = System.Net.HttpStatusCode.OK;
						return response;
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[GET TRIP][ERROR] : Could not get the trip, " + ex.Message);
						throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
                    }
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("new")]
        public HttpResponseMessage PostTripModel([FromBody] TripModel tripJson)
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
                    WebApiApplication.db.AddNewTrip(tripJson);
					response.StatusCode = System.Net.HttpStatusCode.OK;
					return response;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[POST TRIP][ERROR] : Could not post trip to db" + ex.Message);
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("delete")]
		public HttpResponseMessage DeleteTripModel()
        {
            string tripId= string.Empty;
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();
			
            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                if (tripId != null)
                {
                    try
                    {
                        WebApiApplication.db.RemoveTrip(tripId);
						response.StatusCode = System.Net.HttpStatusCode.OK;
                        
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from db, " + ex.Message);
                    }
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }

        [HttpPatch]
        [Route("update")]
        public HttpResponseMessage PatchTripModel()
        {
            string token = null;
			HttpResponseMessage response = new HttpResponseMessage();

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                string tripId = null;
                Dictionary<string, string> kvp = new Dictionary<string, string>();

                if (Request.Headers.Contains("guid"))
                {
                    tripId = Request.Headers.GetValues("guid").FirstOrDefault();
                }

                if (tripId == null)
                {
					throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
                }

                if (Request.Headers.Contains("TripName"))
                {
                    kvp.Add("TripName", Request.Headers.GetValues("TripName").FirstOrDefault());
                }
                if (Request.Headers.Contains("Mileage"))
                {
                    kvp.Add("Mileage", Request.Headers.GetValues("Mileage").FirstOrDefault());
                }

                if (kvp.Count > 0)
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> entry in kvp)
                        {
                            TripItemEnums updateType = Tools.GetTripItemEnum(entry.Key);

                            if (updateType != TripItemEnums.InvalidUpdate && entry.Value != null)
                            {
                                WebApiApplication.db.UpdateTrip(tripId, entry.Value, updateType);
                            }
                        }

						response.StatusCode = System.Net.HttpStatusCode.OK;
						return response;
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip in db, " + ex.Message);
                    }
                }
            }

			throw new HttpResponseException(System.Net.HttpStatusCode.NoContent);
        }
    }
}
