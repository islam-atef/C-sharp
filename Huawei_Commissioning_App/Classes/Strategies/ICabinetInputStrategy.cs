using System.Collections.Generic;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public interface ICabinetInputStrategy
    {
        IEnumerable<CabinetInfo> GetCabinets();
    }
}
