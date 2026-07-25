using System;
using Huawei_Commissioning_App.Classes.Strategies;

namespace Huawei_Commissioning_App.Classes.Factories
{
    public enum IpProviderType
    {
        Excel,
        InMemory,
        Composite
    }

    public static class IpProviderFactory
    {
        public static IIpProviderStrategy Create(IpProviderType type, object? parameter = null)
        {
            return type switch
            {
                IpProviderType.Excel => new ExcelIpProviderStrategy((string[])parameter!),
                IpProviderType.InMemory => new InMemoryIpProviderStrategy(),
                IpProviderType.Composite => new CompositeIpProviderStrategy((IIpProviderStrategy[])parameter!),
                _ => throw new ArgumentException($"Unsupported IP Provider Type: {type}")
            };
        }
    }
}
