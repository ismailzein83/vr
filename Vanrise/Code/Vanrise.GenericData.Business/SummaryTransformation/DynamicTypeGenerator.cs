using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DynamicTypeGenerator
    {
        public static IGenericSummaryTransformer GetSummaryTransformer(int summaryTransformationDefinitionId, out SummaryTransformationDefinition summaryTransformationDefinition)
        {
            var transformationDefManager = new SummaryTransformationDefinitionManager();
            //summaryTransformationDefinition = transformationDefManager.GetSummaryTransformationDefinition(summaryTransformationDefinitionId);
            summaryTransformationDefinition = new SummaryTransformationDefinition
            {
                SummaryTransformationDefinitionId = 1,
                Name = "Traffic Summary Transformation",
                RawItemRecordTypeId = 10,
                SummaryItemRecordTypeId = 18,
                DataRecordStorageId = 9,
                BatchRangeRetrieval = new SummaryBatchTimeInterval
                {
                    RawTimeFieldName = "AttemptTime",
                    IntervalInMinutes = 10
                },
                RawTimeFieldName = "AttemptTime",
                SummaryIdFieldName = "ID",
                SummaryBatchStartFieldName = "BatchStart",
                KeyFieldMappings = new List<SummaryTransformationKeyFieldMapping>
                    {
                        new SummaryTransformationKeyFieldMapping
                        { 
                            RawFieldName= "OperatorAccount", 
                            SummaryFieldName = "OperatorID"
                        },
                        new SummaryTransformationKeyFieldMapping
                        {
                            RawFieldName = "CDPN",
                            SummaryFieldName = "CDPN"
                        }
                    },
                SummaryFromRawSettings = new UpdateSummaryFromRawSettings
                {
                    TransformationDefinitionId = 3,
                    RawRecordName = "cdr",
                    SymmaryRecordName = "trafficSummary"
                },
                UpdateExistingSummaryFromNewSettings = new UpdateExistingSummaryFromNewSettings
                {
                    TransformationDefinitionId = 4,
                    ExistingRecordName = "existingItem",
                    NewRecordName = "newItem"
                }
            };
            if (summaryTransformationDefinition == null)
                throw new NullReferenceException(String.Format("summaryTransformationDefinition {0}", summaryTransformationDefinitionId));
            String cacheName = String.Format("DynamicTypeGenerator_GetSummaryTransformer_{0}", summaryTransformationDefinitionId);
            SummaryTransformationDefinition summaryTransformationDefinition_Local = summaryTransformationDefinition;
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    StringBuilder classDefinitionBuilder = new StringBuilder(@"
                using System;                

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.Business.IGenericSummaryTransformer
                    { 
                        public DateTime GetRawItemTime(dynamic rawItem)
                        {
                            return rawItem.#RawItemTimeFieldName#;
                        }

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

                    classDefinitionBuilder.Replace("#RawItemTimeFieldName#", summaryTransformationDefinition_Local.RawTimeFieldName);
                    classDefinitionBuilder.Replace("#GetItemKeyFromRawItemImplementation#", BuildItemKeyFromRawItemImpl(summaryTransformationDefinition_Local));
                    classDefinitionBuilder.Replace("#GetItemKeyFromSummaryItemImplementation#", BuildItemKeyFromSummaryItemImpl(summaryTransformationDefinition_Local));
                    classDefinitionBuilder.Replace("#SetGroupingFieldsImplementation#", BuildSetGroupingFieldsImpl(summaryTransformationDefinition_Local));
                    classDefinitionBuilder.Replace("#SetSummaryItemFieldsToDataRecordImplementation#", BuildSetSummaryItemFieldsToDataRecordImpl(summaryTransformationDefinition_Local));
                    classDefinitionBuilder.Replace("#SetDataRecordFieldsToSummaryItemImplementation#", BuildSetDataRecordFieldsToSummaryItemImpl(summaryTransformationDefinition_Local));

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
                });
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
            foreach (var fld in keyFieldsMapping)
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

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _summaryTransformationDefinitionCacheLastCheck;
            DateTime? _recordStorageCacheLastCheck;

            protected override bool ShouldSetCacheExpired()
            {
                return
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<SummaryTransformationDefinitionManager.CacheManager>().IsCacheExpired(ref _summaryTransformationDefinitionCacheLastCheck)
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().IsCacheExpired(ref _recordStorageCacheLastCheck);
            }
        }

        #endregion
    }
}
