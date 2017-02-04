using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        [Route("searchString:min(1)")]
        public List<string> SearchUsers(string searchString)
        {
            List<string> nameQueryResult = new List<string>();
            List<string> userNameQueryResults = new List<string>();

            try
            {
                List<UserModel> results = WebApiApplication.db.QueryUsersByName(searchString);
                foreach (UserModel model in results)
                {
                    nameQueryResult.Add(JsonConvert.SerializeObject(model));
                }
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SEARCH-USERS (BY-NAME)][ERROR] : Error while searching users by name, " + ex.Message);
                return new List<string>();
            }

            try
            {
                List<UserModel> results = WebApiApplication.db.QueryUsersByUserName(searchString);
                foreach (UserModel model in results)
                {
                    userNameQueryResults.Add(JsonConvert.SerializeObject(model));
                }
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SEARCH-USERS (BY-USERNAME)][ERROR] : Error while searching users by UserName, " + ex.Message);
                return new List<string>();
            }

            List<string> queryResults = new List<string>(nameQueryResult);
            queryResults.AddRange(userNameQueryResults);

            return queryResults;
        }
    }
}
