using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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

        public static void ParseRouteUrl(string url, out string interfaceName, out string methodName)
        {
            var tokens = url.Split("/".ToCharArray());

            if (tokens.Length != 4)
                throw new Exception($"解析 Url 失敗，找不到 interfaceName, methodName (url = {url})");

            interfaceName = tokens[2];
            methodName = tokens[3];
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
            try
            {
                string interfaceName = string.Empty;
                string methodName = string.Empty;
                WebApiProxyRouteHandler.ParseRouteUrl(context.Request.Url.LocalPath, out interfaceName, out methodName);

                var type = Assembly.GetExecutingAssembly().GetTypes().First(t => ("I" + t.Name) == interfaceName);
                var methodInfo = type.GetMethod(methodName);
                var parameterInfos = methodInfo.GetParameters();
                var args = new List<object>();

                if (parameterInfos.Any())
                {
                    using (var inputStream = new StreamReader(context.Request.InputStream))
                    {
                        var jsonInput = inputStream.ReadToEnd();
                        var jObj = JObject.Parse(jsonInput);

                        foreach (var parameterInfo in parameterInfos)
                        {
                            var jToken = jObj[parameterInfo.Name];
                            args.Add(jToken.ToObject(parameterInfo.ParameterType));
                        }
                    }
                }

                var instance = Activator.CreateInstance(type);
                var result = methodInfo.Invoke(instance, args.ToArray());
                var jsonOutput = JsonConvert.SerializeObject(result);
                context.Response.Write(jsonOutput);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    ex.Message,
                    ex.StackTrace
                }));
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
