using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<Switch> GetSwitches();
        bool AddSwitch(Switch switchObj, out int insertedId);
        bool UpdateSwitch(Switch switchObj);
        bool DeleteSwitch(int switchId);
        bool AreSwitchesUpdated(ref object updateHandle);
    }
}
