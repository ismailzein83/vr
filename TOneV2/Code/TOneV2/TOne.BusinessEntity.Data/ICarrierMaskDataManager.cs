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

        CarrierMask GetCarrierMask(int carrierMaskId);

        bool UpdateCarrierMask(CarrierMask carrierMask);
        bool AddCarrierMask(CarrierMask carrierMask, out int insertedId);
        //bool DeleteCarrierMask(CarrierMask carrierMask);
    }
}
