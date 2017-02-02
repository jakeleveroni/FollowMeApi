using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using log4net.Config;
using FollowMeDataBase.DBCallWrappers;
using FollowMeDataBase.Models;

namespace FollowMeAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // enable log4net logging for debugging
            XmlConfigurator.Configure();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
