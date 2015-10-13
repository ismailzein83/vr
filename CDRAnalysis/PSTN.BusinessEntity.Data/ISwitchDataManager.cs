using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input);

        SwitchDetail GetSwitchById(int switchId);

        List<SwitchInfo> GetSwitches();

        List<SwitchInfo> GetSwitchesToLinkTo(int switchId);

        Switch GetSwitchByDataSourceId(int dataSourceId);

        List<SwitchInfo> GetSwitchesByIds(List<int> switchIds);

        List<SwitchAssignedDataSource> GetSwitchAssignedDataSources();

        bool UpdateSwitch(Switch switchObj);

        bool AddSwitch(Switch switchObj, out int insertedId);

        bool DeleteSwitch(int switchId);
    }
}
