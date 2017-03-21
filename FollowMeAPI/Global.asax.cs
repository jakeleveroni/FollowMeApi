using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using FollowMeDataBase.DBCallWrappers;
using StorageManager.S3Wrapper;

namespace FollowMeAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static DB db = new DB();
        public static S3 s3 = new S3();

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
