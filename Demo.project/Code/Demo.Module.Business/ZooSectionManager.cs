using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ZooSectionManager
    {
        ZooManager _zooManager = new ZooManager();

        #region Public Methods

        public IDataRetrievalResult<ZooSectionDetail> GetFilteredZooSections(DataRetrievalInput<ZooSectionQuery> input)
        {
            var allZooSections = GetCachedZooSections();
            Func<ZooSection, bool> filterExpression = (zooSection) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !zooSection.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.ZooIds != null && !input.Query.ZooIds.Contains(zooSection.ZooId))
                    return false;

                if (input.Query.MaxNbOfAnimals.HasValue && zooSection.Type != null)
                {
                    if (input.Query.MaxNbOfAnimals.Value < zooSection.Type.GetAnimalsNumber(new ZooSectionTypeGetAnimalsNumberContext()))
                        return false;
                }

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allZooSections.ToBigResult(input, filterExpression, ZooSectionDetailMapper));
        }

        public ZooSection GetZooSectionById(long zooSectionId)
        {
            var allZooSections = GetCachedZooSections();
            return allZooSections != null ? allZooSections.GetRecord(zooSectionId) : null;
        }

        public IEnumerable<ZooSectionTypeConfig> GetZooSectionTypeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ZooSectionTypeConfig>(ZooSectionTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<ZooSectionTypeAnimalConfig> GetZooSectionTypeAnimalConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ZooSectionTypeAnimalConfig>(ZooSectionTypeAnimalConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<ZooSectionDetail> AddZooSection(ZooSection zooSection)
        {
            InsertOperationOutput<ZooSectionDetail> insertOperationOutput = new InsertOperationOutput<ZooSectionDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long zooSectionId = -1;

            IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
            bool insertActionSuccess = zooSectionDataManager.Insert(zooSection, out zooSectionId);
            if (insertActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                zooSection.ZooSectionId = zooSectionId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ZooSectionDetailMapper(zooSection);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ZooSectionDetail> UpdateZooSection(ZooSection zooSection)
        {
            UpdateOperationOutput<ZooSectionDetail> updateOperationOutput = new UpdateOperationOutput<ZooSectionDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
            bool updateActionSuccess = zooSectionDataManager.Update(zooSection);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ZooSectionDetailMapper(zooSection);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<long, ZooSection> GetCachedZooSections()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedZooSections", () =>
            {
                IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
                List<ZooSection> zooSections = zooSectionDataManager.GetZooSections();
                return zooSections != null && zooSections.Count > 0 ? zooSections.ToDictionary(zooSection => zooSection.ZooSectionId, zooSection => zooSection) : null;
            });
        }

        #endregion

        #region Private Class

        private class CacheManager : BaseCacheManager
        {
            IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return zooSectionDataManager.AreZooSectionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        private ZooSectionDetail ZooSectionDetailMapper(ZooSection zooSection)
        {
            var zooSectionDetail = new ZooSectionDetail()
            {
                ZooSectionId = zooSection.ZooSectionId,
                Name = zooSection.Name,
                ZooName = _zooManager.GetZooNameById(zooSection.ZooId)
            };

            if (zooSection.Type != null)
            {
                zooSectionDetail.NbOfAnimals = zooSection.Type.GetAnimalsNumber(new ZooSectionTypeGetAnimalsNumberContext());
            }

            return zooSectionDetail;
        }

        #endregion
    }
}
