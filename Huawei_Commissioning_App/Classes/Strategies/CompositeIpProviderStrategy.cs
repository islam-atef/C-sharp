using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public class CompositeIpProviderStrategy : IIpProviderStrategy
    {
        private readonly List<IIpProviderStrategy> _strategies = new();

        public CompositeIpProviderStrategy(params IIpProviderStrategy[] strategies)
        {
            _strategies.AddRange(strategies);
        }

        public bool GetIPs(IpPlan ipPlan, string? cabinetCode)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.GetIPs(ipPlan, cabinetCode))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
