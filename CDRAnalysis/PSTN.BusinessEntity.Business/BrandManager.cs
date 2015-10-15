using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class BrandManager
    {
        public List<SwitchBrand> GetBrands()
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return dataManager.GetBrands();
        }

        public Vanrise.Entities.IDataRetrievalResult<SwitchBrand> GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredBrands(input));
        }

        public SwitchBrand GetBrandById(int brandId)
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return dataManager.GetBrandById(brandId);
        }

        public InsertOperationOutput<SwitchBrand> AddBrand(SwitchBrand brandObj)
        {
            InsertOperationOutput<SwitchBrand> insertOperationOutput = new InsertOperationOutput<SwitchBrand>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int brandId = -1;

            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            bool inserted = dataManager.AddBrand(brandObj, out brandId);

            if (inserted)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                brandObj.BrandId = brandId;
                insertOperationOutput.InsertedObject = brandObj;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<SwitchBrand> UpdateBrand(SwitchBrand brandObj)
        {
            UpdateOperationOutput<SwitchBrand> updateOperationOutput = new UpdateOperationOutput<SwitchBrand>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            bool updated = dataManager.UpdateBrand(brandObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = brandObj;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public DeleteOperationOutput<object> DeleteBrand(int brandId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            bool deleted = dataManager.DeleteBrand(brandId);

            if (deleted)
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;

            return deleteOperationOutput;
        }
    }
}
