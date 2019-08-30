using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var allZooSections = GetAllZooSections();
            Func<ZooSection, bool> filterExpression = (zooSection) =>
            {
                if (input.Query.Name != null && !zooSection.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.ZooIds != null && !input.Query.ZooIds.Contains(zooSection.ZooId))
                    return false;

                if (input.Query.MaxNbOfAnimals != null && zooSection.Type != null)
                {
                    if (input.Query.MaxNbOfAnimals < zooSection.Type.GetAnimalsNumber(new ZooSectionTypeGetAnimalsNumberContext()))
                        return false;
                }

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allZooSections.ToBigResult(input, filterExpression, ZooSectionDetailMapper));
        }

        public ZooSection GetZooSectionById(long zooSectionId)
        {
            var allZooSections = GetAllZooSections();
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

        private Dictionary<long, ZooSection> GetAllZooSections()
        {
            IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
            List<ZooSection> allZooSections = zooSectionDataManager.GetZooSections();

            return allZooSections != null && allZooSections.Count > 0 ? allZooSections.ToDictionary(zooSection => zooSection.ZooSectionId, zooSection => zooSection) : null;
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
