using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public class PortConfigModifier : IConfigModifier
    {
        public List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter)
        {
            var result = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains("[PORT_CONFIGURATION_BLOCK]"))
                {
                    var portBlock = GeneratePortBlock(cabinet, context);
                    result.AddRange(portBlock);
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }

        // we need to modify the writing way and what it can write to the port block (we need to make it flexible so that it can write any command depending on the cabinet type and TE-Ports data).
        // the command we write for the interface of giu 0/0 is just an example.
        // so we need to make it flexible so that it can write any command depending on the cabinet type and TE-Ports data.
        private List<string> GeneratePortBlock(CabinetInfo cabinet, OperationalContext context)
        {
            var block = new List<string>();
            
            // Example: Configure Uplink ports based on Cabinet Type
            string uplinkPort = cabinet.CabinetType == "GPON300" ? "0/8" : "0/9";
            
            block.Add("interface giu 0/0");
            if (context.AutoNegotiationEnabled)
            {
                block.Add($" auto-negotiate {uplinkPort} enable");
            }
            else
            {
                block.Add($" auto-negotiate {uplinkPort} disable");
                block.Add($" speed {uplinkPort} {context.PortSpeed}");
            }
            
            return block;
        }
    }
}
