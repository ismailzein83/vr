using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchBrandManager
    {
        private Dictionary<int, SwitchBrand> GetCachedSwitchBrands()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchBrands",
               () =>
               {
                   ISwitchBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchBrandDataManager>();
                   IEnumerable<SwitchBrand> switchBrands = dataManager.GetSwitchBrands();
                   return switchBrands.ToDictionary(kvp => kvp.BrandId, kvp => kvp);
               });
        }

        public Vanrise.Entities.IDataRetrievalResult<SwitchBrand> GetFilteredSwitchBrands(Vanrise.Entities.DataRetrievalInput<SwitchBrandQuery> input)
        {
            var allSwitchBrands = GetCachedSwitchBrands();

            Func<SwitchBrand, bool> filterExpression = (switchObject) => (input.Query.Name == null || switchObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSwitchBrands.ToBigResult(input, filterExpression));
        }

        public SwitchBrand GetSwitchBrandById(int switchBrandId)
        {
            var switchBrands = GetCachedSwitchBrands();
            return switchBrands.GetRecord(switchBrandId);
        }

        public IEnumerable<SwitchBrand> GetAllSwitchBrands()
        {
            var switchBrands = GetCachedSwitchBrands();

            List<SwitchBrand> allBrands = new List<SwitchBrand>();

            foreach (KeyValuePair<int, SwitchBrand> pair in switchBrands)
            {

                allBrands.Add(new SwitchBrand() { BrandId = pair.Key, Name = pair.Value.Name });
            }
            return allBrands;

        }

        public UpdateOperationOutput<SwitchBrand> UpdateBrand(SwitchBrand brandObj)
        {
            UpdateOperationOutput<SwitchBrand> updateOperationOutput = new UpdateOperationOutput<SwitchBrand>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchBrandDataManager>();
            bool updated = dataManager.UpdateBrand(brandObj);

            if (updated)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetSwitchBrands");
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = brandObj;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public InsertOperationOutput<SwitchBrand> AddBrand(SwitchBrand brandObj)
        {
            InsertOperationOutput<SwitchBrand> insertOperationOutput = new InsertOperationOutput<SwitchBrand>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int brandId = -1;

            ISwitchBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchBrandDataManager>();
            bool inserted = dataManager.AddBrand(brandObj, out brandId);

            if (inserted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetSwitchBrands");
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

        public DeleteOperationOutput<object> DeleteBrand(int brandId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            deleteOperationOutput.Result = DeleteOperationResult.InUse;

            ISwitchBrandDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchBrandDataManager>();
            bool deleted = dataManager.DeleteBrand(brandId);

            if (deleted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetSwitchBrands");
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchBrandDataManager _dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchBrandDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchBrandsUpdated(ref _updateHandle);
            }
        }
        
    }
}
