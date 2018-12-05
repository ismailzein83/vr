using System;
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
    public class VRDynamicAPIModuleManager
    {

        #region Public Methods
        public IDataRetrievalResult<VRDynamicAPIModuleDetails> GetFilteredVRDynamicAPIModules(DataRetrievalInput<VRDynamicAPIModuleQuery> input)
        {
            var allVRDynamicAPIModules = GetCachedVRDynamicAPIModules();
            Func<VRDynamicAPIModule, bool> filterExpression = (vrDynamicAPIModule) =>
            {
                if (input.Query.Name != null && !vrDynamicAPIModule.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
               
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allVRDynamicAPIModules.ToBigResult(input, filterExpression, VRDynamicAPIModuleDetailMapper));

        }

        public InsertOperationOutput<VRDynamicAPIModuleDetails> AddVRDynamicAPIModule(VRDynamicAPIModule vrDynamicAPIModule)
        {
            IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
            InsertOperationOutput<VRDynamicAPIModuleDetails> insertOperationOutput = new InsertOperationOutput<VRDynamicAPIModuleDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int vrDynamicAPIModuleId = -1;

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrDynamicAPIModule.CreatedBy = loggedInUserId;
            vrDynamicAPIModule.LastModifiedBy = loggedInUserId;

            bool insertActionSuccess = vrDynamicAPIModuleDataManager.Insert(vrDynamicAPIModule, out vrDynamicAPIModuleId);
            if (insertActionSuccess)
            {
                vrDynamicAPIModule.VRDynamicAPIModuleId = vrDynamicAPIModuleId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRDynamicAPIModuleDetailMapper(vrDynamicAPIModule);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public VRDynamicAPIModule GetVRDynamicAPIModuleById(int vrDynamicAPIModuleId)
        {
            var allVRDynamicAPIModules = GetCachedVRDynamicAPIModules();
            return allVRDynamicAPIModules.GetRecord(vrDynamicAPIModuleId);
        }

        public string GetVRDynamicAPIModuleName(int vrDynamicAPIModuleID)
        {
            var vRDynamicAPIModule = GetVRDynamicAPIModuleById(vrDynamicAPIModuleID);
            if (vRDynamicAPIModule == null)
                return null;
            return vRDynamicAPIModule.Name;
        }

        public UpdateOperationOutput<VRDynamicAPIModuleDetails> UpdateVRDynamicAPIModule(VRDynamicAPIModule vrDynamicAPIModule)
        {
            IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
            vrDynamicAPIModule.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();
            UpdateOperationOutput<VRDynamicAPIModuleDetails> updateOperationOutput = new UpdateOperationOutput<VRDynamicAPIModuleDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = vrDynamicAPIModuleDataManager.Update(vrDynamicAPIModule);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRDynamicAPIModuleDetailMapper(vrDynamicAPIModule);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public Dictionary<int, VRDynamicAPIModule> GetAllVRDynamicAPIModules()
        {
            return GetCachedVRDynamicAPIModules();
        }
        public List<string> GetAllVRDynamicAPIModulesNames()
        {
            return GetCachedVRDynamicAPIModules().MapRecords(x => x.Name).ToList();
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return vrDynamicAPIModuleDataManager.AreVRDynamicAPIModulesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, VRDynamicAPIModule> GetCachedVRDynamicAPIModules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedVRDynamicAPIModules", () =>
               {
                   IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
                   List<VRDynamicAPIModule> vrDynamicAPIModules = vrDynamicAPIModuleDataManager.GetVRDynamicAPIModules();
                   return vrDynamicAPIModules.ToDictionary(vrDynamicAPIModule => vrDynamicAPIModule.VRDynamicAPIModuleId, vrDynamicAPIModule => vrDynamicAPIModule);
               });
        }

        #endregion

        #region Private Methods


        #endregion

        #region Mappers
        public VRDynamicAPIModuleDetails VRDynamicAPIModuleDetailMapper(VRDynamicAPIModule vrDynamicAPIModule)
        {
            IUserManager userManager = BEManagerFactory.GetManager<IUserManager>();

            return new VRDynamicAPIModuleDetails
            {
                Name = vrDynamicAPIModule.Name,
                VRDynamicAPIModuleId = vrDynamicAPIModule.VRDynamicAPIModuleId,
                CreatedTime=vrDynamicAPIModule.CreatedTime,
                CreatedByDescription = userManager.GetUserName(vrDynamicAPIModule.CreatedBy),
                LastModifiedTime=vrDynamicAPIModule.LastModifiedTime,
                LastModifiedByDescription=userManager.GetUserName(vrDynamicAPIModule.LastModifiedBy)
            };

        }
        #endregion
    }
}
