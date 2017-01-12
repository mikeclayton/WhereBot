using System;
using Nancy;
using Nancy.Hosting.Self;
using WhereBot.Api.Server;

namespace WhereBot.Api
{

    internal static class Program
    {

        private static void Main(string[] args)
        {

            var settings = new HostConfiguration
            {
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                }
            };

            var bootstrapper = new DefaultNancyBootstrapper();

            Globals.Repository.LoadLocations();
            Globals.Repository.LoadResources();

            var uri = "http://localhost:1234";
            using (var host = new NancyHost(new Uri(uri), bootstrapper, settings))
            {
                host.Start();
                Console.WriteLine(string.Format("Running on {0}", uri));
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }

        }

    }

}
