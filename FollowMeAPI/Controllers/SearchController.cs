using System;
using System.Collections.Generic;
using System.Web.Http;
using FollowMeDataBase.Models;
using Newtonsoft.Json;
using Utility;

namespace FollowMeAPI.Controllers
{
	[RoutePrefix("search")]
	public class SearchController : ApiController
	{
		[HttpGet]
		[Route("users/{searchString}")]
		public List<string> SearchUsers(string searchString)
		{
			List<string> queryResults = new List<string>();

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
                Utility.Tools.logger.Error("[SEARCH-USERS (BY-NAME)][ERROR] : Error while searching users by name, " + ex.Message);
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
                Utility.Tools.logger.Error("[SEARCH-USERS (BY-USERNAME)][ERROR] : Error while searching users by UserName, " + ex.Message);
				return new List<string>();
			}

			return queryResults;
		}

		[HttpGet]
		[Route("trips/{searchString}")]
		public List<string> SearchTrips(string searchString)
		{
			List<string> nameQueryResult = new List<string>();

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
                Utility.Tools.logger.Error("[SEARCH-TRIPS][ERROR] : Error while searching trips by name, " + ex.Message);
				return new List<string>();
			}

			return nameQueryResult;
		}
	}
}
