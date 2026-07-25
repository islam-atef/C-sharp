using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public class InMemoryIpProviderStrategy : IIpProviderStrategy
    {
        private readonly Dictionary<string, IpPlan> _ipPlans = new();

        public void AddIpPlan(string cabinetCode, IpPlan ipPlan)
        {
            if (!string.IsNullOrEmpty(cabinetCode) && ipPlan != null)
            {
                _ipPlans[cabinetCode] = ipPlan;
            }
        }

        public bool GetIPs(IpPlan ipPlan, string? cabinetCode)
        {
            if (cabinetCode != null && _ipPlans.TryGetValue(cabinetCode, out var storedPlan))
            {
                // Copy properties to the target ipPlan
                ipPlan.PopName = storedPlan.PopName;
                ipPlan.TedMgGatewayIp = storedPlan.TedMgGatewayIp;
                ipPlan.TedMgSH1Ip = storedPlan.TedMgSH1Ip;
                ipPlan.TedMgSH2Ip = storedPlan.TedMgSH2Ip;
                ipPlan.MgGatewayIp = storedPlan.MgGatewayIp;
                ipPlan.MgSH1Ip = storedPlan.MgSH1Ip;
                ipPlan.MgSH2Ip = storedPlan.MgSH2Ip;
                ipPlan.MgSH3Ip = storedPlan.MgSH3Ip;
                ipPlan.SigGatewayIp = storedPlan.SigGatewayIp;
                ipPlan.SigSH1Ip = storedPlan.SigSH1Ip;
                ipPlan.SigSH2Ip = storedPlan.SigSH2Ip;
                ipPlan.FvnoEmGatewayIp = storedPlan.FvnoEmGatewayIp;
                ipPlan.FvnoEmSH1Ip = storedPlan.FvnoEmSH1Ip;
                ipPlan.FvnoEmSH2Ip = storedPlan.FvnoEmSH2Ip;
                return true;
            }
            return false;
        }
    }
}
