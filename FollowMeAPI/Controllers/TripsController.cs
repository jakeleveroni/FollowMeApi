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
                System.Diagnostics.Debug.WriteLine("Serialized Object : " + str);
                return str;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exceptions thrown");
                System.Diagnostics.Debug.WriteLine("[GET TRIP][ERROR] : Could not get the trip, " + ex.Message);
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
                Console.WriteLine("[POST TRIP][ERROR] : Could not post trip to db" + ex.Message);
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
                Console.WriteLine("[REMOVE TRIP][ERROR] : Could not remove trip from db, " + ex.Message);
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
                System.Diagnostics.Debug.WriteLine("[UPDATE TRIP][ERROR] : Could not update trip in db, " + ex.Message);
                return false;
            }
        }
    }
}
