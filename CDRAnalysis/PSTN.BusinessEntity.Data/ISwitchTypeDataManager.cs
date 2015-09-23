using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTypeDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchType> GetFilteredSwitchTypes(Vanrise.Entities.DataRetrievalInput<SwitchTypeQuery> input);

        SwitchType GetSwitchTypeByID(int switchTypeID);

        bool AddSwitchType(SwitchType switchTypeObject, out int insertedID);

        bool UpdateSwitchType(SwitchType switchTypeObject);

        bool DeleteSwitchType(int switchTypeID);
    }
}
