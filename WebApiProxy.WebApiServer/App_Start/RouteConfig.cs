using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using WebApiProxy.WebApiServer.Handler;
using WebApiProxy.WebApiServer.Routing;

namespace WebApiProxy.WebApiServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Add
                (new Route("order/{0}",
                    new CountryProhibitionRouteHandler(new List<string>() { "XX" })));
        }
    }
}