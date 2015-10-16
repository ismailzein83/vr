using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
       // Vanrise.Entities.BigResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input);

        List<Switch> GetSwitches();

        List<SwitchAssignedDataSource> GetSwitchAssignedDataSources();

        bool UpdateSwitch(Switch switchObj);

        bool AddSwitch(Switch switchObj, out int insertedId);

        bool DeleteSwitch(int switchId);

        bool AreSwitchesUpdated(ref object updateHandle);
    }
}
