using System;
using Huawei_Commissioning_App.Classes.Strategies;

namespace Huawei_Commissioning_App.Classes.Factories
{
    public enum CabinetInputType
    {
        Excel,
    }

    public static class CabinetInputFactory
    {
        public static ICabinetInputStrategy Create(CabinetInputType type, string parameter)
        {
            return type switch
            {
                CabinetInputType.Excel => new ExcelCabinetInputStrategy(parameter),
                _ => throw new ArgumentException($"Unsupported Cabinet Input Type: {type}")
            };
        }
    }
}
