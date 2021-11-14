using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace WebApiProxy.WebApiServer.Routing
{
    public class IpBlockHandler : IHttpHandler
    {
        RequestContext requestContext;

        public IpBlockHandler(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        public bool IsReusable => throw new NotImplementedException();

        public void ProcessRequest(HttpContext context)
        {
            var jsonString = string.Empty;

            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            // context.Request.Form
            throw new NotImplementedException();
        }
    }
}