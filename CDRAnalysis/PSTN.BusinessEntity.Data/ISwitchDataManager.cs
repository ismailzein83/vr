using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchDataManager : IDataManager
    {
        List<SwitchType> GetSwitchTypes();

        Vanrise.Entities.BigResult<Switch> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input);

        Switch GetSwitchByID(int switchID);

        List<Switch> GetSwitches();

        Switch GetSwitchByDataSourceID(int dataSourceID);

        bool UpdateSwitch(Switch switchObject);

        bool AddSwitch(Switch switchObject, out int insertedID);

        bool DeleteSwitch(int switchID);
    }
}
