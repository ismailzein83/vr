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

        public StyleDefinition GetStyleDefinition(Guid StyleDefinitionId)
        {
            Dictionary<Guid, StyleDefinition> cachedStyleDefinitions = this.GetCachedStyleDefinitions();
            return cachedStyleDefinitions.GetRecord(StyleDefinitionId);
        }

        public IDataRetrievalResult<StyleDefinitionDetail> GetFilteredStyleDefinitions(DataRetrievalInput<StyleDefinitionQuery> input)
        {
            var allStyleDefinitions = GetCachedStyleDefinitions();
            Func<StyleDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStyleDefinitions.ToBigResult(input, filterExpression, StyleDefinitionDetailMapper));
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

        #endregion


        #region Private Methods

        Dictionary<Guid, StyleDefinition> GetCachedStyleDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStyleDefinition",
               () =>
               {
                   IStyleDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStyleDefinitionDataManager>();
                   return dataManager.GetStyleDefinition().ToDictionary(x => x.StyleDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        public StyleDefinitionDetail StyleDefinitionDetailMapper(StyleDefinition StyleDefinition)
        {
            StyleDefinitionDetail styleDefinitionDetail = new StyleDefinitionDetail()
            {
                Entity = StyleDefinition
            };
            return styleDefinitionDetail;
        }

        //public StyleDefinitionInfo StyleDefinitionInfoMapper(StyleDefinition StyleDefinition)
        //{
        //    StyleDefinitionInfo StyleDefinitionInfo = new StyleDefinitionInfo()
        //    {
        //        StyleDefinitionId = StyleDefinition.StyleDefinitionId,
        //        Name = StyleDefinition.Name
        //    };
        //    return StyleDefinitionInfo;
        //}

        #endregion
    }
}
