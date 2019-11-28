﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;


namespace Vanrise.Common.Business
{
    public class VRObjectTypeDefinitionManager
    {
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #region Public Methods


        public VRObjectTypeDefinition GetVRObjectTypeDefinition(Guid styleDefinitionId)
        {
            Dictionary<Guid, VRObjectTypeDefinition> cachedVRObjectTypeDefinitions = this.GetCachedVRObjectTypeDefinitions();
 
           return cachedVRObjectTypeDefinitions.GetRecord(styleDefinitionId);
        }

       

        public string GetObjectTypeDefinitionName(VRObjectTypeDefinition vrObjectTypeDefinition)
        {
            if (vrObjectTypeDefinition != null)
               return  vrObjectTypeDefinition.Name;
            return null;
        
        }
        public IDataRetrievalResult<VRObjectTypeDefinitionDetail> GetFilteredVRObjectTypeDefinitions(DataRetrievalInput<VRObjectTypeDefinitionQuery> input)
        {
            var allVRObjectTypeDefinitions = GetCachedVRObjectTypeDefinitions();
            Func<VRObjectTypeDefinition, bool> filterExpression = (x) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(x.DevProjectId))
                    return false;

                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.DevProjectIds != null && (!x.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(x.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRObjectTypeDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRObjectTypeDefinitions.ToBigResult(input, filterExpression, VRObjectTypeDefinitionDetailMapper));

        }

        public Vanrise.Entities.InsertOperationOutput<VRObjectTypeDefinitionDetail> AddVRObjectTypeDefinition(VRObjectTypeDefinition styleDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRObjectTypeDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();

            styleDefinitionItem.VRObjectTypeDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRObjectTypeDefinitionLoggableEntity.Instance, styleDefinitionItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRObjectTypeDefinitionDetailMapper(styleDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRObjectTypeDefinitionDetail> UpdateVRObjectTypeDefinition(VRObjectTypeDefinition styleDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRObjectTypeDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();

            if (dataManager.Update(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRObjectTypeDefinitionLoggableEntity.Instance, styleDefinitionItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRObjectTypeDefinitionDetailMapper(this.GetVRObjectTypeDefinition(styleDefinitionItem.VRObjectTypeDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRObjectTypeDefinitionInfo> GetVRObjectTypeDefinitionsInfo(StyleDefinitionFilter filter)
        {
            Func<VRObjectTypeDefinition, bool> filterExpression = (x) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(x.DevProjectId))
                    return false;

                return true;
            };

            return this.GetCachedVRObjectTypeDefinitions().MapRecords(VRObjectTypeDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRObjectTypeDefinitionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRObjectTypeDefinitionUpdated(ref _updateHandle);
            }
        }

        private class VRObjectTypeDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static VRObjectTypeDefinitionLoggableEntity Instance = new VRObjectTypeDefinitionLoggableEntity();

            private VRObjectTypeDefinitionLoggableEntity()
            {

            }

            static VRObjectTypeDefinitionManager s_VRObjectTypeDefinitionManager = new VRObjectTypeDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_ObjectTypeDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Object Type Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_ObjectTypeDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRObjectTypeDefinition vrObjectTypeDefinition = context.Object.CastWithValidate<VRObjectTypeDefinition>("context.Object");
                return vrObjectTypeDefinition.VRObjectTypeDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRObjectTypeDefinition vrObjectTypeDefinition = context.Object.CastWithValidate<VRObjectTypeDefinition>("context.Object");
                return s_VRObjectTypeDefinitionManager.GetObjectTypeDefinitionName(vrObjectTypeDefinition);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, VRObjectTypeDefinition> GetCachedVRObjectTypeDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRObjectTypeDefinitions",
               () =>
               {
                   IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
                   return dataManager.GetVRObjectTypeDefinitions().ToDictionary(x => x.VRObjectTypeDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRObjectTypeDefinitionDetail VRObjectTypeDefinitionDetailMapper(VRObjectTypeDefinition styleDefinition)
        {
            VRObjectTypeDefinitionDetail styleDefinitionDetail = new VRObjectTypeDefinitionDetail()
            {
                Entity = styleDefinition
            };
            if (styleDefinition.DevProjectId.HasValue)
            {
                styleDefinitionDetail.DevProjectName = vrDevProjectManager.GetVRDevProjectName(styleDefinition.DevProjectId.Value);
            }
            return styleDefinitionDetail;
        }

        public VRObjectTypeDefinitionInfo VRObjectTypeDefinitionInfoMapper(VRObjectTypeDefinition vrObjectTypeDefinition)
        {
            VRObjectTypeDefinitionInfo vrObjectTypeDefinitionInfo = new VRObjectTypeDefinitionInfo()
            {
                VRObjectTypeDefinitionId = vrObjectTypeDefinition.VRObjectTypeDefinitionId,
                Name = vrDevProjectManager.ConcatenateTitleAndDevProjectName(vrObjectTypeDefinition.DevProjectId, vrObjectTypeDefinition.Name)
            };
            return vrObjectTypeDefinitionInfo;
        }

        #endregion
    }
}
