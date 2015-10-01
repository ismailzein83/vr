using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input);

        SwitchDetail GetSwitchByID(int switchID);

        List<SwitchInfo> GetSwitches();

        List<SwitchInfo> GetSwitchesToLinkTo(int switchID);

        Switch GetSwitchByDataSourceID(int dataSourceID);

        bool UpdateSwitch(Switch switchObject);

        bool AddSwitch(Switch switchObject, out int insertedID);

        bool DeleteSwitch(int switchID);
    }
}
