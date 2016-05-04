using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class CloudAuthServerManager
    {        
        public CloudAuthServer GetAuthServer()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAuthServer",
                () =>
                {
                    ICloudAuthServerDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ICloudAuthServerDataManager>();
                    return dataManager.GetAuthServer();
                });
            //return null;
            //if (System.Web.HttpContext.Current.Request.Url.Port != 8787)
            //{
            //    return new CloudAuthServer
            //    {
            //        ApplicationIdentification = new CloudApplicationIdentification
            //        {
            //            IdentificationKey = "Application 1"
            //        },
            //        Settings = new CloudAuthServerSettings
            //        {
            //            AuthenticationCookieName = "Cloud-AuthServer-CookieName",
            //            InternalURL = "http://localhost:8787",
            //            OnlineURL = "http://localhost:8787",
            //            TokenDecryptionKey = "CloudSecretKey",
            //            CurrentApplicationId = 2
            //        }
            //    };
            //}
            //else
            //    return null;
        }

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICloudAuthServerDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<ICloudAuthServerDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.IsAuthServerUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
