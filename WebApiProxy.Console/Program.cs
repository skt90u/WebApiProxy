using System;
using WebApiProxy.Core;

namespace WebApiProxy.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            for(var i=0; i<10000; i++)
            {
                try
                {
                    IWebApiProxy<IDoStuff> proxy = new WebApiProxy<IDoStuff>();
                    var instance = proxy.Instance;

                    //System.Console.WriteLine(instance.Calculate(new AddArgument { value1 = 1, value2 = 2 }));
                    System.Console.WriteLine($"{i} -> {instance.Add(5, 10)}");
                    //instance.Echo();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"{ex.ToString()}");
                }
            }
        }
    }
}
