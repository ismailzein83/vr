using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Vanrise.Entities.InsertOperationOutput<CloudApplication> AddCloudApplication(CloudApplicationToAdd cloudApplicationToAdd)
        {
            Vanrise.Security.Entities.CloudApplicationIdentification appIdentification = new Vanrise.Security.Entities.CloudApplicationIdentification
            {
                IdentificationKey = Guid.NewGuid().ToString()
            };
            ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            int applicationId;
            if (_dataManager.AddCloudApplication(cloudApplicationToAdd, appIdentification, out applicationId))
            {
                CloudApplication cloudApplication = new CloudApplication
                {
                    CloudApplicationId = applicationId,
                    Name = cloudApplicationToAdd.Name,
                    Settings = cloudApplicationToAdd.Settings,
                    ApplicationIdentification = appIdentification
                };
                try
                {
                    if (ConfigureAppAuthServer(cloudApplication))
                    {
                        _dataManager.SetApplicationReady(applicationId);
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        return new Vanrise.Entities.InsertOperationOutput<CloudApplication>
                        {
                            Result = Vanrise.Entities.InsertOperationResult.Succeeded,
                            InsertedObject = cloudApplication
                        };
                    }
                    else
                    {
                        DeleteCloudApplication(applicationId);
                        return new Vanrise.Entities.InsertOperationOutput<CloudApplication>
                        {
                            Result = Vanrise.Entities.InsertOperationResult.Failed,
                            Message = "Application is already connected to Cloud",
                            ShowExactMessage = true
                        };
                    }
                }
                catch (Exception ex)
                {
                    DeleteCloudApplication(applicationId);
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                    return new Vanrise.Entities.InsertOperationOutput<CloudApplication>
                    {
                        Result = Vanrise.Entities.InsertOperationResult.Failed,
                        Message = "Couldn't connect to the Application",
                        ShowExactMessage = true
                    };
                }
            }
            else
            {
                return new Vanrise.Entities.InsertOperationOutput<CloudApplication>
                {
                    Result = Vanrise.Entities.InsertOperationResult.SameExists
                };
            }
        }

        public Vanrise.Entities.UpdateOperationOutput<CloudApplication> UpdateCloudApplication(CloudApplicationToUpdate cloudApplicationToUpdate)
        {
            ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            var application = GetCloudApplication(cloudApplicationToUpdate.CloudApplicationId);

            CloudApplication cloudApplication = new CloudApplication
            {
                CloudApplicationId = cloudApplicationToUpdate.CloudApplicationId,
                Name = application.Name,
                Settings = cloudApplicationToUpdate.Settings,
                ApplicationIdentification = application.ApplicationIdentification
            };
            try
            {
                if (UpdateAppAuthServer(cloudApplication))
                {
                    _dataManager.UpdateCloudApplication(cloudApplicationToUpdate);
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    return new Vanrise.Entities.UpdateOperationOutput<CloudApplication>
                    {
                        Result = Vanrise.Entities.UpdateOperationResult.Succeeded,
                        UpdatedObject = cloudApplication
                    };
                }
                else
                {
                    return new Vanrise.Entities.UpdateOperationOutput<CloudApplication>
                        {
                            Result = Vanrise.Entities.UpdateOperationResult.Failed,
                            Message = "Application not connected to Cloud",
                            ShowExactMessage = true
                        };
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
                return new Vanrise.Entities.UpdateOperationOutput<CloudApplication>
                {
                    Result = Vanrise.Entities.UpdateOperationResult.Failed,
                    Message = "Couldn't connect to the Application",
                    ShowExactMessage = true
                };

            }
        }

        public Vanrise.Entities.IDataRetrievalResult<CloudApplicationDetail> GetFilteredCloudApplications(Vanrise.Entities.DataRetrievalInput<CloudApplicationQuery> input)
        {
            var allcloudApplications = GetCachedCloudApplications();

            Func<CloudApplication, bool> filterExpression = (prod) =>
                (input.Query == null || string.IsNullOrEmpty(input.Query.Name) || string.Compare(input.Query.Name, prod.Name, true) == 0);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allcloudApplications.ToBigResult(input, filterExpression, CloudApplicationDetailMapper));
        }

        public CloudApplication GetCloudApplication(int cloudApplicationId)
        {
            return GetCachedCloudApplications().First(itm => itm.CloudApplicationId == cloudApplicationId);
        }

        public List<CloudApplication> GetCloudApplicationByUser()
        {
            var userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            
            CloudApplicationUserManager cloudUserManager = new CloudApplicationUserManager();
            var cloudUserApplications = cloudUserManager.GetUserApplications(userId);
            if (cloudUserApplications == null || cloudUserApplications.Count == 0)
                return null;

            List<CloudApplication> cloudApplications = new List<CloudApplication>();
            CloudApplicationManager cloudApplicationManager = new CloudApplicationManager();

            CloudApplicationTenantManager cloudTenantManager = new CloudApplicationTenantManager();
            foreach (CloudApplicationUser item in cloudUserApplications)
            {
                var cloudTenantApplication = cloudTenantManager.GetApplicationTenantById(item.CloudApplicationTenantID);
                cloudApplications.Add(cloudApplicationManager.GetCloudApplication(cloudTenantApplication.ApplicationId));
            }
            return cloudApplications;
        }

        #endregion

        #region Private Methods
        private void DeleteCloudApplication(int cloudApplicationId)
        {
            ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            _dataManager.DeleteCloudApplication(cloudApplicationId);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        //private void AddCurrentUserToApplication(CloudApplication cloudApplication)
        //{
        //    CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
        //    var userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
        //    appUserManager.AddUserToApplication(cloudApplication.CloudApplicationId, userId);
        //    appUserManager.AssignUserFullControlToApp(cloudApplication, userId);
        //}

        private bool UpdateAppAuthServer(CloudApplication cloudApplication)
        {
            var securityManager = new Vanrise.Security.Business.SecurityManager();
            var parameterManager = new ParameterManager();
            CloudApplicationServiceProxy appServiceProxy = new CloudApplicationServiceProxy(cloudApplication);
            var updateInput = new Vanrise.Security.Entities.UpdateAuthServerInput
            {
                ApplicationId = cloudApplication.CloudApplicationId,
                ApplicationIdentification = cloudApplication.ApplicationIdentification,
                AuthenticationCookieName = securityManager.GetCookieName(),
                TokenDecryptionKey = securityManager.GetTokenDecryptionKey(),
                InternalURL = parameterManager.GetCloudPortalInternalURL(),
                OnlineURL = parameterManager.GetCloudPortalOnlineURL()
            };
            var updateOutput = appServiceProxy.UpdateAuthServer(updateInput);
            return updateOutput != null && updateOutput.Result == Vanrise.Security.Entities.UpdateAuthServerResult.Succeeded;
        }

        private bool ConfigureAppAuthServer(CloudApplication cloudApplication)
        {
            var securityManager = new Vanrise.Security.Business.SecurityManager();
            var parameterManager = new ParameterManager();
            CloudApplicationServiceProxy appServiceProxy = new CloudApplicationServiceProxy(cloudApplication);
            var configureInput = new Vanrise.Security.Entities.ConfigureAuthServerInput
            {
                ApplicationId = cloudApplication.CloudApplicationId,
                ApplicationIdentification = cloudApplication.ApplicationIdentification,
                AuthenticationCookieName = securityManager.GetCookieName(),
                TokenDecryptionKey = securityManager.GetTokenDecryptionKey(),
                InternalURL = parameterManager.GetCloudPortalInternalURL(),
                OnlineURL = parameterManager.GetCloudPortalOnlineURL()
            };
            var configureOutput = appServiceProxy.ConfigureAuthServer(configureInput);
            return configureOutput != null && configureOutput.Result == Vanrise.Security.Entities.ConfigureAuthServerResult.Succeeded;
        }

        private CloudApplicationDetail CloudApplicationDetailMapper(CloudApplication cloudApplication)
        {
            return new CloudApplicationDetail
            {
                Entity = cloudApplication
            };
        }

        private Dictionary<string, CloudApplication> GetAllApplicationsByIdentification()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllApplicationsByIdentification",
                () =>
                {
                    ICloudApplicationDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
                    List<CloudApplication> allApplications = _dataManager.GetAllCloudApplications();
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

        private List<CloudApplication> GetCachedCloudApplications()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCloudApplications",
            () =>
            {
                ICloudApplicationDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
                return dataManager.GetAllCloudApplications();
            });
        }

        #endregion

        #region Private Classes


        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICloudApplicationDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreCloudApplicationsUpdated(ref _updateHandle);
            }
        }


        #endregion
    }
}
