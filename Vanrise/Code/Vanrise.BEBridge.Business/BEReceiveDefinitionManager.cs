using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Data;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionManager : IBEReceiveDefinitionManager
    {
        #region Ctor/Fields

        static SecurityManager s_securityManager = new SecurityManager();

        #endregion
        #region Public Methods
        public BEReceiveDefinition GetBEReceiveDefinition(Guid id)
        {
            var allBEReceiveDefinitions = GetCachedBEReceiveDefinitions();
            return allBEReceiveDefinitions.GetRecord(id);
        }

        public IEnumerable<BEReceiveDefinitionInfo> GetBEReceiveDefinitionsInfo(BEReceiveDefinitionFilter filter)
        {
            Func<BEReceiveDefinition, bool> filterExpression = null;
            if (filter != null)
            {


                filterExpression = (beReceiveDefinition) =>
                {

                    if (filter.Filters != null && !CheckIfFilterIsMatch(beReceiveDefinition, filter.Filters))
                        return false;

                    return true;
                };
            }
            return GetCachedBEReceiveDefinitions().MapRecords(BEReceiveDefinitionInfoMapper, filterExpression);
        }
        public IDataRetrievalResult<BEReceiveDefinitionDetail> GetFilteredBeReceiveDefinitions(DataRetrievalInput<BEReceiveDefinitionQuery> input)
        {
            var receiveDefinitions = GetCachedBEReceiveDefinitions().Values.ToList();
            Func<BEReceiveDefinition, bool> filterExpression = x => ((input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower())));
            return DataRetrievalManager.Instance.ProcessResult(input, receiveDefinitions.ToBigResult(input, filterExpression, BeReceiveDefinitionDetailMapper));
        }
        public BEReceiveDefinition GetReceiveDefinition(Guid receiveDefinitionId)
        {
            return GetCachedBEReceiveDefinitions().GetRecord(receiveDefinitionId);
        }

        public string GetReceiveDefinitionName(Guid receiveDefinitionId)
        {
            BEReceiveDefinition receiveDefinition = GetReceiveDefinition(receiveDefinitionId);
            return receiveDefinition != null ? receiveDefinition.Name : null;
        }

        public InsertOperationOutput<BEReceiveDefinitionDetail> AddReceiveDefinition(BEReceiveDefinition beReceiveDefinition)
        {
            var insertOperationOutput = new InsertOperationOutput<BEReceiveDefinitionDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            IBEReceiveDefinitionDataManager dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
            beReceiveDefinition.BEReceiveDefinitionId = Guid.NewGuid();
            if (dataManager.Insert(beReceiveDefinition))
            {
                Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BeReceiveDefinitionDetailMapper(GetBEReceiveDefinition(beReceiveDefinition.BEReceiveDefinitionId)); ;
            }
            else
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public UpdateOperationOutput<BEReceiveDefinitionDetail> UpdateReceiveDefinition(BEReceiveDefinition beReceiveDefinition)
        {
            var updateOperationOutput = new UpdateOperationOutput<BEReceiveDefinitionDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            IBEReceiveDefinitionDataManager dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();

            if (dataManager.Update(beReceiveDefinition))
            {
                Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BeReceiveDefinitionDetailMapper(GetBEReceiveDefinition(beReceiveDefinition.BEReceiveDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public IEnumerable<SourceBeReadersConfig> GetSourceReaderExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SourceBeReadersConfig>(SourceBeReadersConfig.EXTENSION_TYPE);
        }
        public IEnumerable<BEConvertorConfig> GetTargetConvertorExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<BEConvertorConfig>(BEConvertorConfig.EXTENSION_TYPE);
        }
        public IEnumerable<TargetBESynchronizerConfig> GetTargetSynchronizerExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<TargetBESynchronizerConfig>(TargetBESynchronizerConfig.EXTENSION_TYPE);
        }
       
        

        #endregion

        #region Private Methods
        Dictionary<Guid, BEReceiveDefinition> GetCachedBEReceiveDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBEReceiveDefinitions",
               () =>
               {
                   IBEReceiveDefinitionDataManager dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
                   IEnumerable<BEReceiveDefinition> carrierAccounts = dataManager.GetBEReceiveDefinitions();
                   return carrierAccounts.ToDictionary(kvp => kvp.BEReceiveDefinitionId, kvp => kvp);
               });
        }

        BEReceiveDefinitionInfo BEReceiveDefinitionInfoMapper(BEReceiveDefinition beDefinition)
        {
            return new BEReceiveDefinitionInfo
            {
                Id = beDefinition.BEReceiveDefinitionId,
                Name = beDefinition.Name
            };
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBEReceiveDefinitionDataManager _dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBEReceiveDefinitionsUpdated(ref _updateHandle);
            }
        }


        private bool DoesUserHaveAccess(int userId, BEReceiveDefinition beReceiveDefinition, Func<BEReceiveDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            if (beReceiveDefinition != null && beReceiveDefinition.Settings.Security != null && beReceiveDefinition.Settings.Security != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(beReceiveDefinition.Settings.Security), userId);
            return true;
            
        }

        private bool CheckIfFilterIsMatch(BEReceiveDefinition BEReceiveDefinition, List<IBEReceiveDefinitionFilter> filters)
        {
            var context = new BEReceiveDefinitionFilterContext { BEReceiveDefinitionId = BEReceiveDefinition.BEReceiveDefinitionId };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region DetailMappers

        public BEReceiveDefinitionDetail BeReceiveDefinitionDetailMapper(BEReceiveDefinition beReceiveDefinition)
        {
            BEReceiveDefinitionDetail beReceiveDefinitionDetail = new BEReceiveDefinitionDetail
            {
                Entity = beReceiveDefinition,
                Description = ""
            };
            return beReceiveDefinitionDetail;
        }
        #endregion

        #region Security
        public bool DoesUserHaveStartInstanceAccess(Guid beRecieveDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var beRecieveDefinition =  GetBEReceiveDefinition(beRecieveDefinitionId);
            return DoesUserHaveAccess(userId, beRecieveDefinition, (sec) => sec.StartInstancePermission);
        }

        //public bool DoesUserHaveScheduleTaskAccess(Guid beRecieveDefinitionId)
        //{
        //    int userId = SecurityContext.Current.GetLoggedInUserId();
        //    var beRecieveDefinition = GetBEReceiveDefinition(beRecieveDefinitionId);
        //    return DoesUserHaveAccess(userId, beRecieveDefinition, (sec) => sec.ScheduleTaskPermission);
        //}
        public bool DoesUserHaveViewAccess(int userId)
        {
            var allBEdef = GetCachedBEReceiveDefinitions();
            foreach (var beDef in allBEdef)
            {
                if (DoesUserHaveAccess(userId, beDef.Value, (sec) => sec.ViewPermission))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveStartNewInstanceAccess(int userId)
        {
            var allBEdef = GetCachedBEReceiveDefinitions();
            foreach (var bedef in allBEdef)
            {
                if (DoesUserHaveAccess(userId, bedef.Value, (sec) => sec.StartInstancePermission))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveStartSpecificInstanceAccess(int userId, List<Guid> beReceiveDefinitionIds)
        {
            foreach (var be in beReceiveDefinitionIds)
            {
                var beDef = GetBEReceiveDefinition(be);
                if (!DoesUserHaveAccess(userId, beDef, (sec) => sec.StartInstancePermission))
                    return false;
            }
            return true;
        }

        #endregion
    }

    
}
