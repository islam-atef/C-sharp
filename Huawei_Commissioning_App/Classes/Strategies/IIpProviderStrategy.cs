using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public interface IIpProviderStrategy
    {
        bool GetIPs(IpPlan ipPlan, string? cabinetCode);
    }
}
