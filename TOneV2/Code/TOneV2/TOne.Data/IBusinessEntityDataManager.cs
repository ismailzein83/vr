using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABS;
using TOne.Entities;

namespace TOne.Data
{
    public interface IBusinessEntityDataManager : IDataManager
    {
        List<SwitchInfo> GetSwitches();
    }
}
