using System;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FollowMeAPI.DataModels;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

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

        // NOTE: This is the correct way to do it, implemented a workaround for the current time
        //[HttpPatch]
        //[Route("route/update")]
        //public DataModels.DTO.RouteDTO AddRoutePoints([FromBody] DataModels.DTO.RouteDTO route)
        //{
        //    string tripId = null;
        //    if (Request.Headers.Contains("guid"))
        //    {
        //        tripId = Request.Headers.GetValues("guid").FirstOrDefault();
        //    }

        //    if (tripId != null)
        //    {
        //        try
        //        {
        //            WebApiApplication.Db.UpdateTripRoute(tripId, route.RoutePoints, TripItemEnums.Route);
        //        }
        //        catch (Exception ex)
        //        {
        //            Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip route in Db, " + ex.Message);
        //        }
        //    }

        //    return route;
        //}

            // NOTE: workaround for above function ^^^
        [HttpPatch]
        [Route("route/update")]
        public List<RouteDataPoint> AddRoutePoints()
        {
            string tripId = null;
            string route = null;
            List<RouteDataPoint> points = new List<RouteDataPoint>();
            if (Request.Headers.Contains("guid"))
            {
                tripId = Request.Headers.GetValues("guid").FirstOrDefault();

                if (Request.Headers.Contains("Route"))
                {
                    route = Request.Headers.GetValues("Route").FirstOrDefault();

                    if (route != null)
                    {
                        points = ParseRouteInput(route);
                    }

                    if (points != null)
                    {
                        if (tripId != null)
                        {
                            try
                            {
                                WebApiApplication.Db.UpdateTripRoute(tripId, points, TripItemEnums.Route);
                            }
                            catch (Exception ex)
                            {
                                Tools.logger.Error("[UPDATE TRIP][ERROR] : Could not update trip route in Db, " + ex.Message);
                            }
                        }
                    }
                }
            }

            return points;
        }

        [HttpGet]
        [Route("route/get")]
        public List<RouteDataPoint> GetRoute()
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

        private List<RouteDataPoint> ParseRouteInput(string routes)
        {
            List<RouteDataPoint> route = new List<RouteDataPoint>();
            var points = routes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var point in points)
            {
                var data = point.Split(',');
                route.Add(new RouteDataPoint(float.Parse(data[0].Substring(1)), float.Parse(data[1].Substring(0, data[1].Length - 1))));
            }

            return route;
        }
    }
}
