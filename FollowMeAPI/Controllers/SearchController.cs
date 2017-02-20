using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;
using FollowMeAPI.Sessions;

namespace FollowMeAPI.Controllers
{
	[RoutePrefix("search")]
	public class SearchController : ApiController
	{
		[HttpGet]
		[Route("users/{searchString}")]
		public List<string> SearchUsers(string searchString)
		{
            string token = null;
            List<string> queryResults = new List<string>();

            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                try
                {
                    List<UserModel> results = WebApiApplication.db.QueryUsersByName(searchString);
                    foreach (UserModel model in results)
                    {
                        queryResults.Add(JsonConvert.SerializeObject(model));
                    }
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[SEARCH-USERS (BY-NAME)][ERROR] : Error while searching users by name, " + ex.Message);
                    return new List<string>();
                }

                try
                {
                    List<UserModel> results = WebApiApplication.db.QueryUsersByUserName(searchString);
                    foreach (UserModel model in results)
                    {
                        queryResults.Add(JsonConvert.SerializeObject(model));
                    }
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[SEARCH-USERS (BY-USERNAME)][ERROR] : Error while searching users by UserName, " + ex.Message);
                    return new List<string>();
                }
            }

            return queryResults;
        }

        [HttpGet] 
		[Route("trips/{searchString}")]
		public List<string> SearchTrips(string searchString)
		{
            string token = null;
            List<string> nameQueryResult = new List<string>();
            
            if (Request.Headers.Contains("Token"))
            {
                token = Request.Headers.GetValues("Token").FirstOrDefault();
            }

            if (SessionManager.ValidateSession(token))
            {
                try
                {
                    List<TripModel> results = WebApiApplication.db.QueryTripsByName(searchString);
                    foreach (TripModel model in results)
                    {
                        nameQueryResult.Add(JsonConvert.SerializeObject(model));
                    }
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[SEARCH-TRIPS][ERROR] : Error while searching trips by name, " + ex.Message);
                    return new List<string>();
                }
            }

			return nameQueryResult;
		}
	}
}
