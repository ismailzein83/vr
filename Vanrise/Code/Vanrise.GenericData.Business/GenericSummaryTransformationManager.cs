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
        public int SummaryTransformationDefinitionId { get; set; }
        
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
                if(_summaryRecordDataManager == null)
                {
                    GetSummaryRecordStorageDataManagerContext context = new GetSummaryRecordStorageDataManagerContext(this.SummaryTransformationDefinition);
                    _summaryRecordDataManager = context.DataStore.Settings.GetSummaryDataRecordDataManager(context);
                    if (_summaryRecordDataManager == null)
                        throw new NullReferenceException("_summaryRecordDataManager");
                }
                return _summaryRecordDataManager;
            }
        }

        #endregion

        protected override IEnumerable<GenericSummaryItem> GetItemsFromDB(DateTime batchStart)
        {
            IEnumerable<dynamic> dataRecords = this.SummaryRecordDataManager.GetExistingSummaryRecords(batchStart);
            return GetSummaryItemsFromDataRecords(dataRecords);
        }

        protected override void GetRawItemBatchTimeRange(dynamic rawItem, out DateTime batchStart)
        {
            var batchRangeRetrieval = this.SummaryTransformationDefinition.BatchRangeRetrieval;
            if (batchRangeRetrieval == null)
                throw new NullReferenceException("batchRangeRetrieval");
            batchRangeRetrieval.GetRawItemBatchTimeRange(rawItem, out batchStart);
        }

        protected override string GetSummaryItemKey(GenericSummaryItem summaryItem)
        {
            return this.GenericSummaryTransformer.GetItemKeyFromSummaryItem(summaryItem.DataRecord);
        }

        protected override string GetSummaryItemKey(dynamic rawItem)
        {
            return this.GenericSummaryTransformer.GetItemKeyFromRawItem(rawItem);
        }

        protected override void InsertItemsToDB(List<GenericSummaryItem> itemsToAdd)
        {
            var dataRecords = GetDataRecordsFromSummaryItems(itemsToAdd);
            this.SummaryRecordDataManager.InsertSummaryRecords(dataRecords);
        }

        protected override void SetSummaryItemGroupingFields(GenericSummaryItem summaryItem, dynamic rawItem)
        {
            this.GenericSummaryTransformer.SetGroupingFields(summaryItem.DataRecord, rawItem);
        }

        protected override void UpdateItemsInDB(List<GenericSummaryItem> itemsToUpdate)
        {
            var dataRecords = GetDataRecordsFromSummaryItems(itemsToUpdate);
            this.SummaryRecordDataManager.UpdateSummaryRecords(dataRecords);
        }

        protected override void UpdateSummaryItemFromRawItem(GenericSummaryItem summaryItem, dynamic rawItem)
        {
            var summaryFromRawSettings = this.SummaryTransformationDefinition.SummaryFromRawSettings;
            if (summaryFromRawSettings == null)
                throw new NullReferenceException("summaryFromRawSettings");
            this.DataTransformer.ExecuteDataTransformation(summaryFromRawSettings.TransformationDefinitionId,
                (context) =>
                {
                    context.SetRecordValue(summaryFromRawSettings.RawRecordName, rawItem);
                    context.SetRecordValue(summaryFromRawSettings.SymmaryRecordName, summaryItem.DataRecord);
                });
        }

        protected override void UpdateSummaryItemFromSummaryItem(GenericSummaryItem existingItem, GenericSummaryItem newItem)
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
                    this.GenericSummaryTransformer.SetSummaryItemFieldsToDataRecord(itm);
                    dataRecords.Add(itm.DataRecord);
                }
                return dataRecords;
            }
            else
                return null;
        }

        public List<GenericSummaryItem> GetSummaryItemsFromDataRecords(IEnumerable<dynamic> dataRecords)
        {
            if (dataRecords != null)
            {
                List<GenericSummaryItem> genericSummaryItems = new List<GenericSummaryItem>();
                foreach (var dr in dataRecords)
                {
                    var summaryItem = new GenericSummaryItem
                    {
                        DataRecord = dr
                    };
                    this.GenericSummaryTransformer.SetDataRecordFieldsToSummaryItem(summaryItem);
                    genericSummaryItems.Add(summaryItem);
                }
                return genericSummaryItems;
            }
            else
                return null;
        }
    }

    public interface IGenericSummaryTransformer
    {
        string GetItemKeyFromRawItem(dynamic rawItem);

        string GetItemKeyFromSummaryItem(dynamic summaryItem);

        void SetGroupingFields(dynamic summaryItem, dynamic rawItem);

        void SetSummaryItemFieldsToDataRecord(GenericSummaryItem summaryItem);

        void SetDataRecordFieldsToSummaryItem(GenericSummaryItem summaryItem);
    }
    public class DynamicTypeGenerator
    {
        public static IGenericSummaryTransformer GetSummaryTransformer(int summaryTransformationDefinitionId, out SummaryTransformationDefinition summaryTransformationDefinition)
        {
            var transformationDefManager = new GenericSummaryTransformationDefinitionManager();
            summaryTransformationDefinition = transformationDefManager.GetSummaryTransformationDefinition(summaryTransformationDefinitionId);
            if (summaryTransformationDefinition == null)
                throw new NullReferenceException(String.Format("summaryTransformationDefinition {0}", summaryTransformationDefinitionId));

            StringBuilder classDefinitionBuilder = new StringBuilder(@"
                using System;                

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.Business.IGenericSummaryTransformer
                    { 
                        public string GetItemKeyFromRawItem(dynamic rawItem)
                        {
                            #GetItemKeyFromRawItemImplementation#
                        }

                        public string GetItemKeyFromSummaryItem(dynamic summaryItem)
                        {
                            #GetItemKeyFromSummaryItemImplementation#                            
                        }

                        public void SetGroupingFields(dynamic summaryItem, dynamic rawItem)
                        {
                            #SetGroupingFieldsImplementation#                            
                        }

                        public void SetSummaryItemFieldsToDataRecord(Vanrise.GenericData.Entities.GenericSummaryItem summaryItem)
                        {
                            #SetSummaryItemFieldsToDataRecordImplementation#                            
                        }

                        public void SetDataRecordFieldsToSummaryItem(Vanrise.GenericData.Entities.GenericSummaryItem summaryItem)
                        {
                            #SetDataRecordFieldsToSummaryItemImplementation#                            
                        }
                    }
                }");

            classDefinitionBuilder.Replace("#GetItemKeyFromRawItemImplementation#", BuildItemKeyFromRawItemImpl(summaryTransformationDefinition));
            classDefinitionBuilder.Replace("#GetItemKeyFromSummaryItemImplementation#", BuildItemKeyFromSummaryItemImpl(summaryTransformationDefinition));
            classDefinitionBuilder.Replace("#SetGroupingFieldsImplementation#", BuildSetGroupingFieldsImpl(summaryTransformationDefinition));
            classDefinitionBuilder.Replace("#SetSummaryItemFieldsToDataRecordImplementation#", BuildSetSummaryItemFieldsToDataRecordImpl(summaryTransformationDefinition));
            classDefinitionBuilder.Replace("#SetDataRecordFieldsToSummaryItemImplementation#", BuildSetDataRecordFieldsToSummaryItemImpl(summaryTransformationDefinition));

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.Business");
            string className = "GenericSummaryTransformer";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinitionBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }
                throw new Exception(String.Format("Compile Error when building GenericSummaryTransformer for Summary Transformation Definition Id'{0}'. Errors: {1}",
                    summaryTransformationDefinitionId, errorsBuilder));
            }
            else
                return Activator.CreateInstance(compilationOutput.OutputAssembly.GetType(fullTypeName)) as IGenericSummaryTransformer;
        }

        private static string BuildItemKeyFromRawItemImpl(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            return BuildItemKeyImpl(summaryTransformationDefinition, "rawItem", (fld) => fld.RawFieldName);
        }

        private static string BuildItemKeyFromSummaryItemImpl(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            return BuildItemKeyImpl(summaryTransformationDefinition, "summaryItem", (fld) => fld.SummaryFieldName);
        }

        private static string BuildItemKeyImpl(SummaryTransformationDefinition summaryTransformationDefinition, string itemVariableName, Func<SummaryTransformationKeyFieldMapping, string> getFieldName)
        {
            var keyFieldsMapping = summaryTransformationDefinition.KeyFieldMappings;
            if (keyFieldsMapping == null)
                throw new NullReferenceException("keyFieldsMapping");
            StringBuilder formatBuilder = new StringBuilder();
            StringBuilder fldValuesBuilder = new StringBuilder();
            int columnIndex = 0;
            foreach (var fld in keyFieldsMapping)
            {
                if (columnIndex > 0)
                    formatBuilder.Append("^*^");
                formatBuilder.Append("{" + columnIndex.ToString() + "}");
                fldValuesBuilder.AppendFormat(", {0}.{1}", itemVariableName, getFieldName(fld));
            }
            return String.Format(@"return String.Format(""{0}""{1});", formatBuilder, fldValuesBuilder);
        }

        private static string BuildSetGroupingFieldsImpl(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            var keyFieldsMapping = summaryTransformationDefinition.KeyFieldMappings;
            if (keyFieldsMapping == null)
                throw new NullReferenceException("keyFieldsMapping");
            StringBuilder builder = new StringBuilder();
            foreach(var fld in keyFieldsMapping)
            {
                builder.AppendLine(String.Format("summaryItem.{0} = rawItem.{1};", fld.SummaryFieldName, fld.RawFieldName));
            }
            return builder.ToString();
        }

        private static string BuildSetSummaryItemFieldsToDataRecordImpl(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            return String.Format(@" if(summaryItem.DataRecord == null) throw new NullReferenceException(""summaryItem.DataRecord"");
                                    summaryItem.DataRecord.{0} = summaryItem.SummaryItemId;
                                    summaryItem.DataRecord.{1} = summaryItem.BatchStart;"
                , summaryTransformationDefinition.SummaryIdFieldName, summaryTransformationDefinition.SummaryBatchStartFieldName);
        }

        private static string BuildSetDataRecordFieldsToSummaryItemImpl(SummaryTransformationDefinition summaryTransformationDefinition)
        {
            return String.Format(@" if(summaryItem.DataRecord == null) throw new NullReferenceException(""summaryItem.DataRecord"");
                                    summaryItem.SummaryItemId = summaryItem.DataRecord.{0};
                                    summaryItem.BatchStart = summaryItem.DataRecord.{1};"
               , summaryTransformationDefinition.SummaryIdFieldName, summaryTransformationDefinition.SummaryBatchStartFieldName);
        }
    }

    public class SummaryBatchTimeInterval : SummaryTransformationBatchRangeRetrieval
    {
        public string RawTimeFieldName { get; set; }

        public int IntervalInMinutes { get; set; }

        public override void GetRawItemBatchTimeRange(dynamic rawItem, out DateTime batchStart)
        {
            DateTime time = rawItem.AttemptTime;//[RawTimeFieldName];
            batchStart = new DateTime(time.Year, time.Month, time.Day, time.Hour, ((int)(time.Minute / this.IntervalInMinutes)) * this.IntervalInMinutes, 0);
        }
    }
}
