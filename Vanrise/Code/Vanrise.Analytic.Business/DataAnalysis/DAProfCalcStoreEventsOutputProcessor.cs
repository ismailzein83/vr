using System;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcStoreEventsOutputProcessor : IDAProfCalcOutputRecordProcessor
    {
        //public Guid DataRecordStorageId { get; set; }
        public DAProfCalcStoreEventsOutputProcessor()
        {
            //DataRecordStorageId = new Guid("4dcc3450-a7fc-4996-aaaa-42fe67adaa2c");
        }

        public void Initialize(IDAProfCalcOutputRecordProcessorIntializeContext context)
        {

        }

        public void ProcessOutputRecords(IDAProfCalcOutputRecordProcessorProcessContext context)
        {
            //if (context.OutputRecords == null || context.OutputRecords[0].DAProfCalcExecInput == null)
            //    return;
            
            //DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
            //DataStoreManager _dataStoreManager = new DataStoreManager();
            //DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();

            //var allDataRecordTypes = dataRecordTypeManager.GetCachedDataRecordTypes();
            //var items = allDataRecordTypes.FindAllRecords(itm => itm.ExtraFieldsEvaluator != null && itm.ExtraFieldsEvaluator as DAProfCalcRecordTypeExtraFields != null);

            //foreach (var item in items)
            //{
            //    if ((item.ExtraFieldsEvaluator as DAProfCalcRecordTypeExtraFields).DataAnalysisItemDefinitionId == context.OutputRecords[0].DAProfCalcExecInput.OutputItemDefinitionId)
            //    {
            //        DataRecordStorage dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(DataRecordStorageId);
            //        if (dataRecordStorage == null)
            //            throw new NullReferenceException(String.Format("dataRecordStorage. ID '{0}'", DataRecordStorageId));

            //        var recordStorageDataManager = _dataRecordStorageManager.GetStorageDataManager(DataRecordStorageId);
            //        if (recordStorageDataManager == null)
            //            throw new NullReferenceException(String.Format("recordStorageDataManager. ID '{0}'", DataRecordStorageId));
            //        var dbApplyStream = recordStorageDataManager.InitialiazeStreamForDBApply();

            //        Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(dataRecordStorage.DataRecordTypeId);

            //        foreach (DAProfCalcOutputRecord outputRecord in context.OutputRecords)
            //        {
            //            dynamic record = Activator.CreateInstance(dataRecordRuntimeType, outputRecord.FieldValues);

            //            recordStorageDataManager.WriteRecordToStream(record as Object, dbApplyStream);
            //        }

            //        var streamReadyToApply = recordStorageDataManager.FinishDBApplyStream(dbApplyStream);
            //        recordStorageDataManager.ApplyStreamToDB(streamReadyToApply);
            //    }
            //}
        }

        public void Finalize(IDAProfCalcOutputRecordProcessorFinalizeContext context)
        {

        }
    }
}
