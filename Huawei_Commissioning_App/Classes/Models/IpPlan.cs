using System;

namespace Huawei_Commissioning_App.Classes.Models
{
    public class IpPlan
    {
        public string? PopName { get; set; }

        // TED Mg IPs
        public string? TedMgGatewayIp { get; set; }
        public string? TedMgSH1Ip { get; set; }
        public string? TedMgSH2Ip { get; set; }

        // MG IPs
        public string? MgGatewayIp { get; set; }
        public string? MgSH1Ip { get; set; }
        public string? MgSH2Ip { get; set; }
        public string? MgSH3Ip { get; set; }

        // Sig IPs
        public string? SigGatewayIp { get; set; }
        public string? SigSH1Ip { get; set; }
        public string? SigSH2Ip { get; set; }

        // FVNO EM IPs
        public string? FvnoEmGatewayIp { get; set; }
        public string? FvnoEmSH1Ip { get; set; }
        public string? FvnoEmSH2Ip { get; set; }
    }
}
