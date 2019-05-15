﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;


namespace Vanrise.Common.Business
{
    public class VRMailMessageTypeManager
    {
        #region Public Methods

        public VRMailMessageType GetMailMessageType(Guid vrMailMessageTypeId)
        {
            Dictionary<Guid, VRMailMessageType> cachedVRMailMessageTypes = this.GetCachedVRMailMessageTypes();
   
            return cachedVRMailMessageTypes.GetRecord(vrMailMessageTypeId);
        }
       
        public IDataRetrievalResult<VRMailMessageTypeDetail> GetFilteredMailMessageTypes(DataRetrievalInput<VRMailMessageTypeQuery> input)
        {
            var allVRMailMessageTypes = GetCachedVRMailMessageTypes();
            Func<VRMailMessageType, bool> filterExpression = (x) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(x.DevProjectId))
                    return false;

                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRMailMessageTypeLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRMailMessageTypes.ToBigResult(input, filterExpression, VRMailMessageTypeDetailMapper));
        }
        public string GetMailMessageTypeName(VRMailMessageType vrMailMessageType)
        {
            if (vrMailMessageType != null)
                return vrMailMessageType.Name;
            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<VRMailMessageTypeDetail> AddMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRMailMessageTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();

            vrMailMessageTypeItem.VRMailMessageTypeId = Guid.NewGuid();

            if (dataManager.Insert(vrMailMessageTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRMailMessageTypeLoggableEntity.Instance, vrMailMessageTypeItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRMailMessageTypeDetailMapper(vrMailMessageTypeItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail> UpdateMailMessageType(VRMailMessageType vrMailMessageTypeItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRMailMessageTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();

            if (dataManager.Update(vrMailMessageTypeItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRMailMessageTypeLoggableEntity.Instance, vrMailMessageTypeItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRMailMessageTypeDetailMapper(this.GetMailMessageType(vrMailMessageTypeItem.VRMailMessageTypeId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRMailMessageTypeInfo> GetMailMessageTypesInfo(VRMailMessageTypeFilter filter)
        {
            Func<VRMailMessageType, bool> filterExpression = (x) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(x.DevProjectId))
                    return false;

                return true;
            };

            return this.GetCachedVRMailMessageTypes().MapRecords(VRMailMessageTypeInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRMailMessageTypeDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreMailMessageTypeUpdated(ref _updateHandle);
            }
        }

        private class VRMailMessageTypeLoggableEntity : VRLoggableEntityBase
        {
            public static VRMailMessageTypeLoggableEntity Instance = new VRMailMessageTypeLoggableEntity();

            private VRMailMessageTypeLoggableEntity()
            {

            }

            static VRMailMessageTypeManager s_VRMailMessageTypeManager = new VRMailMessageTypeManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_MailMessageType"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Mail Message Type"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_MailMessageType_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRMailMessageType vrMailMessageType = context.Object.CastWithValidate<VRMailMessageType>("context.Object");
                return vrMailMessageType.VRMailMessageTypeId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRMailMessageType vrMailMessageType = context.Object.CastWithValidate<VRMailMessageType>("context.Object");
                return s_VRMailMessageTypeManager.GetMailMessageTypeName(vrMailMessageType);
            }
        }
        #endregion


        #region Private Methods

        Dictionary<Guid, VRMailMessageType> GetCachedVRMailMessageTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRMailMessageTypes",
               () =>
               {
                   IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
                   return dataManager.GetMailMessageTypes().ToDictionary(x => x.VRMailMessageTypeId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRMailMessageTypeDetail VRMailMessageTypeDetailMapper(VRMailMessageType vrMailMessageType)
        {
            VRMailMessageTypeDetail vrMailMessageTypeDetail = new VRMailMessageTypeDetail()
            {
                Entity = vrMailMessageType
            };
            return vrMailMessageTypeDetail;
        }

        public VRMailMessageTypeInfo VRMailMessageTypeInfoMapper(VRMailMessageType vrMailMessageType)
        {
            VRMailMessageTypeInfo vrMailMessageTypeInfo = new VRMailMessageTypeInfo()
            {
                VRMailMessageTypeId = vrMailMessageType.VRMailMessageTypeId,
                Name = vrMailMessageType.Name
            };
            return vrMailMessageTypeInfo;
        }

        #endregion
    }
}
