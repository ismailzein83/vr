using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class OperatorConfigurationManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<OperatorConfigurationDetail> GetFilteredOperatorConfigurations(Vanrise.Entities.DataRetrievalInput<OperatorConfigurationQuery> input)
        {
            var allOperatorConfigurations = GetCachedOperatorConfigurations();

            Func<OperatorConfiguration, bool> filterExpression = (prod) =>
                 (input.Query.OperatorIds == null || input.Query.OperatorIds.Count == 0 || input.Query.OperatorIds.Contains(prod.OperatorId))
                 &&

                 (input.Query.OperatorConfigurationIds == null || input.Query.OperatorConfigurationIds.Contains(prod.OperatorConfigurationId));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allOperatorConfigurations.ToBigResult(input, filterExpression, OperatorConfigurationDetailMapper));
        }
        public OperatorConfiguration GetOperatorConfiguration(int operatorConfigurationId)
        {
            var info = GetCachedOperatorConfigurations();
            return info.GetRecord(operatorConfigurationId);
        }
        public Vanrise.Entities.InsertOperationOutput<OperatorConfigurationDetail> AddOperatorConfiguration(OperatorConfiguration operatorConfiguration)
        {
            Vanrise.Entities.InsertOperationOutput<OperatorConfigurationDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<OperatorConfigurationDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int infoId = -1;

            IOperatorConfigurationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorConfigurationDataManager>();
            bool insertActionSucc = dataManager.Insert(operatorConfiguration, out infoId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                operatorConfiguration.OperatorConfigurationId = infoId;
                insertOperationOutput.InsertedObject = OperatorConfigurationDetailMapper(operatorConfiguration);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<OperatorConfigurationDetail> UpdateOperatorConfiguration(OperatorConfiguration operatorConfiguration)
        {
            IOperatorConfigurationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorConfigurationDataManager>();

            bool updateActionSucc = dataManager.Update(operatorConfiguration);
            Vanrise.Entities.UpdateOperationOutput<OperatorConfigurationDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<OperatorConfigurationDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = OperatorConfigurationDetailMapper(operatorConfiguration);
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        #endregion

        #region Private Members
        private Dictionary<int, OperatorConfiguration> GetCachedOperatorConfigurations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetOperatorConfigurations",
               () =>
               {
                   IOperatorConfigurationDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorConfigurationDataManager>();
                   IEnumerable<OperatorConfiguration> config = dataManager.GetOperatorConfigurations();
                   return config.ToDictionary(cn => cn.OperatorConfigurationId, cn => cn);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IOperatorConfigurationDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IOperatorConfigurationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreOperatorConfigurationsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers

        private OperatorConfigurationDetail OperatorConfigurationDetailMapper(OperatorConfiguration config)
        {
            OperatorConfigurationDetail infoDetail = new OperatorConfigurationDetail();
            infoDetail.Entity = config;
            
            OperatorProfileManager operatorProfileManager = new OperatorProfileManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
            
            infoDetail.OperatorName = operatorProfileManager.GetOperatorProfile(config.OperatorId).Name;
            infoDetail.AmountTypeName = serviceTypeManager.GetServiceType(config.AmountType).Description;
            return infoDetail;
        }
        #endregion
    }

}
