using System.Web.Http;
using log4net.Config;

namespace FollowMeAPI
{
    public static class WebApiConfig
    {
		public const string Version = "1.2.0";

        public const string LastCommit = "Updated the content upload download implementation to test multiple methods (multipart/base 64 string)";

        public static void Register(HttpConfiguration config)
        {
            // enable log4net logging for debugging
            BasicConfigurator.Configure();
            Tools.logger.Info("[APPLICATION START]");

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
