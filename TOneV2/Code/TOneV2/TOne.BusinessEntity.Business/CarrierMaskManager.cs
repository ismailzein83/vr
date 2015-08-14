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
        public Vanrise.Entities.IDataRetrievalResult<CarrierMask> GetFilteredCarrierMasks(Vanrise.Entities.DataRetrievalInput<CarrierMaskQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetCarrierMasksByCriteria(input));
        }

        public CarrierMask GetCarrierMask(int carrierMaskId)
        {
            return _dataManager.GetCarrierMask(carrierMaskId);
        }

        public TOne.Entities.UpdateOperationOutput<CarrierMask> UpdateCarrierMask(CarrierMask carrierMask)
        {
            bool updateActionSucc = _dataManager.UpdateCarrierMask(carrierMask);
            TOne.Entities.UpdateOperationOutput<CarrierMask> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierMask>();

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierMask;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            return updateOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<CarrierMask> AddCarrierMask(CarrierMask carrierMask)
        {
            TOne.Entities.InsertOperationOutput<CarrierMask> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierMask>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int carrierMaskId = -1;
            bool insertActionSucc = _dataManager.AddCarrierMask(carrierMask, out carrierMaskId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                carrierMask.ID = carrierMaskId;
                insertOperationOutput.InsertedObject = carrierMask;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

    }
}
