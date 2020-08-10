using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BDMT.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceGuard.Debug.LogGeneratedClasses();
            CreateHostBuilder(args)
                        .Build()
                        .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();

                var port = Environment.GetEnvironmentVariable("PORT");
                if (port != null)
                {
                    webBuilder.UseUrls("http://*:" + port);
                }
            });
    }
    public static class Test
    {
        public static void SayHello(this object obj)
        {
            Console.WriteLine("OBJ VERSION " + obj);
        }
    }
}