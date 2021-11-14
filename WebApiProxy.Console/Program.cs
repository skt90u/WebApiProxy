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
                    var result = instance.Calculate(new AddArgument { value1 = 1, value2 = 2 });
                    System.Console.WriteLine(result.add);
                    System.Console.WriteLine(result.sub);
                    System.Console.WriteLine(result.mul);
                    System.Console.WriteLine(result.div);
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine($"{i} {ex.Message}");
                }
            }
        }
    }
}
