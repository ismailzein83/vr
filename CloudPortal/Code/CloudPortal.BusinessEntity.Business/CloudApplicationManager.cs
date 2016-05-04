using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationManager
    {
        #region Public Methods

        public CloudApplication GetApplicationByIdentification(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            if (applicationIdentification == null)
                throw new ArgumentNullException("applicationIdentification");
            if (applicationIdentification.IdentificationKey == null)
                throw new ArgumentNullException("applicationIdentification.IdentificationKey");
            return GetAllApplicationsByIdentification().GetRecord(applicationIdentification.IdentificationKey);
        }

        public Vanrise.Entities.InsertOperationOutput<CloudApplicationDetail> AddApplication(CloudApplicationToAdd cloudApplicationToAdd)
        {
            Vanrise.Security.Entities.CloudApplicationIdentification appIdentification = new Vanrise.Security.Entities.CloudApplicationIdentification
            {
                IdentificationKey = Guid.NewGuid().ToString()
            };
            ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            int applicationId;
            if(_dataManager.Insert(cloudApplicationToAdd, appIdentification, out applicationId))
            {
                CloudApplication cloudApplication = new CloudApplication
                {
                    CloudApplicationId = applicationId,
                    Name = cloudApplicationToAdd.Name,
                    Settings = cloudApplicationToAdd.Settings,
                    ApplicationIdentification = appIdentification
                };
                
                if (ConfigureAppAuthServer(appIdentification, cloudApplication))
                {
                    _dataManager.SetApplicationReady(applicationId);
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    CloudApplicationDetail appDetail = ApplicationDetailMapper(cloudApplication);
                    //AddCurrentUserToApplication(cloudApplication);
                    return new Vanrise.Entities.InsertOperationOutput<CloudApplicationDetail>
                    {
                        Result = Vanrise.Entities.InsertOperationResult.Succeeded,
                        InsertedObject = appDetail
                    };
                }
                else
                {
                    return new Vanrise.Entities.InsertOperationOutput<CloudApplicationDetail>
                    {
                        Result = Vanrise.Entities.InsertOperationResult.Failed,
                        Message = "Application is already connected to Cloud",
                        ShowExactMessage = true
                    };
                }
            }
            else
            {
                return new Vanrise.Entities.InsertOperationOutput<CloudApplicationDetail>
                {
                    Result = Vanrise.Entities.InsertOperationResult.SameExists
                };
            }
        }

        private void AddCurrentUserToApplication(CloudApplication cloudApplication)
        {
            CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
            var userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            appUserManager.AddUserToApplication(cloudApplication.CloudApplicationId, userId);
            appUserManager.AssignUserFullControlToApp(cloudApplication, userId);
        }

        private bool ConfigureAppAuthServer(Vanrise.Security.Entities.CloudApplicationIdentification appIdentification, CloudApplication cloudApplication)
        {
            var securityManager = new Vanrise.Security.Business.SecurityManager();
            var parameterManager = new ParameterManager();
            CloudApplicationServiceProxy appServiceProxy = new CloudApplicationServiceProxy(cloudApplication);
            var configureInput = new Vanrise.Security.Entities.ConfigureAuthServerInput
            {
                ApplicationId = cloudApplication.CloudApplicationId,
                ApplicationIdentification = appIdentification,
                AuthenticationCookieName = securityManager.GetCookieName(),
                TokenDecryptionKey = securityManager.GetTokenDecryptionKey(),
                InternalURL = parameterManager.GetCloudPortalInternalURL(),
                OnlineURL = parameterManager.GetCloudPortalOnlineURL()
            };
            var configureOutput = appServiceProxy.ConfigureAuthServer(configureInput);
            return configureOutput != null && configureOutput.Result == Vanrise.Security.Entities.ConfigureAuthServerResult.Succeeded;
        }

        private CloudApplicationDetail ApplicationDetailMapper(CloudApplication cloudApplication)
        {
            return new CloudApplicationDetail
            {
                Entity = cloudApplication
            };
        }

        #endregion

        #region Private Methods

        private Dictionary<string, CloudApplication> GetAllApplicationsByIdentification()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllApplicationsByIdentification",
                () =>
                {
                    ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
                    List<CloudApplication> allApplications = _dataManager.GetAllApplications();
                    if (allApplications == null)
                        return null;
                    else
                    {
                        Dictionary<string, CloudApplication> dicApplications = new Dictionary<string, CloudApplication>();
                        foreach (var app in allApplications)
                        {
                            if (app.ApplicationIdentification == null || app.ApplicationIdentification.IdentificationKey == null)
                                throw new NullReferenceException(String.Format("app.ApplicationIdentification. ID '{0}'", app.CloudApplicationId));
                            if (dicApplications.ContainsKey(app.ApplicationIdentification.IdentificationKey))
                                throw new Exception(String.Format("Duplicate Identification Key '{0}'", app.ApplicationIdentification.IdentificationKey));
                            dicApplications.Add(app.ApplicationIdentification.IdentificationKey, app);
                        }
                        return dicApplications;
                    }
                });
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {

            ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreApplicationsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
