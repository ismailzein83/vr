using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class BrandManager
    {
        public List<Brand> GetBrands()
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return dataManager.GetBrands();
        }

        public Vanrise.Entities.IDataRetrievalResult<Brand> GetFilteredBrands(Vanrise.Entities.DataRetrievalInput<BrandQuery> input)
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredBrands(input));
        }

        public Brand GetBrandById(int brandId)
        {
            IBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<IBrandDataManager>();
            return dataManager.GetBrandById(brandId);
        }

        public InsertOperationOutput<Brand> AddBrand(Brand brandObj)
        {
            InsertOperationOutput<Brand> insertOperationOutput = new InsertOperationOutput<Brand>();

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

        public UpdateOperationOutput<Brand> UpdateBrand(Brand brandObj)
        {
            UpdateOperationOutput<Brand> updateOperationOutput = new UpdateOperationOutput<Brand>();

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
