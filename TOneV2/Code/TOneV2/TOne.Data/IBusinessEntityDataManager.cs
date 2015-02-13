using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface IBusinessEntityDataManager :IDataManager
    {
        List<CarrierInfo> GetCarriers(string carrierType);
        List<CodeGroupInfo> GetCodeGroups();
        List<SwitchInfo> GetSwitches();
    }
}
