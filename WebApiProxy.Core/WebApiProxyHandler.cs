using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace WebApiProxy.Core
{
    public class WebApiProxyRouteHandler : IRouteHandler
    {
        public static string RouteUrlPattern = "WebApiProxy/{interfaceName}/{methodName}";


        public static string GetRouteUrl(string interfaceName, string methodName)
        {
            return RouteUrlPattern.Replace("{interfaceName}", interfaceName).Replace("{methodName}", methodName);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Add(new Route(RouteUrlPattern, new WebApiProxyRouteHandler()));
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new WebApiProxyHttpHandler(requestContext);
        }
    }

    public class WebApiProxyHttpHandler : IHttpHandler
    {
        private RequestContext requestContext;

        public WebApiProxyHttpHandler(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        public bool IsReusable => throw new NotImplementedException();

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
