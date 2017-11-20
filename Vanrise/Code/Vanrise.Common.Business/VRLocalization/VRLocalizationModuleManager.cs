using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class VRLocalizationModuleManager
    {
        public IDataRetrievalResult<VRLocalizationModuleDetail> GetFilteredVRLocalizationModules(DataRetrievalInput<VRLocalizationModuleQuery> input)
        {
            var allVRLocalizationModules = GetCachedVRLocalizationModules();
            Func<VRLocalizationModule, bool> filterExpression = (item) =>
            {
                if (input.Query.Name != null && !item.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationModules.ToBigResult(input, filterExpression, VRLocalizationModuleDetailMapper));
        }

        public VRLocalizationModule GetVRLocalizationModule(Guid vrLocalizationModuleId)
        {
            var vrLocalizationModules = GetCachedVRLocalizationModules();
            if (vrLocalizationModules == null)
                return null;
            return vrLocalizationModules.GetRecord(vrLocalizationModuleId);
        }
        public string GetVRModuleName(Guid ModuleId)
        {
            var module = GetVRLocalizationModule(ModuleId);
            if (module == null)
                return null;
            return module.Name;
        }
        public InsertOperationOutput<VRLocalizationModuleDetail> AddVRLocalizationModule(VRLocalizationModule vrLocalizationModule)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationModuleDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRLocalizationModuleDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationModuleDataManager>();

            vrLocalizationModule.VRLocalizationModuleId = Guid.NewGuid();

            if (dataManager.Insert(vrLocalizationModule))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRLocalizationModuleDetailMapper(vrLocalizationModule);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<VRLocalizationModuleDetail> UpdateVRLocalizationModule(VRLocalizationModule vrLocalizationModule)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationModuleDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRLocalizationModuleDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationModuleDataManager>();

            if (dataManager.Update(vrLocalizationModule))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRLocalizationModuleDetailMapper(this.GetVRLocalizationModule(vrLocalizationModule.VRLocalizationModuleId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Dictionary<Guid, VRLocalizationModule> GetAllModules()
        {
            return GetCachedVRLocalizationModules();
        }

        private Dictionary<Guid, VRLocalizationModule> GetCachedVRLocalizationModules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRLocalizationModules",
               () =>
               {
                   IVRLocalizationModuleDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationModuleDataManager>();
                   IEnumerable<VRLocalizationModule> vrLocalizationModules = dataManager.GetVRLocalizationModules();
                   return vrLocalizationModules.ToDictionary(itm => itm.VRLocalizationModuleId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRLocalizationModuleDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationModuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRLocalizationModulesUpdated(ref _updateHandle);
            }
        }

        VRLocalizationModuleDetail VRLocalizationModuleDetailMapper(VRLocalizationModule localizationModule)
        {

            VRLocalizationModuleDetail vrLocalizationModuleDetail = new Entities.VRLocalizationModuleDetail
            {
                VRLocalizationModuleId = localizationModule.VRLocalizationModuleId,
                Name = localizationModule.Name
            };
            return vrLocalizationModuleDetail;
        }

        public IEnumerable<VRLocalizationModuleInfo> GetVRLocalizationModulesInfo(VRLocalizationModuleInfoFilter filter)
        {
            Func<VRLocalizationModule, bool> filterExpression = (item) =>
            {
                return true;
            };
            return this.GetCachedVRLocalizationModules().MapRecords(VRLocalizationModuleInfoMapper, filterExpression).OrderBy(item => item.Name);
        }

        public VRLocalizationModuleInfo VRLocalizationModuleInfoMapper(VRLocalizationModule vrLocalizationModule)
        {
            VRLocalizationModuleInfo vrLocalizationModuleInfo = new VRLocalizationModuleInfo()
            {
                LocalizationModuleId = vrLocalizationModule.VRLocalizationModuleId,
                Name = vrLocalizationModule.Name
            };
            return vrLocalizationModuleInfo;
        }
    }
}
