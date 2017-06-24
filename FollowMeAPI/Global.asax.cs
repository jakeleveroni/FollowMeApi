using System.Web.Http;
using FollowMeDataBase.DBCallWrappers;
using FollowMeAPI.DataModels;
using System.Collections.Generic;

namespace FollowMeAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static DB Db = new DB();
        
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //TripModel t = new TripModel(Guid.NewGuid(), "ROUTE TESTING", 0, "TESTING ROUTE MANIPULATION");
            //t.AddRoutePoint(1.0f, 2.0f);
            //t.AddRoutePoint(2.0f, 3.0f);

            Db.UpdateTripRoute("34bcd503-3fb5-4d15-b5d9-43f67125371a", new List<RouteDataPoint> { new RouteDataPoint(5, 5), new RouteDataPoint(6, 6), new RouteDataPoint(7, 7) }, TripItemEnums.Route);
        }
    }
}
