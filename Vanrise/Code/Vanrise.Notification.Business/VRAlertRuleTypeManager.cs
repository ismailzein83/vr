using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleTypeManager
    {
        #region Public Methods

        public VRAlertRuleType GetVRAlertRuleType(Guid vrAlertRuleTypeId)
        {
            Dictionary<Guid, VRAlertRuleType> cachedVRAlertRuleTypes = this.GetCachedVRAlertRuleTypes();
            return cachedVRAlertRuleTypes.GetRecord(vrAlertRuleTypeId);
        }

        public T GetVRAlertRuleTypeSettings<T>(Guid vrAlertRuleTypeId) where T : VRAlertRuleTypeSettings
        {
            var alertRuleType = GetVRAlertRuleType(vrAlertRuleTypeId);
            if (alertRuleType == null)
                throw new NullReferenceException(String.Format("alertRuleType '{0}'", vrAlertRuleTypeId));
            if (alertRuleType.Settings == null)
                throw new NullReferenceException(String.Format("alertRuleType.Settings '{0}'", vrAlertRuleTypeId));
            T settingsAsT = alertRuleType.Settings as T;
            if (settingsAsT == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type '{0}'. it is of type '{1}'", typeof(T), alertRuleType.Settings.GetType()));
            return settingsAsT;
        }

        public IDataRetrievalResult<VRAlertRuleTypeDetail> GetFilteredVRAlertRuleTypes(DataRetrievalInput<VRAlertRuleTypeQuery> input)
        {
            var allVRAlertRuleTypes = GetCachedVRAlertRuleTypes();
            Func<VRAlertRuleType, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRAlertRuleTypes.ToBigResult(input, filterExpression, VRAlertRuleTypeDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleTypeDetail> AddVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRAlertRuleTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();

            vrAlertRuleTypeItem.VRAlertRuleTypeId = Guid.NewGuid();

            if (dataManager.Insert(vrAlertRuleTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRAlertRuleTypeDetailMapper(vrAlertRuleTypeItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleTypeDetail> UpdateVRAlertRuleType(VRAlertRuleType vrAlertRuleTypeItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRAlertRuleTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();

            if (dataManager.Update(vrAlertRuleTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRAlertRuleTypeDetailMapper(this.GetVRAlertRuleType(vrAlertRuleTypeItem.VRAlertRuleTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRAlertRuleTypeConfig> GetVRAlertRuleTypeSettingsExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRAlertRuleTypeConfig>(VRAlertRuleTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRAlertRuleTypeInfo> GetVRAlertRuleTypesInfo(VRAlertRuleTypeFilter filter)
        {
            Func<VRAlertRuleType, bool> filterExpression = null;

            return this.GetCachedVRAlertRuleTypes().MapRecords(VRAlertRuleTypeInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRAlertRuleTypeDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRAlertRuleTypeUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, VRAlertRuleType> GetCachedVRAlertRuleTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRAlertRuleTypes",
               () =>
               {
                   IVRAlertRuleTypeDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleTypeDataManager>();
                   return dataManager.GetVRAlertRuleTypes().ToDictionary(x => x.VRAlertRuleTypeId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRAlertRuleTypeDetail VRAlertRuleTypeDetailMapper(VRAlertRuleType vrAlertRuleType)
        {
            VRAlertRuleTypeDetail vrAlertRuleTypeDetail = new VRAlertRuleTypeDetail()
            {
                Entity = vrAlertRuleType
            };
            return vrAlertRuleTypeDetail;
        }

        public VRAlertRuleTypeInfo VRAlertRuleTypeInfoMapper(VRAlertRuleType vrAlertRuleType)
        {
            VRAlertRuleTypeInfo vrAlertRuleTypeInfo = new VRAlertRuleTypeInfo()
            {
                VRAlertRuleTypeId = vrAlertRuleType.VRAlertRuleTypeId,
                Name = vrAlertRuleType.Name
            };
            return vrAlertRuleTypeInfo;
        }

        #endregion
    }
}
