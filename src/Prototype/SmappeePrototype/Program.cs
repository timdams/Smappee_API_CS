using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmappeeAPITD;
using SmappeePrototype.Properties;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SmappeePrototype
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TestAsync().Wait();
        }

        private static async Task TestAsync()
        {
            //Example usage


            SmappeeAPI dll = new SmappeeAPI("yourClient_id","yourClient_secret", "yourmysmappeeusername","yourmysmappeepassword");

            await dll.RetrieveAccessToken();

            var serv = await dll.GetServiceLocations();
            //Ask consumption of last week of first smappeedevice retrieved from ServiceLocations
            var resk =
                await
                    dll.GetConsumption(serv.serviceLocations[0].serviceLocationId,
                        DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)),
                        DateTime.Now, SmappeeAPITD.Aggregation.Hourly);

        }

    }
}
