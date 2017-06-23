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
        }
    }
}
