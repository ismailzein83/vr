using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ZooSectionManager
    {
        //#region Public Methods

        //public IDataRetrievalResult<ZooSectionDetail> GetFilteredZooSections(DataRetrievalInput<ZooSectionQuery> input)
        //{
        //    IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
        //    List<ZooSection> allZooSections = zooSectionDataManager.GetZooSections();

        //    Func<ZooSection, bool> filterExpression = (zooSection) =>
        //    {
        //        return true;
        //    };
        
        //    return DataRetrievalManager.Instance.ProcessResult(input, allZooSections.ToBigResult(input, filterExpression, ZooSectionDetailMapper));
        //}

        //public InsertOperationOutput<ZooSectionDetail> AddZooSection(ZooSection zooSection)
        //{
        //    InsertOperationOutput<ZooSectionDetail> insertOperationOutput = new InsertOperationOutput<ZooSectionDetail>();
        //    insertOperationOutput.Result = InsertOperationResult.Failed;
        //    insertOperationOutput.InsertedObject = null;
        //    long zooSectionId = -1;

        //    IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
        //    bool insertActionSuccess = zooSectionDataManager.Insert(zooSection, out zooSectionId);
        //    if(insertActionSuccess)
        //    {
        //        zooSection.ZooSectionId = zooSectionId;
        //        insertOperationOutput.Result = InsertOperationResult.Succeeded;
        //        insertOperationOutput.InsertedObject = ZooSectionDetailMapper(zooSection);
        //    }
        //    else
        //    {
        //        insertOperationOutput.Result = InsertOperationResult.SameExists;
        //    }

        //    return insertOperationOutput;
        //}

        //public UpdateOperationOutput<ZooSectionDetail> UpdateZooSection(ZooSection zooSection)
        //{
        //    UpdateOperationOutput<ZooSectionDetail> updateOperationOutput = new UpdateOperationOutput<ZooSectionDetail>();
        //    updateOperationOutput.Result = UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    IZooSectionDataManager zooSectionDataManager = DemoModuleFactory.GetDataManager<IZooSectionDataManager>();
        //    bool updateActionSuccess = zooSectionDataManager.Update(zooSection);
        //    if (updateActionSuccess)
        //    {
        //        updateOperationOutput.Result = UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = ZooSectionDetailMapper(zooSection);
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}

        //#endregion

        //#region Mappers

        //private ZooSectionDetail ZooSectionDetailMapper(ZooSection zooSection)
        //{
        //    return new ZooSectionDetail()
        //    {
        //        ZooSectionId = zooSection.ZooSectionId,
        //        Name = zooSection.Name,
        //        ZooId = zooSection.ZooId,
        //        Position = zooSection.Position,
        //    };
        //}

        //#endregion
    }
}
