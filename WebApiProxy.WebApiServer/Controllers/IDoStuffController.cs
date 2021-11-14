using System.Web.Http;
using WebApiProxy.Core;

namespace WebApiProxy.WebApiServer.Controllers
{
    [RoutePrefix("api/IDoStuff")]
    public class IDoStuffController : ApiController
    {
        [HttpPost]
        [Route("Calculate")]
        public AddResult Calculate(AddArgument arg)
        {
            return new DoStuff().Calculate(arg);
        }
    }
}