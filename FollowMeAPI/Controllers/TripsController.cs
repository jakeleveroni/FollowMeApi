using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FollowMeAPI.DataModels;
using Newtonsoft.Json;
using System.Text;

namespace FollowMeAPI.Controllers
{
    [RoutePrefix("trips")]
    public class TripsController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public TripModel GetTripModel()
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
                    var str = WebApiApplication.Db.GetTrip(tripId);
                    Tools.logger.Debug("Serialized Object : " + str);
                    return JsonConvert.DeserializeObject<TripModel>(str);
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[GET TRIP][ERROR] : Could not get the trip, " + ex.Message);
                }
            }

            return new TripModel(false);
        }

        [HttpPost]
        [Route("new")]
        public TripModel PostTripModel([FromBody] TripModel tripJson)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            try
            {
                WebApiApplication.Db.AddNewTrip(tripJson);
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[POST TRIP][ERROR] : Could not post trip to Db" + ex.Message);
            }

            return tripJson;
        }

        [HttpDelete]
        [Route("delete")]
        public bool DeleteTripModel()
        {
            string tripId = string.Empty;

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId != null)
            {
                try
                {
                    WebApiApplication.Db.RemoveTrip(tripId);
                    return true;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from Db, " + ex.Message);
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
            if (Request.Headers.Contains("TripDescription"))
            {
                kvp.Add("TripDescription", Request.Headers.GetValues("TripDescription").FirstOrDefault());
            }
            if (Request.Headers.Contains("Participants"))
            {
                kvp.Add("Participants", Request.Headers.GetValues("Participants").FirstOrDefault());
            }
            if (Request.Headers.Contains("Moments"))
            {
                kvp.Add("Moments", Request.Headers.GetValues("Moments").FirstOrDefault());
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
                            WebApiApplication.Db.UpdateTrip(tripId, entry.Value, updateType);
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip in Db, " + ex.Message);
                }
            }

            return false;
        }

        [HttpPatch]
        [Route("route/update")]
        public bool AddRoutePoint()
        {
            string tripId = null;
            StringBuilder builder = new StringBuilder();

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId == null)
            {
                return false;
            }

            if (Request.Headers.Contains("Longitude"))
            {
                builder.Append(Request.Headers.GetValues("Longitude").FirstOrDefault() + ", ");

                if (Request.Headers.Contains("Latitude"))
                {
                    builder.Append(Request.Headers.GetValues("Latitude").FirstOrDefault());

                    try
                    {
                        WebApiApplication.Db.UpdateTrip(tripId, builder.ToString(), TripItemEnums.Route);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip route in Db, " + ex.Message);
                    }
                }
            }

            return false;
        }

        // when passing in a chunk of location data, do it 
        // in the format and from least recent to most recent:
        // "(longitude,latitude) (longitude,latitude) ... "
        [HttpPatch]
        [Route("route/update")]
        public bool AddRoutePointsChunk([FromBody] string routePoints)
        {
            string tripId = null;
            List<string> points = ParseRouteInput(routePoints);

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();
            }

            if (tripId == null)
            {
                return false;
            }

            try
            {
                WebApiApplication.Db.UpdateTrip(tripId, "", TripItemEnums.Route, new List<string>(points));
                return true;
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip route in Db, " + ex.Message);
            }

            return false;
        }

        [HttpGet]
        [Route("route/get")]
        public List<string> GetRoute()
        {
            string tripId = null;

            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();

                if (tripId == null)
                {
                    return null;
                }
            }

            try
            {
                string tripString = WebApiApplication.Db.GetTrip(tripId);
                var trip = JsonConvert.DeserializeObject<TripModel>(tripString);

                return trip.Route;
            }
            catch (Exception ex)
            {
                Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not get trip route in Db, " + ex.Message);
            }

            return null;
        }

        private List<string> ParseRouteInput(string routes)
        {
            List<string> route = new List<string>();
            var points = routes.Split(' ');

            foreach (var point in points)
            {
                var data = point.Split(',');
                route.Add($"{data[0].Substring(1)} {data[1].Substring(0, data[1].Length - 1)}");
            }

            return route;
        }
    }
}
