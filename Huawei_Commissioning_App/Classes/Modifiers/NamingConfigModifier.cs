using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public class NamingConfigModifier : IConfigModifier
    {
        public List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter)
        {
            var result = new List<string>();
            foreach (var line in lines)
            {
                var tempLine = line;
                
                // Replace POP Name
                tempLine = tempLine.Replace("[POP_Name]", ipPlan.PopName ?? "POP_Name");

                // Replace Cabinet Code & Shelf Suffixes
                switch (processCounter)
                {
                    case 0:
                        tempLine = tempLine.Replace("(00-00-00-00)", $"({cabinet.Code1})");
                        break;
                    case 1:
                        tempLine = tempLine.Replace("(00-00-00-00)(SH2)", $"({cabinet.Code1})(SH2)");
                        break;
                    case 2:
                        tempLine = tempLine.Replace("(00-00-00-00)(SH2)", $"({cabinet.Code2})(SH1)");
                        break;
                    case 3:
                        tempLine = tempLine.Replace("(00-00-00-00)(SH2)", $"({cabinet.Code2})(SH2)");
                        break;
                }
                result.Add(tempLine);
            }
            return result;
        }
    }
}
