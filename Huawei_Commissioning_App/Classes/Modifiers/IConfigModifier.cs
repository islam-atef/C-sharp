using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public interface IConfigModifier
    {
        List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter);
    }
}
