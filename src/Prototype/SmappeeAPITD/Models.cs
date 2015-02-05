using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmappeeAPITD
{
    public class ServiceLocationInfo
    {
        public int serviceLocationId { get; set; }
        public string name { get; set; }
        public string timezone { get; set; }
        public float lon { get; set; }
        public float lat { get; set; }
        public float electricityCost { get; set; }
        public string electricityCurrency { get; set; }

        public appliance[] appliances { get; set; }
        public actuator[] actuators { get; set; }

    }
    public class appliance
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
    public class actuator
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ConsumptionOverview
    {
        public int serviceLocationId { get; set; }
        public Consumption[] consumptions { get; set; }
    }

    public class Consumption
    {
        public long timestamp { get; set; }
        public float consumption { get; set; }
        public float solar { get; set; }
        public float alwaysOn { get; set; }
    }


    public class EventsOverview
    {
        public Event[] Property1 { get; set; }
    }

    public class Event
    {
        public int activePower { get; set; }
        public int applianceId { get; set; }
        public long timestamp { get; set; }

        //Todo return as standard timestamp (non utc)
    }


    public class ServicelocationOverview
    {
        public string appName { get; set; }
        public Servicelocation[] serviceLocations { get; set; }
    }

    public class Servicelocation
    {
        public int serviceLocationId { get; set; }
        public string name { get; set; }
    }
}
