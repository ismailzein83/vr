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
        List<RateType> GetRateTypes();

        bool Update(RateType rateType);

        bool Insert(RateType rateType, out int insertedId);

        bool AreRateTypesUpdated(ref object UpdateHandle);

    }
}
