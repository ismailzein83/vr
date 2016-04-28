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
        public CloudApplication GetApplicationByIdentification(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            if (applicationIdentification == null)
                throw new ArgumentNullException("applicationIdentification");
            if (applicationIdentification.IdentificationKey == null)
                throw new ArgumentNullException("applicationIdentification.IdentificationKey");
            return GetAllApplicationsByIdentification().GetRecord(applicationIdentification.IdentificationKey);
        }

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
