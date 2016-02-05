using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Data
{
    public interface IRateTypeDataManager : IDataManager
    {
        List<Vanrise.Entities.RateType> GetRateTypes();

        bool Update(Vanrise.Entities.RateType rateType);

        bool Insert(Vanrise.Entities.RateType rateType, out int insertedId);

        bool AreRateTypesUpdated(ref object updateHandle);
    }
}
