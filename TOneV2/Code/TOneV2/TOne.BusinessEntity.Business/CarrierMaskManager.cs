using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CarrierMaskManager
    {
        ICarrierMaskDataManager _dataManager;

        public CarrierMaskManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICarrierMaskDataManager>();
        }
        public Vanrise.Entities.IDataRetrievalResult<CarrierMask> GetCarrierMasks(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierMasksByCriteria(input));
        }

    }
}
