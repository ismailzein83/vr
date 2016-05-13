using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class CloudAuthServerManager
    {
        public bool HasAuthServer()
        {
            return GetAuthServer() != null;
        }
        public CloudAuthServer GetAuthServer()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAuthServer",
                () =>
                {
                    ICloudAuthServerDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ICloudAuthServerDataManager>();
                    return dataManager.GetAuthServer();
                });
        }

        public InsertOperationOutput<CloudAuthServer> InsertCloudAuthServer(CloudAuthServer cloudAuthServer)
        {
            InsertOperationOutput<CloudAuthServer> insertOperationOutput = new InsertOperationOutput<CloudAuthServer>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            
            ICloudAuthServerDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ICloudAuthServerDataManager>();

            bool insertActionSucc = dataManager.InsertCloudAuthServer(cloudAuthServer);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<CloudAuthServer> UpdateCloudAuthServer(CloudAuthServer cloudAuthServer)
        {
            UpdateOperationOutput<CloudAuthServer> updateOperationOutput = new UpdateOperationOutput<CloudAuthServer>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;

            ICloudAuthServerDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ICloudAuthServerDataManager>();

            bool updateActionSucc = dataManager.UpdateCloudAuthServer(cloudAuthServer);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
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
