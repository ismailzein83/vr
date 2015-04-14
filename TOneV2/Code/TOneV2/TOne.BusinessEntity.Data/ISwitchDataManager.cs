using System;
using System.Collections.Generic;
using TOne.BusinessEntity.Entities;
namespace TOne.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<SwitchInfo> GetSwitches();
    }
}
