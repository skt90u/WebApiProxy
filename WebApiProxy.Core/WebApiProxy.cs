using ImpromptuInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Dynamic;
using System.Linq;

namespace WebApiProxy.Core
{
    public interface IWebApiProxy<T> where T : class
    {
        T Instance { get; }
    }

    /// <summary>
    /// NUGET 必須安裝 ImpromptuInterface
    /// </summary>
    public class WebApiProxy<T> : IWebApiProxy<T> where T : class
    {
        private T instance;
        public T Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!typeof(T).IsInterface)
                        throw new ArgumentException($"{typeof(T).Name} must be an Interface");

                    instance = new WebApiClient(typeof(T)).ActLike<T>();
                }

                return instance;
            }
        }

        public class WebApiClient : DynamicObject
        {
            private readonly Type interfaceType;

            public WebApiClient(Type interfaceType)
            {
                this.interfaceType = interfaceType;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var methodInfo = interfaceType.GetMethod(binder.Name);
                var parameterInfos = methodInfo.GetParameters();
                var baseUrl = "http://localhost:44327/";
                var resource = WebApiProxyRouteHandler.GetRouteUrl(interfaceType.Name, methodInfo.Name);
                var client = new RestClient(baseUrl);
                var request = new RestRequest(resource, Method.POST, DataFormat.Json);

                if (args.Length != 0)
                {
                    var postData = parameterInfos.Zip(args, (parameterInfo, arg) => new { key = parameterInfo.Name, val = arg }).ToDictionary(x => x.key, x => x.val);
                    request.AddJsonBody(postData);
                }

                var response = client.Execute(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var jsonInput = response.Content;
                    var jObj = JObject.Parse(jsonInput);
                    var Message = jObj["Message"]?.Value<string>();
                    var StackTrace = jObj["StackTrace"]?.Value<string>();

                    if (!string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(StackTrace))
                        throw new WebApiProxyException(Message, StackTrace);

                    throw new Exception(response.Content);
                }

                if (typeof(void).Equals(methodInfo.ReturnType))
                    result = null;
                else
                    result = JsonConvert.DeserializeObject(response.Content, methodInfo.ReturnType);

                return true;
            }
        }
    
    }

    public class WebApiProxyException : Exception
    {
        private string oldStackTrace;

        public WebApiProxyException(string message, string stackTrace) : base(message)
        {
            this.oldStackTrace = stackTrace;
        }


        public override string StackTrace
        {
            get
            {
                return this.oldStackTrace;
            }
        }

        public override string ToString()
        {
            return Message + "\r\n" + StackTrace;
        }
    }
}