using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionManager : IBPDefinitionManager
    {
        #region public methods

        public IEnumerable<BPDefinition> GetBPDefinitions()
        {
            var cachedDefinitions = GetCachedBPDefinitions();
            if (cachedDefinitions != null)
                return cachedDefinitions.Values;
            else
                return null;
        }

        public Vanrise.Entities.IDataRetrievalResult<BPDefinitionDetail> GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input, int? viewableByUserId = null)
        {
            var allBPDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
            {
                if (input.Query.ShowOnlyVisibleInManagementScreen == true && prod.Configuration.NotVisibleInManagementScreen)
                    return false;

                if (!string.IsNullOrEmpty(input.Query.Title) && !prod.Title.ToLower().Contains(input.Query.Title.ToLower()))
                    return false;

                if (viewableByUserId != null && !DoesUserHaveViewAccess((int)viewableByUserId,prod))
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBPDefinitions.ToBigResult(input, filterExpression, BPDefinitionDetailMapper));
        }

        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(BPDefinitionInfoFilter filter)
        {
            var bpDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
            {
               
                if (!DoesUserHaveViewAccess(SecurityContext.Current.GetLoggedInUserId(), prod))
                    return false;

                return true;
            };

            return bpDefinitions.FindAllRecords(filterExpression).MapRecords(BPDefinitionInfoMapper);
            
        }

        public UpdateOperationOutput<BPDefinitionDetail> UpdateBPDefinition(BPDefinition bPDefinition)
        {
            IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateBPDefinition(bPDefinition);
            UpdateOperationOutput<BPDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BPDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BPDefinitionDetailMapper(bPDefinition);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public BPDefinition GetBPDefinition(int definitionId)
        {
            return GetCachedBPDefinitions().GetRecord(definitionId);
        }
        public BPDefinition GetDefinition(string processName)
        {
            return GetBPDefinitions().FirstOrDefault(itm => itm.Name == processName);
        }
        public bool DoesUserHaveViewAccess(int userId)
        {
            var allPB = GetCachedBPDefinitions().Select(x=>x.Value);
            foreach (var bp in allPB)
            {
                if (DoesUserHaveViewAccess(userId, bp))
                    return true;
            }
            return false;
        }
        #endregion

        #region private methods

        private bool DoesUserHaveViewAccess(int userId, BPDefinition bPDefinition)
        {
            SecurityManager secManager = new SecurityManager();
            if (bPDefinition.Configuration.Security != null && bPDefinition.Configuration.Security.View != null && !DoesUserHaveBPPermission( userId , bPDefinition.Configuration.Security.View))
                return false;
            return true;
        }

        private bool DoesUserHaveBPPermission(int userId, RequiredPermissionSettings permission)
        {
            SecurityManager secManager = new SecurityManager();
            return secManager.IsAllowed(permission, userId);
        }
        private Dictionary<int, BPDefinition> GetCachedBPDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPDefinitions",
               () =>
               {
                   IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
                   IEnumerable<BPDefinition> accounts = dataManager.GetBPDefinitions();
                   return accounts.ToDictionary(cn => cn.BPDefinitionID, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPDefinitionsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region mapper

       
        private BPDefinitionDetail BPDefinitionDetailMapper(BPDefinition bpDefinition)
        {
            if (bpDefinition == null)
                return null;

            return new BPDefinitionDetail()
            {
                Entity = bpDefinition,
            };
        }

        private BPDefinitionInfo BPDefinitionInfoMapper(BPDefinition bpDefinition)
        {
            if (bpDefinition == null)
                return null;

            return new BPDefinitionInfo()
            {
                BPDefinitionID = bpDefinition.BPDefinitionID,
                Name = bpDefinition.Title
            };
        }
        #endregion
    }
}