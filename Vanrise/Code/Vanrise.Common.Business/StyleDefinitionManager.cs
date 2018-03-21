using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class StyleDefinitionManager
    {
        #region Public Methods

        public StyleDefinition GetStyleDefinition(Guid styleDefinitionId)
        {
            Dictionary<Guid, StyleDefinition> cachedStyleDefinitions = this.GetCachedStyleDefinitions();
            return cachedStyleDefinitions.GetRecord(styleDefinitionId);
        }

        public IDataRetrievalResult<StyleDefinitionDetail> GetFilteredStyleDefinitions(DataRetrievalInput<StyleDefinitionQuery> input)
        {
            var allStyleDefinitions = this.GetCachedStyleDefinitions();
            Func<StyleDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(StyleDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStyleDefinitions.ToBigResult(input, filterExpression, StyleDefinitionDetailMapper));
        }

        public List<StyleDefinition> GetAllStyleDefinitions()
        {
            var allStyleDefinitions = this.GetCachedStyleDefinitions();
            return allStyleDefinitions.Select(x => x.Value).ToList();
        }
        public string GetStyleDefinitionName(StyleDefinition styleDefinition)
        {
            return (styleDefinition != null) ? styleDefinition.Name : null;

        }

        public Vanrise.Entities.InsertOperationOutput<StyleDefinitionDetail> AddStyleDefinition(StyleDefinition styleDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<StyleDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IStyleDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();

            styleDefinitionItem.StyleDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(StyleDefinitionLoggableEntity.Instance, styleDefinitionItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StyleDefinitionDetailMapper(styleDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<StyleDefinitionDetail> UpdateStyleDefinition(StyleDefinition styleDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<StyleDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStyleDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();

            if (dataManager.Update(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(StyleDefinitionLoggableEntity.Instance, styleDefinitionItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StyleDefinitionDetailMapper(this.GetStyleDefinition(styleDefinitionItem.StyleDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<StyleFormatingConfig> GetStyleFormatingExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<StyleFormatingConfig>(StyleFormatingConfig.EXTENSION_TYPE);
        }

        public IEnumerable<StyleDefinitionInfo> GetStyleDefinitionsInfo(StyleDefinitionFilter filter)
        {
            Func<StyleDefinition, bool> filterExpression = null;

            return this.GetCachedStyleDefinitions().MapRecords(StyleDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStyleDefinitionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStyleDefinitionUpdated(ref _updateHandle);
            }
        }

        private class StyleDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static StyleDefinitionLoggableEntity Instance = new StyleDefinitionLoggableEntity();

            private StyleDefinitionLoggableEntity()
            {

            }

            static StyleDefinitionManager s_styleDefinitionManager = new StyleDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_StyleDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "Style Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_StyleDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                StyleDefinition styleDefinition = context.Object.CastWithValidate<StyleDefinition>("context.Object");
                return styleDefinition.StyleDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                StyleDefinition styleDefinition = context.Object.CastWithValidate<StyleDefinition>("context.Object");
                return s_styleDefinitionManager.GetStyleDefinitionName(styleDefinition);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, StyleDefinition> GetCachedStyleDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStyleDefinitions",
               () =>
               {
                   IStyleDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();
                   return dataManager.GetStyleDefinitions().ToDictionary(x => x.StyleDefinitionId, x => x);
               });
        }

        #endregion

        #region Mappers

        public StyleDefinitionDetail StyleDefinitionDetailMapper(StyleDefinition styleDefinition)
        {
            StyleDefinitionDetail styleDefinitionDetail = new StyleDefinitionDetail()
            {
                Entity = styleDefinition
            };
            return styleDefinitionDetail;
        }

        public StyleDefinitionInfo StyleDefinitionInfoMapper(StyleDefinition styleDefinition)
        {
            StyleDefinitionInfo styleDefinitionInfo = new StyleDefinitionInfo()
            {
                StyleDefinitionId = styleDefinition.StyleDefinitionId,
                Name = styleDefinition.Name
            };
            return styleDefinitionInfo;
        }

        #endregion
    }
}
