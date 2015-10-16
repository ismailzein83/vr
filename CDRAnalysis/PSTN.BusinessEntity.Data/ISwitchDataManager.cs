using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<Switch> GetSwitches();
        bool UpdateSwitch(Switch switchObj);
        bool AddSwitch(Switch switchObj, out int insertedId);
        bool DeleteSwitch(int switchId);
        bool AreSwitchesUpdated(ref object updateHandle);
    }
}
