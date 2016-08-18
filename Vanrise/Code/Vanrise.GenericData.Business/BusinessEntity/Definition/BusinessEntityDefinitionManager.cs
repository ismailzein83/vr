using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityDefinitionManager : IBusinessEntityDefinitionManager
    {
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<BusinessEntityDefinitionDetail> GetFilteredBusinessEntityDefinitions(Vanrise.Entities.DataRetrievalInput<BusinessEntityDefinitionQuery> input)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();

            Func<BusinessEntityDefinition, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedBEDefinitions.ToBigResult(input, filterExpression, BusinessEntityDefinitionDetailMapper));
        }
        public int GetBusinessEntityDefinitionId(string businessEntityDefinitionName)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            var businessEntityDefinition = cachedBEDefinitions.FindRecord(x=>x.Name == businessEntityDefinitionName);
            if (businessEntityDefinition == null)
                throw new NullReferenceException(String.Format("businessEntityDefinition. businessEntityDefinitionName '{0}'", businessEntityDefinitionName));
            return businessEntityDefinition.BusinessEntityDefinitionId;
        }
        public BusinessEntityDefinition GetBusinessEntityDefinition(int businessEntityDefinitionId)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            return cachedBEDefinitions.GetRecord(businessEntityDefinitionId);
        }
        public IEnumerable<BusinessEntityDefinitionInfo> GetBusinessEntityDefinitionsInfo(BusinessEntityDefinitionInfoFilter filter)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            Func<BusinessEntityDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                // Set filterExpression
            }

            return cachedBEDefinitions.MapRecords(BusinessEntityDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IBusinessEntityManager GetBusinessEntityManager(int businessEntityDefinitionId)
        {
            string cacheName = String.Format("GetBusinessEntityManager_{0}", businessEntityDefinitionId);
            Type beManagerType = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);

                    if (businessEntityDefinition == null)
                        throw new NullReferenceException("businessEntityDefinition");

                    if (businessEntityDefinition.Settings == null)
                        throw new NullReferenceException("businessEntityDefinition.Settings");

                    if (businessEntityDefinition.Settings.ManagerFQTN == null)
                        throw new NullReferenceException("businessEntityDefinition.Settings.ManagerFQTN");

                    Type beManagerType_Internal = Type.GetType(businessEntityDefinition.Settings.ManagerFQTN);

                    if (beManagerType_Internal == null)
                        throw new NullReferenceException("beManagerType_Internal");
                    return beManagerType_Internal;
                });

            var beManagerInstance = Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("'{0}' does not implement IBusinessEntityManager", beManagerType.Name));

            return beManagerInstance;
        }

        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail> UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            UpdateOperationOutput<BusinessEntityDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BusinessEntityDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateBusinessEntityDefinition(businessEntityDefinition);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = BusinessEntityDefinitionDetailMapper(businessEntityDefinition);
            }
            return updateOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail> AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            InsertOperationOutput<BusinessEntityDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BusinessEntityDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int businessEntityDefinitionId = -1;
            IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddBusinessEntityDefinition(businessEntityDefinition, out businessEntityDefinitionId);
            if (insertActionSucc)
            {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    businessEntityDefinition.BusinessEntityDefinitionId = businessEntityDefinitionId;
                    insertOperationOutput.InsertedObject = BusinessEntityDefinitionDetailMapper(businessEntityDefinition);

                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            }

            return insertOperationOutput;
        }
        public Vanrise.Security.Entities.View GetGenericBEDefinitionView(int businessEntityDefinitionId)
        {
            var viewManager = new Vanrise.Security.Business.ViewManager();
            var allViews = viewManager.GetViews();
            return allViews.FirstOrDefault(v => (v.Settings as GenericBEViewSettings) != null && (v.Settings as GenericBEViewSettings).BusinessEntityDefinitionId == businessEntityDefinitionId);
        }
        public string GetBusinessEntityDefinitionName(int businessEntityDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            return beDefinition != null ? beDefinition.Name : null;
        }
        public int? GetBEDataRecordTypeIdIfGeneric(int businessEntityDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null)
            {
                var genericBEDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
                if (genericBEDefinitionSettings != null)
                    return genericBEDefinitionSettings.DataRecordTypeId;
            }
            return null;
        }

        public bool DoesUserHaveViewAccess(int genericBEDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId, genericBEDefinitionId);
        }
        public bool DoesUserHaveViewAccess(int userId, int genericBEDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.Security != null && beDefinition.Settings.Security.ViewRequiredPermission != null)
                return DoesUserHaveAccess(userId, beDefinition.Settings.Security.ViewRequiredPermission);
            return true;
        }
        public bool DoesUserHaveAddAccess(int genericBEDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.Security != null && beDefinition.Settings.Security.AddRequiredPermission != null)
                return DoesUserHaveAccess(beDefinition.Settings.Security.AddRequiredPermission);
            return true;
        }
        public bool DoesUserHaveEditAccess(int genericBEDefinitionId)
        {
            var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.Security != null && beDefinition.Settings.Security.EditRequiredPermission != null)
                return DoesUserHaveAccess(beDefinition.Settings.Security.EditRequiredPermission);
            return true;
        }

        #endregion

        #region Private Methods
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
        private Dictionary<int, BusinessEntityDefinition> GetCachedBusinessEntityDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBusinessEntityDefinitions",
                () =>
                {
                    IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
                    IEnumerable<BusinessEntityDefinition> beDefinitions = dataManager.GetBusinessEntityDefinitions();
                    return beDefinitions.ToDictionary(beDefinition => beDefinition.BusinessEntityDefinitionId, beDefinition => beDefinition);
                });
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBusinessEntityDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        BusinessEntityDefinitionInfo BusinessEntityDefinitionInfoMapper(BusinessEntityDefinition beDefinition)
        {
            return new BusinessEntityDefinitionInfo()
            {
                BusinessEntityDefinitionId = beDefinition.BusinessEntityDefinitionId,
                Name = beDefinition.Name
            };
        }
        BusinessEntityDefinitionDetail BusinessEntityDefinitionDetailMapper(BusinessEntityDefinition beDefinition)
        {
           Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);
           bool isExtensible = typeof(ExtensibleBEManager).IsAssignableFrom(beManagerType);
            return new BusinessEntityDefinitionDetail()
            {
                Entity = beDefinition,
                IsExtensible = isExtensible
            };
        }
        
        #endregion
    }
}
