using System;
using System.Collections.Generic;
using TOne.BusinessEntity.Entities;
namespace TOne.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<SwitchInfo> GetSwitches();
        List<Switch> GetFilteredSwitches(string switchName, int rowFrom, int rowTo);
        Switch GetSwitchDetails(int switchID);

        int UpdateSwitch(Switch switchObject);

        int InsertSwitch(Switch switchObject);
    }
}
