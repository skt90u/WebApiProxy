using System.Web.Routing;
using WebApiProxy.Core;

namespace WebApiProxy.WebApiServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            WebApiProxyRouteHandler.RegisterRoutes(routes);
        }
    }
}