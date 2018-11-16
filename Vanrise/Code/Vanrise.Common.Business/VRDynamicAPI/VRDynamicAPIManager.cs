﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRDynamicAPIManager
    {


        #region Public Methods
        public IDataRetrievalResult<VRDynamicAPIDetails> GetFilteredVRDynamicAPIs(DataRetrievalInput<VRDynamicAPIQuery> input)
        {
            var allVRDynamicAPIs = GetCachedVRDynamicAPIs();
            Func<VRDynamicAPI, bool> filterExpression = (vrDynamicAPI) =>
            {
                if (input.Query.VRDynamicAPIModuleId.HasValue && !(vrDynamicAPI.ModuleId == input.Query.VRDynamicAPIModuleId))
                    return false;                
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allVRDynamicAPIs.ToBigResult(input, filterExpression, VRDynamicAPIDetailMapper));

        }

        public IEnumerable<VRDynamicAPIMethodConfig> GetVRDynamicAPIMethodSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRDynamicAPIMethodConfig>(VRDynamicAPIMethodConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<VRDynamicAPIDetails> AddVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            InsertOperationOutput<VRDynamicAPIDetails> insertOperationOutput = new InsertOperationOutput<VRDynamicAPIDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int vrDynamicAPIId = -1;

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrDynamicAPI.CreatedBy = loggedInUserId;
            vrDynamicAPI.LastModifiedBy = loggedInUserId;

            bool insertActionSuccess = vrDynamicAPIDataManager.Insert(vrDynamicAPI, out vrDynamicAPIId);
            if (insertActionSuccess)
            {
                vrDynamicAPI.VRDynamicAPIId = vrDynamicAPIId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRDynamicAPIDetailMapper(vrDynamicAPI);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public VRDynamicAPI GetVRDynamicAPIById(int vrDynamicAPIId)
        {
            var allVRDynamicAPIs = GetCachedVRDynamicAPIs();
            return allVRDynamicAPIs.GetRecord(vrDynamicAPIId);
        }

        public UpdateOperationOutput<VRDynamicAPIDetails> UpdateVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            vrDynamicAPI.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();
            UpdateOperationOutput<VRDynamicAPIDetails> updateOperationOutput = new UpdateOperationOutput<VRDynamicAPIDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = vrDynamicAPIDataManager.Update(vrDynamicAPI);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRDynamicAPIDetailMapper(vrDynamicAPI);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return vrDynamicAPIDataManager.AreVRDynamicAPIsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, VRDynamicAPI> GetCachedVRDynamicAPIs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedVRDynamicAPIs", () =>
               {
                   IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
                   List<VRDynamicAPI> vrDynamicAPIs = vrDynamicAPIDataManager.GetVRDynamicAPIs();
                   return vrDynamicAPIs.ToDictionary(vrDynamicAPI => vrDynamicAPI.VRDynamicAPIId, vrDynamicAPI => vrDynamicAPI);
               });
        }
        #endregion

        #region Mappers
        public VRDynamicAPIDetails VRDynamicAPIDetailMapper(VRDynamicAPI vrDynamicAPI)
        {
            IUserManager userManager = BEManagerFactory.GetManager<IUserManager>();
            VRDynamicAPIModuleManager vrDynamicAPIModuleManager = new VRDynamicAPIModuleManager();

            return new VRDynamicAPIDetails
            {
                Name = vrDynamicAPI.Name,
                VRDynamicAPIId = vrDynamicAPI.VRDynamicAPIId,
                ModuleName= vrDynamicAPIModuleManager.GetVRDynamicAPIModuleName((int)vrDynamicAPI.VRDynamicAPIId),
                Settings= vrDynamicAPI.Settings,
                CreatedTime = vrDynamicAPI.CreatedTime,
                CreatedByDescription = userManager.GetUserName(vrDynamicAPI.CreatedBy),
                LastModifiedTime = vrDynamicAPI.LastModifiedTime,
                LastModifiedByDescription = userManager.GetUserName(vrDynamicAPI.LastModifiedBy)
            };

        }
        #endregion
    }
}
