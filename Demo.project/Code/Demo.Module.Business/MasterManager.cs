using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Demo.Module.Business
{
   public class MasterManager
    {

        #region Public Methods
        public IDataRetrievalResult<MasterDetails> GetFilteredMasters(DataRetrievalInput<MasterQuery> input)
        {
            var allMasters = GetCachedMasters();
            Func<Master, bool> filterExpression = (master) =>
            {
                if (input.Query.Name != null && !master.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allMasters.ToBigResult(input, filterExpression, MasterDetailMapper));

        }
        public string GetMasterName(long masterId)
        {
            var parent = GetMasterById(masterId);
            if (parent == null)
                return null;
            return parent.Name;
        }


        public InsertOperationOutput<MasterDetails> AddMaster(Master master)
        {
            IMasterDataManager masterDataManager = DemoModuleFactory.GetDataManager<IMasterDataManager>();
            InsertOperationOutput<MasterDetails> insertOperationOutput = new InsertOperationOutput<MasterDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long masterId = -1;

            bool insertActionSuccess = masterDataManager.Insert(master, out masterId);
            if (insertActionSuccess)
            {
                master.MasterId = masterId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = MasterDetailMapper(master);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Master GetMasterById(long masterId)
        {
            return GetCachedMasters().GetRecord(masterId);
        }
        
        public UpdateOperationOutput<MasterDetails> UpdateMaster(Master master)
        {
            IMasterDataManager masterDataManager = DemoModuleFactory.GetDataManager<IMasterDataManager>();
            UpdateOperationOutput<MasterDetails> updateOperationOutput = new UpdateOperationOutput<MasterDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = masterDataManager.Update(master);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MasterDetailMapper(master);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IMasterDataManager masterDataManager = DemoModuleFactory.GetDataManager<IMasterDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return masterDataManager.AreMastersUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Master> GetCachedMasters()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedMasters", () =>
               {
                   IMasterDataManager masterDataManager = DemoModuleFactory.GetDataManager<IMasterDataManager>();
                   List<Master> masters = masterDataManager.GetMasters();
                   return masters.ToDictionary(master => master.MasterId, master => master);
               });
        }
        #endregion
        
        #region Mappers
        public MasterDetails MasterDetailMapper(Master master)
        {
            return new MasterDetails
            {
                Name = master.Name,
                MasterId = master.MasterId
            };
        }

      
        #endregion 

    }
}
