using System;
using System.Collections.Generic;

namespace Huawei_Commissioning_App.Classes.Models
{
    public class OperationalContext
    {
        // SNMP Settings
        public string SnmpReadCommunity { get; set; } = "public";
        public string SnmpWriteCommunity { get; set; } = "private";
        
        // Mapping of Region Code -> List of SNMP Targets
        public Dictionary<string, List<SnmpTarget>> RegionalSnmpTargets { get; set; } = new();

        // Link Aggregation & Ports
        public string LinkAggregationMode { get; set; } = "lacp-static";
        public bool AutoNegotiationEnabled { get; set; } = true;
        public string PortSpeed { get; set; } = "auto";

        public OperationalContext()
        {
            // Populate default SNMP targets for Region "11" (Sohag/Assiut) and "12" (Aswan)
            var defaultTargets = new List<SnmpTarget>
            {
                new SnmpTarget { Hostname = "Giza", IpAddress = "10.241.251.146", UdpPort = 162, Version = "v2C" },
                new SnmpTarget { Hostname = "ASSIA", IpAddress = "213.158.188.234", UdpPort = 162, Version = "v2C" },
                new SnmpTarget { Hostname = "ENTER", IpAddress = "196.219.224.1", UdpPort = 8001, Version = "v1" },
                new SnmpTarget { Hostname = "U2000", IpAddress = "213.158.166.18", UdpPort = 162, Version = "v1" },
                new SnmpTarget { Hostname = "almaza", IpAddress = "172.22.12.105", UdpPort = 162, Version = "v2C" },
                new SnmpTarget { Hostname = "NCE_146", IpAddress = "10.241.51.42", UdpPort = 162, Version = "v2C" },
                new SnmpTarget { Hostname = "almaza1", IpAddress = "10.14.253.238", UdpPort = 162, Version = "v2C" },
                new SnmpTarget { Hostname = "NCE_146_Standby", IpAddress = "10.241.51.58", UdpPort = 162, Version = "v2C" }
            };

            RegionalSnmpTargets["11"] = defaultTargets;
            RegionalSnmpTargets["12"] = defaultTargets;
            RegionalSnmpTargets["Unknown"] = defaultTargets; // Fallback
        }
    }

    public class SnmpTarget
    {
        public string Hostname { get; set; } = "";
        public string IpAddress { get; set; } = "";
        public int UdpPort { get; set; } = 162;
        public string Version { get; set; } = "v2c";
    }
}
