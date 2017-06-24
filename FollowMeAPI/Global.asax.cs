using System.Web.Http;
using FollowMeDataBase.DBCallWrappers;
using FollowMeAPI.DataModels;
using System;

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

            //Db.AddNewTrip(t);
        }
    }
}
