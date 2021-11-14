using ImpromptuInterface;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Dynamic;

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
                try
                {
                    var methodInfo = interfaceType.GetMethod(binder.Name);
                    var parameters = methodInfo.GetParameters();
                    var baseUrl = "http://localhost:44327/";
                    var resource = WebApiProxyHandler.GetRouteUrl(interfaceType.Name, methodInfo.Name);

                    var client = new RestClient(baseUrl);
                    var request = new RestRequest(resource, Method.POST, DataFormat.Json);

                    if (args.Length != 0)
                    {
                        // Web Api 目前只允許一筆參數，多筆參數傳遞過去需要使用一下方式
                        // https://newbedev.com/pass-multiple-parameters-in-a-post-api-without-using-a-dto-class-in-net-core-mvc

                        request.AddJsonBody(args[0]);

                        //var postData = parameters.Zip(args, (parameter, arg) => new { key = parameter.Name, val = arg })
                        //                     .ToDictionary(x => x.key, x => x.val);
                        //request.AddJsonBody(postData);
                    }

                    IRestResponse response = client.Execute(request);

                    result = JsonConvert.DeserializeObject(response.Content, methodInfo.ReturnType);

                    return true;
                }
                catch
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}