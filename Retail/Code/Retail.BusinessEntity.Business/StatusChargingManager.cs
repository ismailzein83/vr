using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities.RecurringPeriod;
using Retail.BusinessEntity.Entities.Status;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class StatusChargingManager
    {
        public StatusChargingSet GetChargingSet(int chargingSetId)
        {
            Dictionary<int, StatusChargingSet> cachedStatusChargingSets = GetCachedStatusChargingSets();
            return cachedStatusChargingSets.GetRecord(chargingSetId);
        }

        public UpdateOperationOutput<StatusChargingSetDetail> UpdateStatusChargingSet(StatusChargingSet statusChargingSet)
        {
            var updateOperationOutput = new UpdateOperationOutput<StatusChargingSetDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            IStatusChargingSetDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusChargingSetDataManager>();

            if (dataManager.Update(statusChargingSet))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StatusChargingSetDetailMapper(GetChargingSet(statusChargingSet.StatusChargingSetId));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public InsertOperationOutput<StatusChargingSetDetail> AddStatusChargingSet(StatusChargingSet statusChargingSetItem)
        {
            var insertOperationOutput = new InsertOperationOutput<StatusChargingSetDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };

            IStatusChargingSetDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusChargingSetDataManager>();
            int insertedId = -1;
            if (dataManager.Insert(statusChargingSetItem, out insertedId))
            {
                statusChargingSetItem.StatusChargingSetId = insertedId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StatusChargingSetDetailMapper(GetChargingSet(statusChargingSetItem.StatusChargingSetId)); ;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;

            }

            return insertOperationOutput;
        }
        public IDataRetrievalResult<StatusChargingSetDetail> GetFilteredStatusChargingSet(DataRetrievalInput<StatusChargingSetQuery> input)
        {
            IStatusChargingSetDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusChargingSetDataManager>();
            var chargingSets = dataManager.GetStatusChargingSets().ToDictionary(x => x.StatusChargingSetId, x => x);
            Func<StatusChargingSet, bool> filterExpression = x => ((input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower())));
            return DataRetrievalManager.Instance.ProcessResult(input, chargingSets.ToBigResult(input, filterExpression, StatusChargingSetDetailMapper));
        }

        //public bool HasInitialCharging(EntityType entityType, long entityId, Guid statusDefinitionId, out Decimal initialCharge)
        //{
        //    StatusChargingSet chargingSet = GetChargingSet(entityType, entityId);
        //    if (chargingSet != null)
        //    {
        //        var statusCharge = chargingSet.Settings.StatusCharges.FirstOrDefault(itm => itm.StatusDefinitionId == statusDefinitionId);
        //        if (statusCharge != null)
        //        {
        //            initialCharge = statusCharge.InitialCharge;
        //            return true;
        //        }
        //    }
        //    initialCharge = 0;
        //    return false;
        //}
        
        public IEnumerable<RecurringPeriodConfig> GetRecurringPeriodExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RecurringPeriodConfig>(RecurringPeriodConfig.EXTENSION_TYPE);
        }

        public List<EntityStatusChargeInfo> GetStatusChargeInfos(int entityTypeId)
        {
            StatusDefinitionManager manager = new StatusDefinitionManager();
            Retail.BusinessEntity.Entities.StatusDefinitionFilter filter = new Retail.BusinessEntity.Entities.StatusDefinitionFilter
            {
                EntityType = (EntityType)entityTypeId
            };
            var statusDefinitions = manager.GetFilteredStatusDefinitions(filter);
            return statusDefinitions.Select(def => new EntityStatusChargeInfo
            {
                StatusDefinitionId = def.StatusDefinitionId,
                StatusName = def.Name,
                HasInitialCharge = def.Settings.HasInitialCharge,
                HasRecurringCharge = def.Settings.HasRecurringCharge
            }).ToList();
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStatusDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStatusDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, StatusChargingSet> GetCachedStatusChargingSets()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedStatusChargingSets",
               () =>
               {
                   IStatusChargingSetDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusChargingSetDataManager>();
                   return dataManager.GetStatusChargingSets().ToDictionary(x => x.StatusChargingSetId, x => x);
               });
        }

        //private StatusChargingSet GetChargingSet(EntityType entityType, long entityId)
        //{
        //    switch (entityType)
        //    {
        //        case EntityType.Account: return GetAccountChargingSet(entityId);
        //        case EntityType.AccountService: return GetServiceChargingSet(entityId);
        //        default: throw new NotImplementedException(string.Format("entityType '{0}'", entityType));
        //    }
        //}

        //private StatusChargingSet GetAccountChargingSet(long accountId)
        //{
        //    var account = (new AccountManager()).GetAccount(accountId);
        //    if (account == null)
        //        throw new NullReferenceException(String.Format("account '{0}'", accountId));
        //    if (account.Settings.StatusChargingSetId.HasValue)
        //        return GetChargingSet(account.Settings.StatusChargingSetId.Value);
        //    else
        //        return null;
        //}

        //private StatusChargingSet GetServiceChargingSet(long accountServiceId)
        //{
        //    var accountService = (new AccountServiceManager()).GetAccountService(accountServiceId);
        //    if (accountService == null)
        //        throw new NullReferenceException(String.Format("accountService '{0}'", accountServiceId));
        //    var chargingPolicy = accountService.ServiceChargingPolicyId.HasValue?(new ChargingPolicyManager()).GetChargingPolicy(accountService.ServiceChargingPolicyId.Value):null;
        //    if (chargingPolicy == null)
        //        throw new NullReferenceException(String.Format("chargingPolicy '{0}'", accountService.ServiceChargingPolicyId));
        //    if (chargingPolicy.Settings.StatusChargingSetId.HasValue)
        //        return GetChargingSet(chargingPolicy.Settings.StatusChargingSetId.Value);
        //    else
        //        return null;
        //}

        #endregion

        #region Mappers

        public StatusChargingSetDetail StatusChargingSetDetailMapper(StatusChargingSet statusChargingSet)
        {
            StatusChargingSetDetail statusChargingSetDetail = new StatusChargingSetDetail()
            {
                Entity = statusChargingSet,
                Description = ""
            };
            return statusChargingSetDetail;
        }
        #endregion
    }
}
