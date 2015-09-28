using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTypeDataManager : IDataManager
    {
        List<SwitchType> GetSwitchTypes();

        Vanrise.Entities.BigResult<SwitchType> GetFilteredSwitchTypes(Vanrise.Entities.DataRetrievalInput<SwitchTypeQuery> input);

        SwitchType GetSwitchTypeByID(int switchTypeID);

        bool AddSwitchType(SwitchType switchTypeObject, out int insertedID);

        bool UpdateSwitchType(SwitchType switchTypeObject);

        bool DeleteSwitchType(int switchTypeID);
    }
}
