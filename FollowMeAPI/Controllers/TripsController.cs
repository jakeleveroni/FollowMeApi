using System;
using System.Web.Http;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("trips")]
    public class TripsController : ApiController
    {
        [HttpGet]
        [Route("{tripId:guid}")]
        public string GetTripModel(string tripId)
        {
            try
            {
                var str = WebApiApplication.db.GetTrip(tripId);

                Utility.Tools.logger.Debug("Serialized Object : " + str);
                return str;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[GET TRIP][ERROR] : Could not get the trip, " + ex.Message);
                return string.Empty;
            }
        }

        [HttpPost]
        [Route("{tripJson}")]
        public bool PostTripModel(string tripJson)
        {
            try
            {
                TripModel trip = JsonConvert.DeserializeObject<TripModel>(tripJson);
                WebApiApplication.db.AddNewTrip(trip);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[POST TRIP][ERROR] : Could not post trip to db" + ex.Message);
                return false;
            }
        }

        [HttpDelete]
        [Route("{tripId:guid}")]
        public bool DeleteTripModel(string tripId)
        {
            try
            {
                WebApiApplication.db.RemoveTrip(tripId);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from db, " + ex.Message);
                return false;
            }
        }

        [HttpPatch]
        [Route("update/{tripId:guid}/{key:minlength(1)}/{value:minlength(1)}/")]
        public bool PatchTripModel(string tripId, string key, string value)
        {
            TripItemEnums updateType;

            // convert key to type of updating 
            switch (key)
            {
                case "TripName":
                    updateType = TripItemEnums.UpdateTripName;
                    break;
                case "Mileage":
                    updateType = TripItemEnums.UpdateTripMileage;
                    break;
                default:
                    return false;
            }

            try
            {
                WebApiApplication.db.UpdateTrip(tripId, value, updateType);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip in db, " + ex.Message);
                return false;
            }
        }
    }
}
