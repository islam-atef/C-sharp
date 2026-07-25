using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public class LinkAggregationModifier : IConfigModifier
    {
        public List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter)
        {
            var result = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains("[LINK_AGGREGATION_BLOCK]"))
                {
                    var lagBlock = GenerateLagBlock(cabinet, context);
                    result.AddRange(lagBlock);
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }

        // need to modify the writing way and what it can write to the LAG block (we need to make it flexible so that it can write any command depending on the cabinet type and TE-Ports data).
        private List<string> GenerateLagBlock(CabinetInfo cabinet, OperationalContext context)
        {
            var block = new List<string>();
            
            // Determine ports based on Cabinet Type
            string mainPort = "0/9";
            string memberPort = "0/10";
            
            if (cabinet.CabinetType == "GPON300")
            {
                mainPort = "0/8";
                memberPort = "0/9";
            }
            else if (cabinet.CabinetType == "MA5818")
            {
                mainPort = "0/19";
                memberPort = "0/20";
            }

            block.Add($" link-aggregation {mainPort} 0 egress-ingress workmode {context.LinkAggregationMode} ");
            block.Add($" link-aggregation max-link-number {mainPort}/0 2");
            block.Add($" link-aggregation add-member {mainPort}/0 {memberPort} 0");
            block.Add($" link-aggregation lacp-key {mainPort}/0 1");
            
            return block;
        }
    }
}
