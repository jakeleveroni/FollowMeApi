using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("trips")]
    public class TripsController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public string GetTripModel()
        {
            string tripId = null;

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId != null)
            {
                try
                {
                    var str = WebApiApplication.db.GetTrip(tripId);
                    Tools.logger.Debug("Serialized Object : " + str);
                    return str;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[GET TRIP][ERROR] : Could not get the trip, " + ex.Message);
                }
            }

            return null;
        }

        [HttpPost]
        [Route("new")]
        public string PostTripModel([FromBody] TripModel tripJson)
        {
            try
            {
                WebApiApplication.db.AddNewTrip(tripJson);
                return "Successfully added trip to db";
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[POST TRIP][ERROR] : Could not post trip to db" + ex.Message);
                return ex.Message;
            }
        }

        [HttpDelete]
        [Route("delete")]
        public bool DeleteTripModel()
        {
            string tripId= string.Empty;

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId != null)
            {
                try
                {
                    WebApiApplication.db.RemoveTrip(tripId);
                    return true;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from db, " + ex.Message);
                }
            }

            return false;
        }

        [HttpPatch]
        [Route("update")]
        public bool PatchTripModel()
        {
            string tripId = null;
            Dictionary<string, string> kvp = new Dictionary<string, string>();

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId == null)
            {
                return false;
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
                            return true;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip in db, " + ex.Message);
                } 
            }

            return false;
        }
    }
}
