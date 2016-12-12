using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleDefinitionManager : IGenericRuleDefinitionManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<GenericRuleDefinition> GetFilteredGenericRuleDefinitions(Vanrise.Entities.DataRetrievalInput<GenericRuleDefinitionQuery> input)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            Func<GenericRuleDefinition, bool> filterExpression = (genericRuleDefinition) => (input.Query.Name == null || genericRuleDefinition.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedGenericRuleDefinitions.ToBigResult(input, filterExpression));
        }

        public GenericRuleDefinition GetGenericRuleDefinition(Guid genericRuleDefinitionId)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            return cachedGenericRuleDefinitions.GetRecord(genericRuleDefinitionId);
        }

        public IEnumerable<GenericRuleDefinition> GetGenericRulesDefinitons()
        {
            return this.GetCachedGenericRuleDefinitions().Values;
        }

        public Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition> AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            InsertOperationOutput<GenericRuleDefinition> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
             genericRuleDefinition.GenericRuleDefinitionId = Guid.NewGuid();

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.AddGenericRuleDefinition(genericRuleDefinition);

            if (added)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = genericRuleDefinition;

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition> UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            UpdateOperationOutput<GenericRuleDefinition> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = genericRuleDefinition;

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.UpdateGenericRuleDefinition(genericRuleDefinition);

            if (added)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<GenericRuleDefinitionInfo> GetGenericRuleDefinitionsInfo(GenericRuleDefinitionInfoFilter filter)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            Func<GenericRuleDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (item) =>   (item.SettingsDefinition != null && item.SettingsDefinition.ConfigId == filter.RuleTypeId)  || (filter.Filters != null && CheckIfFilterIsMatch(item, filter.Filters));
            }

            return cachedGenericRuleDefinitions.MapRecords(GenericRuleDefinitionInfoMapper, filterExpression);
        }
        public bool CheckIfFilterIsMatch(GenericRuleDefinition ruleDefinition,List<IGenericRuleDefinitionFilter> filters)
        {                    
            GenericRuleDefinitionFilterContext context = new GenericRuleDefinitionFilterContext{ RuleDefinition = ruleDefinition};
            foreach(var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }
        public IEnumerable<GenericRuleDefinition> GetGenericRuleDefinitionsByType(string ruleTypeName)
        {
            GenericRuleTypeConfigManager configManager = new GenericRuleTypeConfigManager();
            var ruleTypeConfigId = configManager.GetGenericRuleTypeIdByName(ruleTypeName);
            Func<GenericRuleDefinition, bool> filterExpression = (item) => (item.SettingsDefinition != null && item.SettingsDefinition.ConfigId == ruleTypeConfigId);
            return GetCachedGenericRuleDefinitions().FindAllRecords(filterExpression);
        }

        public bool DoesUserHaveViewAccess(Guid genericRuleDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var genericRuleDefinition = GetGenericRuleDefinition(genericRuleDefinitionId);
            return DoesUserHaveViewAccess(userId, genericRuleDefinition);
        }
        public bool DoesUserHaveViewAccess(GenericRuleDefinition genericRuleDefinition)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId, genericRuleDefinition);
        }
        public bool DoesUserHaveViewAccess(int userId, List<Guid> RuleDefinitionIds)
        {
            foreach (var guid in RuleDefinitionIds)
            {
                var genericRuleDefinition = GetGenericRuleDefinition(guid);
                if (DoesUserHaveViewAccess(userId, genericRuleDefinition))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveViewAccess(int userId, GenericRuleDefinition genericRuleDefinition)
        {
            if (genericRuleDefinition.Security != null && genericRuleDefinition.Security.ViewRequiredPermission != null)
                return DoesUserHaveAccess(userId, genericRuleDefinition.Security.ViewRequiredPermission);
            else
                return true;
        }

        public bool DoesUserHaveAddAccess(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            if (genericRuleDefinition != null && genericRuleDefinition.Security != null && genericRuleDefinition.Security.AddRequiredPermission != null)
                return DoesUserHaveAccess(genericRuleDefinition.Security.AddRequiredPermission);
            return true;
        }
        public bool DoesUserHaveEditAccess(Guid genericRuleDefinitionId)
        {
            var genericRuleDefinition = new GenericRuleDefinitionManager().GetGenericRuleDefinition(genericRuleDefinitionId);
            if (genericRuleDefinition != null && genericRuleDefinition.Security != null && genericRuleDefinition.Security.EditRequiredPermission != null)
                return DoesUserHaveAccess(genericRuleDefinition.Security.EditRequiredPermission);
            return true;
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, GenericRuleDefinition> GetCachedGenericRuleDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericRuleDefinitions",
                () =>
                {
                    IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
                    IEnumerable<GenericRuleDefinition> genericRuleDefinitions = dataManager.GetGenericRuleDefinitions();
                    var dictionary = genericRuleDefinitions.ToDictionary(genericRuleDefinition => genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition => genericRuleDefinition);
                    return dictionary;
                });
        }

        private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        {
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericRuleDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        public GenericRuleDefinitionInfo GenericRuleDefinitionInfoMapper(GenericRuleDefinition genericRuleDefinition)
        {
            return new GenericRuleDefinitionInfo()
            {
                GenericRuleDefinitionId = genericRuleDefinition.GenericRuleDefinitionId,
                Name = genericRuleDefinition.Name
            };
        }

        #endregion
    }
}
