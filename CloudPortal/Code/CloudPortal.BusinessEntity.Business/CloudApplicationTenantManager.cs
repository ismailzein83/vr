using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationTenantManager
    {
        #region public methods
        
        public IDataRetrievalResult<CloudApplicationTenantDetail> GetFilteredCloudApplicationTenants(DataRetrievalInput<CloudApplicationTenantQuery> input)
        {
            List<CloudApplicationTenant> items = GetApplicationTenants(input.Query.ApplicationId);

            Func<CloudApplicationTenant, bool> filterExpression = (prod) => true;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, items.ToBigResult(input, filterExpression, CloudApplicationTenantDetailMapper));
        }

        public List<CloudApplicationTenant> GetApplicationTenants(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            var application = GetApplication(applicationIdentification);
            return GetApplicationTenants(application.CloudApplicationId);
        }

        public CloudApplicationTenant GetApplicationTenantById(int cloudApplicationTenantId)
        {
            var items = GetCachedAppTenants();
            return items.GetRecord(cloudApplicationTenantId);
        }

        public CloudApplicationTenant GetApplicationTenantByAppIdTenantId(int applicationId, int tenantId)
        {
            var items = GetCachedAppTenants();
            return items.FindRecord(itm => itm.Value.ApplicationId == applicationId && itm.Value.TenantId == tenantId).Value;
        }

        public bool AddApplicationTenant(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification, int tenantId, out CloudApplicationTenant appTenant)
        {
            var application = GetApplication(applicationIdentification);
            appTenant = new CloudApplicationTenant
            {
                TenantId = tenantId,
                ApplicationId = application.CloudApplicationId,
                TenantSettings = new Vanrise.Security.Entities.TenantSettings(),
                CloudTenantSettings = new CloudApplicationTenantSettings() { LicenseExpiresOn = DateTime.Now.AddDays(1) }
            };
            ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
            return dataManager.AddApplicationTenant(appTenant);
        }

        public bool UpdateApplicationTenant(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification, int tenantId, out CloudApplicationTenant appTenant)
        {
            var application = GetApplication(applicationIdentification);
            appTenant = new CloudApplicationTenant
            {
                TenantId = tenantId,
                ApplicationId = application.CloudApplicationId,
                TenantSettings = new Vanrise.Security.Entities.TenantSettings()
            };
            ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
            return dataManager.UpdateApplicationTenant(appTenant);
        }

        public Vanrise.Entities.InsertOperationOutput<CloudApplicationTenant> AssignCloudApplicationTenant(CloudApplicationTenant cloudApplicationTenant)
        {
            InsertOperationOutput<CloudApplicationTenant> insertOperationOutput = new InsertOperationOutput<CloudApplicationTenant>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
            bool insertActionSucc = dataManager.AddApplicationTenant(cloudApplicationTenant);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = cloudApplicationTenant;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<CloudApplicationTenant> UpdateCloudApplicationTenant(CloudApplicationTenant cloudApplicationTenant)
        {
            UpdateOperationOutput<CloudApplicationTenant> updateOperationOutput = new UpdateOperationOutput<CloudApplicationTenant>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;


            ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
            bool updateActionSucc = dataManager.UpdateApplicationTenant(cloudApplicationTenant);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = cloudApplicationTenant;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public List<CloudApplicationTenant> GetApplicationTenants(int applicationId)
        {
            return GetAllAppTenantsByAppId().GetRecord(applicationId);
        }
        
        #endregion

        #region private methods
       
        private static CloudApplication GetApplication(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            if (applicationIdentification == null)
                throw new ArgumentNullException("applicationIdentification");
            CloudApplicationManager cloudApplicationManager = new CloudApplicationManager();
            var application = cloudApplicationManager.GetApplicationByIdentification(applicationIdentification);
            if (application == null)
                throw new NullReferenceException(String.Format("application. IdentificationKey '{0}'", applicationIdentification.IdentificationKey));
            return application;
        }

        private Dictionary<int, CloudApplicationTenant> GetCachedAppTenants()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAppTenants",
            () =>
            {
                ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
                List<CloudApplicationTenant> allApplicationTenants = dataManager.GetAllApplicationTenants();
                if (allApplicationTenants == null)
                    return null;
                else
                {
                    return allApplicationTenants.ToDictionary(itm => itm.CloudApplicationTenantId, itm => itm);
                }
            });
        }

        private Dictionary<int, List<CloudApplicationTenant>> GetAllAppTenantsByAppId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAppTenantsByAppId",
                () =>
                {
                    ICloudApplicationTenantDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
                    List<CloudApplicationTenant> allApplicationTenants = dataManager.GetAllApplicationTenants();
                    if (allApplicationTenants == null)
                        return null;
                    else
                    {

                        Dictionary<int, List<CloudApplicationTenant>> dicAppUsers = new Dictionary<int, List<CloudApplicationTenant>>();
                        foreach (var appTenant in allApplicationTenants)
                        {
                            dicAppUsers.GetOrCreateItem(appTenant.ApplicationId).Add(appTenant);
                        }
                        return dicAppUsers;
                    }
                });
        }

        private CloudApplicationTenantDetail CloudApplicationTenantDetailMapper(CloudApplicationTenant cloudApplicationTenant)
        {
            TenantManager tenantManager = new TenantManager();
            var tenant = tenantManager.GetTenantbyId(cloudApplicationTenant.TenantId);

            return new CloudApplicationTenantDetail()
            {
                Entity = cloudApplicationTenant,
                TenantName = tenant.Name
            };
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICloudApplicationTenantDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationTenantDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreApplicationTenantsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
