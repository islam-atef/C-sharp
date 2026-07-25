using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public class SnmpConfigModifier : IConfigModifier
    {
        public List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter)
        {
            var result = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains("[SNMP_CONFIG_BLOCK]"))
                {
                    var snmpBlock = GenerateSnmpBlock(cabinet, context);
                    result.AddRange(snmpBlock);
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }

        // need to modify the writing way and what it can write to the snmp block (we need to make it flexible so that it can write any command depending on the cabinet type and TE-Ports data).
        private List<string> GenerateSnmpBlock(CabinetInfo cabinet, OperationalContext context)
        {
            var block = new List<string>();
            
            block.Add(" snmp-agent local-engineid 800007DB03286ED4357FCD");
            block.Add(" snmp-agent sys-info contact TE");
            block.Add($" snmp-agent sys-info location {cabinet.Code1}");
            block.Add(" snmp-agent sys-info version v1 v2c");
            
            // Get targets based on Region (e.g. "11", "12", or "Unknown" fallback)
            string regionKey = context.RegionalSnmpTargets.ContainsKey(cabinet.Region) ? cabinet.Region : "Unknown";
            
            if (context.RegionalSnmpTargets.TryGetValue(regionKey, out var targets))
            {
                foreach (var target in targets)
                {
                    block.Add($" snmp-agent target-host trap-hostname {target.Hostname} address {target.IpAddress} udp-port {target.UdpPort} trap-paramsname {target.Hostname}");
                    block.Add($" snmp-agent target-host trap-paramsname {target.Hostname} {target.Version} securityname {context.SnmpReadCommunity}");
                }
            }
            
            block.Add(" snmp-agent trap enable standard");
            return block;
        }
    }
}
