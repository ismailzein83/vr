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
    public class VRDynamicAPIModuleManager
    {
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();
        #region Public Methods
        public IDataRetrievalResult<VRDynamicAPIModuleDetails> GetFilteredVRDynamicAPIModules(DataRetrievalInput<VRDynamicAPIModuleQuery> input)
        {
            var allVRDynamicAPIModules = GetCachedVRDynamicAPIModules();
            Func<VRDynamicAPIModule, bool> filterExpression = (vrDynamicAPIModule) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(vrDynamicAPIModule.DevProjectId))
                    return false;
                if (input.Query.Name != null && !vrDynamicAPIModule.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.DevProjectIds != null && (!vrDynamicAPIModule.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(vrDynamicAPIModule.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRDynamicAPIModuleLoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, allVRDynamicAPIModules.ToBigResult(input, filterExpression, VRDynamicAPIModuleDetailMapper));

        }

        public InsertOperationOutput<VRDynamicAPIModuleDetails> AddVRDynamicAPIModule(VRDynamicAPIModule vrDynamicAPIModule)
        {
            IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
            InsertOperationOutput<VRDynamicAPIModuleDetails> insertOperationOutput = new InsertOperationOutput<VRDynamicAPIModuleDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            vrDynamicAPIModule.VRDynamicAPIModuleId = Guid.NewGuid();
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrDynamicAPIModule.CreatedBy = loggedInUserId;
            vrDynamicAPIModule.LastModifiedBy = loggedInUserId;

            bool insertActionSuccess = vrDynamicAPIModuleDataManager.Insert(vrDynamicAPIModule);
            if (insertActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRDynamicAPIModuleLoggableEntity.Instance, vrDynamicAPIModule);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRDynamicAPIModuleDetailMapper(vrDynamicAPIModule);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public VRDynamicAPIModule GetVRDynamicAPIModuleById(Guid vrDynamicAPIModuleId)
        {
            var allVRDynamicAPIModules = GetCachedVRDynamicAPIModules();
            return allVRDynamicAPIModules.GetRecord(vrDynamicAPIModuleId);
        }

        public string GetVRDynamicAPIModuleName(Guid vrDynamicAPIModuleID)
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRDynamicAPIModuleLoggableEntity.Instance, vrDynamicAPIModule);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRDynamicAPIModuleDetailMapper(vrDynamicAPIModule);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public Dictionary<Guid, VRDynamicAPIModule> GetAllVRDynamicAPIModules()
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
        private Dictionary<Guid, VRDynamicAPIModule> GetCachedVRDynamicAPIModules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedVRDynamicAPIModules", () =>
               {
                   IVRDynamicAPIModuleDataManager vrDynamicAPIModuleDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIModuleDataManager>();
                   List<VRDynamicAPIModule> vrDynamicAPIModules = vrDynamicAPIModuleDataManager.GetVRDynamicAPIModules();
                   return vrDynamicAPIModules.ToDictionary(vrDynamicAPIModule => vrDynamicAPIModule.VRDynamicAPIModuleId, vrDynamicAPIModule => vrDynamicAPIModule);
               });
        }
        private class VRDynamicAPIModuleLoggableEntity : VRLoggableEntityBase
        {
            public static VRDynamicAPIModuleLoggableEntity Instance = new VRDynamicAPIModuleLoggableEntity();

            private VRDynamicAPIModuleLoggableEntity()
            {

            }

            static VRDynamicAPIModuleManager _vrDynamicAPIModuleManager = new VRDynamicAPIModuleManager();

            public override string EntityUniqueName { get { return "VR_Common_VRDynamicAPIModule"; } }

            public override string ModuleName { get { return "Common"; } }

            public override string EntityDisplayName { get { return "VRDynamicAPIModule"; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_Common_VRDynamicAPIModule_ViewHistoryItem"; } }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRDynamicAPIModule vrDynamicAPIModule = context.Object.CastWithValidate<VRDynamicAPIModule>("context.Object");
                return vrDynamicAPIModule.VRDynamicAPIModuleId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRDynamicAPIModule vrDynamicAPIModule = context.Object.CastWithValidate<VRDynamicAPIModule>("context.Object");
                return _vrDynamicAPIModuleManager.GetVRDynamicAPIModuleName(vrDynamicAPIModule.VRDynamicAPIModuleId);
            }
        }


        #endregion

        #region Private Methods


        #endregion

        #region Mappers
        public VRDynamicAPIModuleDetails VRDynamicAPIModuleDetailMapper(VRDynamicAPIModule vrDynamicAPIModule)
        {
            IUserManager userManager = BEManagerFactory.GetManager<IUserManager>();
            string devProjectName = null;
            if (vrDynamicAPIModule.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(vrDynamicAPIModule.DevProjectId.Value);
            }
            return new VRDynamicAPIModuleDetails
            {
                Name = vrDynamicAPIModule.Name,
                VRDynamicAPIModuleId = vrDynamicAPIModule.VRDynamicAPIModuleId,
                DevProjectId= vrDynamicAPIModule.DevProjectId,
                DevProjectName = devProjectName,
                CreatedTime=vrDynamicAPIModule.CreatedTime,
                CreatedByDescription = userManager.GetUserName(vrDynamicAPIModule.CreatedBy),
                LastModifiedTime=vrDynamicAPIModule.LastModifiedTime,
                LastModifiedByDescription=userManager.GetUserName(vrDynamicAPIModule.LastModifiedBy)
            };

        }
        #endregion
    }
}
