using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICarrierMaskDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<CarrierMask> GetCarrierMasksByCriteria(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input);
    }
}
