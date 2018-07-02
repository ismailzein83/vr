using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericSummaryTransformationManager<T> : Vanrise.Common.Business.SummaryTransformation.SummaryTransformationManager<dynamic, GenericSummaryItem, T>
        where T : class, ISummaryBatch<GenericSummaryItem>
    {
        public Guid SummaryTransformationDefinitionId { get; set; }

        public TempStorageInformation TempStorageInformation { get; set; }


        #region Private Properties

        IDataTransformer _dataTransformer;
        IDataTransformer DataTransformer
        {
            get
            {
                if (_dataTransformer == null)
                {
                    _dataTransformer = Transformation.Entities.BusinessManagerFactory.GetManager<IDataTransformer>();
                    if (_dataTransformer == null)
                        throw new NullReferenceException("_dataTransformer");
                }
                return _dataTransformer;
            }
        }

        SummaryTransformationDefinition _summaryTransformationDefinition;
        public SummaryTransformationDefinition SummaryTransformationDefinition
        {
            get
            {
                if (_summaryTransformationDefinition == null)
                    LoadSummaryTransformerAndDefinition();
                return _summaryTransformationDefinition;
            }
        }

        IGenericSummaryTransformer _genericSummaryTransformer;
        IGenericSummaryTransformer GenericSummaryTransformer
        {
            get
            {
                if (_genericSummaryTransformer == null)
                    LoadSummaryTransformerAndDefinition();
                return _genericSummaryTransformer;
            }
        }

        void LoadSummaryTransformerAndDefinition()
        {
            _genericSummaryTransformer = DynamicTypeGenerator.GetSummaryTransformer(this.SummaryTransformationDefinitionId, out _summaryTransformationDefinition);
            if (_genericSummaryTransformer == null)
                throw new NullReferenceException(String.Format("_genericSummaryTransformer {0}", this.SummaryTransformationDefinitionId));
            if (_summaryTransformationDefinition == null)
                throw new NullReferenceException(String.Format("_summaryTransformationDefinition {0}", this.SummaryTransformationDefinitionId));
        }

        ISummaryRecordDataManager _summaryRecordDataManager;
        ISummaryRecordDataManager SummaryRecordDataManager
        {
            get
            {
                if (_summaryRecordDataManager == null)
                {
                    GetSummaryRecordStorageDataManagerContext context = new GetSummaryRecordStorageDataManagerContext(this.SummaryTransformationDefinition, TempStorageInformation);
                    _summaryRecordDataManager = context.DataStore.Settings.GetSummaryDataRecordDataManager(context);
                    if (_summaryRecordDataManager == null)
                        throw new NullReferenceException("_summaryRecordDataManager");
                }
                return _summaryRecordDataManager;
            }
        }

        #endregion

        #region Override

        public override string UniqueTypeName
        {
            get
            {
                return String.Format("GenericSummaryTransformationManager_SummaryTransformationDefinitionId_{0}", this.SummaryTransformationDefinitionId);
            }
        }

        #endregion

        protected override IEnumerable<GenericSummaryItem> GetItemsFromDB(DateTime batchStart)
        {
            IEnumerable<dynamic> dataRecords = this.SummaryRecordDataManager.GetExistingSummaryRecords(batchStart);
            return GetSummaryItemsFromDataRecords(dataRecords);
        }

        protected override void GetRawItemBatchTimeRange(dynamic rawItem, out DateTime batchStart, out DateTime batchEnd)
        {
            var batchRangeRetrieval = this.SummaryTransformationDefinition.BatchRangeRetrieval;
            if (batchRangeRetrieval == null)
                throw new NullReferenceException("batchRangeRetrieval");
            batchRangeRetrieval.GetRawItemBatchTimeRange(rawItem, this.GenericSummaryTransformer.GetRawItemTime(rawItem), out batchStart, out batchEnd);
        }

        public override string GetSummaryItemKey(GenericSummaryItem summaryItem)
        {
            return this.GenericSummaryTransformer.GetItemKeyFromSummaryItem(summaryItem.DataRecord);
        }

        protected override void InsertItemsToDB(List<GenericSummaryItem> itemsToAdd)
        {
            var dataRecords = GetDataRecordsFromSummaryItems(itemsToAdd);
            new DataRecordStorageManager().AddDataRecords(_summaryTransformationDefinition.DataRecordStorageId, dataRecords);
        }

        protected override void UpdateItemsInDB(List<GenericSummaryItem> itemsToUpdate)
        {
            var dataRecords = GetDataRecordsFromSummaryItems(itemsToUpdate);

            List<string> fieldsToJoin = new List<string>() { _summaryTransformationDefinition.SummaryIdFieldName };

            DataRecordType summaryItemDataRecordType = new DataRecordTypeManager().GetDataRecordType(this.SummaryTransformationDefinition.SummaryItemRecordTypeId);
            summaryItemDataRecordType.ThrowIfNull("summaryItemDataRecordType");
            summaryItemDataRecordType.Fields.ThrowIfNull("summaryItemDataRecordType.Fields");
            List<string> fieldsToUpdate = summaryItemDataRecordType.Fields.FindAllRecords(itm => itm.Formula == null).Select(itm => itm.Name).ToList();

            new DataRecordStorageManager().UpdateDataRecords(_summaryTransformationDefinition.DataRecordStorageId, dataRecords, fieldsToJoin, fieldsToUpdate);
        }

        protected override GenericSummaryItem CreateSummaryItemFromRawItem(dynamic rawItem)
        {
            var summaryFromRawSettings = this.SummaryTransformationDefinition.SummaryFromRawSettings;
            if (summaryFromRawSettings == null)
                throw new NullReferenceException("summaryFromRawSettings");
            var output = this.DataTransformer.ExecuteDataTransformation(summaryFromRawSettings.TransformationDefinitionId,
                (context) =>
                {
                    context.SetRecordValue(summaryFromRawSettings.RawRecordName, rawItem);
                });
            var summaryDataRecord = output.GetRecordValue(summaryFromRawSettings.SymmaryRecordName);
            if (summaryDataRecord == null)
                throw new NullReferenceException("summaryDataRecord");
            return new GenericSummaryItem
            {
                DataRecord = summaryDataRecord
            };
        }

        public override void UpdateSummaryItemFromSummaryItem(GenericSummaryItem existingItem, GenericSummaryItem newItem)
        {
            var existingFromNewSummarySettings = this.SummaryTransformationDefinition.UpdateExistingSummaryFromNewSettings;
            if (existingFromNewSummarySettings == null)
                throw new NullReferenceException("existingFromNewSummarySettings");
            this.DataTransformer.ExecuteDataTransformation(existingFromNewSummarySettings.TransformationDefinitionId,
                (context) =>
                {
                    context.SetRecordValue(existingFromNewSummarySettings.ExistingRecordName, existingItem.DataRecord);
                    context.SetRecordValue(existingFromNewSummarySettings.NewRecordName, newItem.DataRecord);
                });
        }

        public List<dynamic> GetDataRecordsFromSummaryItems(IEnumerable<GenericSummaryItem> summaryItems)
        {
            if (summaryItems != null)
            {
                List<dynamic> dataRecords = new List<dynamic>();
                foreach (var itm in summaryItems)
                {
                    var dataRecord = GetDataRecordFromSummaryItem(itm);
                    dataRecords.Add(dataRecord);
                }
                return dataRecords;
            }
            else
                return null;
        }

        public dynamic GetDataRecordFromSummaryItem(GenericSummaryItem summaryItem)
        {
            this.GenericSummaryTransformer.SetSummaryItemFieldsToDataRecord(summaryItem);
            return summaryItem.DataRecord;
        }

        public List<GenericSummaryItem> GetSummaryItemsFromDataRecords(IEnumerable<dynamic> dataRecords)
        {
            if (dataRecords != null)
            {
                List<GenericSummaryItem> genericSummaryItems = new List<GenericSummaryItem>();
                foreach (var dr in dataRecords)
                {
                    var summaryItem = GetSummaryItemFromDataRecord(dr);
                    genericSummaryItems.Add(summaryItem);
                }
                return genericSummaryItems;
            }
            else
                return null;
        }

        public GenericSummaryItem GetSummaryItemFromDataRecord(dynamic dr)
        {
            var summaryItem = new GenericSummaryItem
            {
                DataRecord = dr
            };
            this.GenericSummaryTransformer.SetDataRecordFieldsToSummaryItem(summaryItem);
            return summaryItem;
        }
    }
}
