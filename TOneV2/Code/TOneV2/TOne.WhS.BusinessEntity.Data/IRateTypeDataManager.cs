using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRateTypeDataManager:IDataManager
    {
        List<TOne.WhS.BusinessEntity.Entities.RateType> GetRateTypes();

        bool Update(TOne.WhS.BusinessEntity.Entities.RateType rateType);

        bool Insert(TOne.WhS.BusinessEntity.Entities.RateType rateType, out int insertedId);

        bool AreRateTypesUpdated(ref object updateHandle);

    }
}
