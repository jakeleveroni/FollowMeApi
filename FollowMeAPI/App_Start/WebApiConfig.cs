using System;
using System.Web.Http;
using log4net.Config;

namespace FollowMeAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // enable log4net logging for debugging
            BasicConfigurator.Configure();
            Utility.Logger.logger.Info("[APPLICATION START]");

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
