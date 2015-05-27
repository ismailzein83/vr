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

        bool UpdateSwitch(Switch switchObject);

        bool InsertSwitch(Switch switchObject, out int insertedId);
    }
}
