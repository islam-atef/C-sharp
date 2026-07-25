using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Modifiers
{
    public class IpConfigModifier : IConfigModifier
    {
        public List<string> Modify(List<string> lines, CabinetInfo cabinet, IpPlan ipPlan, OperationalContext context, int processCounter)
        {
            var result = new List<string>();

            // Determine whether we are processing Shelf 1 or Shelf 2
            // For Huawei MA5818:
            // processCounter 0 -> Code1, Shelf 1
            // processCounter 1 -> Code1, Shelf 2
            // processCounter 2 -> Code2, Shelf 1
            // processCounter 3 -> Code2, Shelf 2
            bool isShelf1 = (processCounter == 0 || processCounter == 2);

            string? activeTedMgIp = isShelf1 ? ipPlan.TedMgSH1Ip : ipPlan.TedMgSH2Ip;
            string? activeMgIp = isShelf1 ? ipPlan.MgSH1Ip : ipPlan.MgSH2Ip;
            string? activeSigIp = isShelf1 ? ipPlan.SigSH1Ip : ipPlan.SigSH2Ip;
            string? activeFvnoEmIp = isShelf1 ? ipPlan.FvnoEmSH1Ip : ipPlan.FvnoEmSH2Ip;

            foreach (var line in lines)
            {
                var tempLine = line;
                tempLine = tempLine.Replace("[TED_Mg_SH_IP]", activeTedMgIp ?? "");
                tempLine = tempLine.Replace("[TED_Mg_Gateway_IP]", ipPlan.TedMgGatewayIp ?? "");
                tempLine = tempLine.Replace("[Mg_SH_IP]", activeMgIp ?? "");
                tempLine = tempLine.Replace("[Mg_Gateway_IP]", ipPlan.MgGatewayIp ?? "");
                tempLine = tempLine.Replace("[Sig_SH_IP]", activeSigIp ?? "");
                tempLine = tempLine.Replace("[Sig_Gateway_IP]", ipPlan.SigGatewayIp ?? "");
                tempLine = tempLine.Replace("[FVNO_EM_SH_IP]", activeFvnoEmIp ?? "");
                tempLine = tempLine.Replace("[FVNO_EM_Gateway_IP]", ipPlan.FvnoEmGatewayIp ?? "");
                result.Add(tempLine);
            }
            return result;
        }
    }
}
