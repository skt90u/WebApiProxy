using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static bool ParseRouteUrl(string url, out string interfaceName, out string methodName)
        {
            var tokens = url.Split("/".ToCharArray());
            if (tokens.Length == 4)
            {
                interfaceName = tokens[2];
                methodName = tokens[3];
                return true;
            }
            else
            {
                interfaceName = null;
                methodName = null;
                return false;
            }
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
            string interfaceName = string.Empty; 
            string methodName = string.Empty;

            var jsonString = string.Empty;

            if(!WebApiProxyRouteHandler.ParseRouteUrl(context.Request.Url.LocalPath, out interfaceName, out methodName))
            {
                throw new Exception("AA");
            }

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }

            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => ("I" + t.Name) == interfaceName);
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod(methodName);
            var arg = JsonConvert.DeserializeObject(jsonString, method.GetParameters()[0].ParameterType);

            var result = method.Invoke(instance, new object[] { arg });

            context.Response.Write(JsonConvert.SerializeObject(result));
            context.Response.StatusCode = 200;
        }
    }
}
