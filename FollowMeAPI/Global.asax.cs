using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using FollowMeDataBase.DBCallWrappers;

namespace FollowMeAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static DB db;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            db = new DB();
        }
    }
}
