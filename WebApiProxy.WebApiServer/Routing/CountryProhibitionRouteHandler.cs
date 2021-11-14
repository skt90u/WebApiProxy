using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace WebApiProxy.WebApiServer.Routing
{
    public class CountryProhibitionRouteHandler : IRouteHandler
    {
        #region Fields

        private List<string> _countries;

        #endregion

        #region Public Constructor

        public CountryProhibitionRouteHandler(List<string> countries)
        {
            this._countries = countries;
        }

        #endregion

        #region IRouteHandler Members

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            IpBlockHandler handler = new IpBlockHandler(requestContext);
            return handler;
        }

        #endregion
    }
}