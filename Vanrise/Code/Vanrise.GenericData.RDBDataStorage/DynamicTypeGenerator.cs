using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.RDBDataStorage
{
    internal class DynamicTypeGenerator
    {
        public IDynamicManager GetDynamicManager(DataRecordStorage dataRecordStorage, RDBDataRecordStorageSettings dataRecordStorageSettings)
        {
            String cacheName = String.Format("RDBDataStorage_DynamicTypeGenerator_GetBulkInsertWriter_{0}", dataRecordStorage.DataRecordStorageId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var recordTypeManager = new DataRecordTypeManager();
                    var recordType = recordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
                    if (recordType == null)
                        throw new NullReferenceException(String.Format("recordType ID {0}", dataRecordStorage.DataRecordTypeId));
                    if (recordType.Fields == null)
                        throw new NullReferenceException(String.Format("recordType.Fields ID {0}", dataRecordStorage.DataRecordTypeId));

                    StringBuilder classDefinitionBuilder = new StringBuilder(@"
                using System;                
                using System.Data;
                using System.Linq;
                using System.Collections.Generic;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.RDBDataStorage.IDynamicManager
                    {   
                        static Guid s_recordTypeId = new Guid(""#RECORDTYPEID#"");
                        #RECORDTYPEFIELDTYPES#

                        static #CLASSNAME#()
                        {
                            LoadRecordTypeFieldTypes();
                        }

                        static void LoadRecordTypeFieldTypes()
                        {
                            #FillRECORDTYPEFIELDTYPES#
                        }

                        public void WriteFieldsToRecordStream(dynamic dynamicRecord, Vanrise.Data.RDB.RDBBulkInsertQueryWriteRecordContext bulkInsertRecordContext)
                        {
                            #DATARECORD_RUNTIMETYPE# record = Vanrise.Common.ExtensionMethods.CastWithValidate<#DATARECORD_RUNTIMETYPE#>(dynamicRecord, ""dynamicRecord"");
                            #SetRDBBulkInsertColumnsFromRecordImplementation#
                        }

                        public void SetRDBInsertColumnsToTempTableFromRecord(dynamic dynamicRecord, Vanrise.Data.RDB.RDBInsertMultipleRowsQueryRowContext tempTableRowContext)
                        {
                            #DATARECORD_RUNTIMETYPE# record = Vanrise.Common.ExtensionMethods.CastWithValidate<#DATARECORD_RUNTIMETYPE#>(dynamicRecord, ""dynamicRecord"");
                            #SetRDBInsertColumnsToTempTableFromRecordImplementation#
                        }

                        public dynamic GetDynamicRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader)
                        {
                            #DATARECORD_RUNTIMETYPE# dataRecord = new #DATARECORD_RUNTIMETYPE#();
                            #DynamicRecordMapperImplementation#
                            return dataRecord;
                        }

                        public Vanrise.GenericData.Entities.DataRecord GetDataRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader, List<string> fieldNames)
                        {
                            var fieldValues = new Dictionary<string, Object>();
                            var dataRecord = new Vanrise.GenericData.Entities.DataRecord { FieldValues = fieldValues };
                            #DataRecordMapperImplementation#
                            return dataRecord;
                        }
                    }
                }");

                    StringBuilder recordFieldTypesBuilder = new StringBuilder();
                    StringBuilder fillRecordTypeFieldTypesBuilder = new StringBuilder();

                    fillRecordTypeFieldTypesBuilder.AppendLine("var recordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();");
                    fillRecordTypeFieldTypesBuilder.AppendLine("var recordType = recordTypeManager.GetDataRecordType(s_recordTypeId);");

                    foreach (DataRecordField dataRecordField in recordType.Fields)
                    {
                        recordFieldTypesBuilder.AppendLine(string.Format("static Vanrise.GenericData.Entities.DataRecordFieldType s_{0}FieldType;", dataRecordField.Name));
                        fillRecordTypeFieldTypesBuilder.AppendLine(string.Format(@"s_{0}FieldType = recordType.Fields.First(itm => itm.Name == ""{0}"").Type;", dataRecordField.Name));
                    }

                    classDefinitionBuilder.Replace("#RECORDTYPEID#", recordType.DataRecordTypeId.ToString());
                    classDefinitionBuilder.Replace("#RECORDTYPEFIELDTYPES#", recordFieldTypesBuilder.ToString());
                    classDefinitionBuilder.Replace("#FillRECORDTYPEFIELDTYPES#", fillRecordTypeFieldTypesBuilder.ToString());
                    
                    StringBuilder setRDBBulkInsertColumnsValuesBuilder = new StringBuilder();
                    StringBuilder setRDBInsertColumnsToTempTableFromRecordBuilder = new StringBuilder();


                    foreach (var columnSettings in dataRecordStorageSettings.Columns)
                    {
                        var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == columnSettings.FieldName);
                        matchField.ThrowIfNull("matchField", columnSettings.FieldName);
                        if (!columnSettings.IsIdentity)
                            AppendSetRDBValueFromRecordField(setRDBBulkInsertColumnsValuesBuilder, matchField, "bulkInsertRecordContext");
                        AppendSetRDBValueFromRecordField(setRDBInsertColumnsToTempTableFromRecordBuilder, matchField, $@"tempTableRowContext.Column(""{matchField.Name}"")");
                    }

                    if (dataRecordStorageSettings.IncludeQueueItemId)
                    {
                        AppendSetRDBValueFromRecordQueueItemId(setRDBBulkInsertColumnsValuesBuilder, "bulkInsertRecordContext");
                        AppendSetRDBValueFromRecordQueueItemId(setRDBInsertColumnsToTempTableFromRecordBuilder, @"tempTableRowContext.Column(""QueueItemId"")");                        
                    }

                    classDefinitionBuilder.Replace("#SetRDBBulkInsertColumnsFromRecordImplementation#", setRDBBulkInsertColumnsValuesBuilder.ToString());
                    classDefinitionBuilder.Replace("#SetRDBInsertColumnsToTempTableFromRecordImplementation#", setRDBInsertColumnsToTempTableFromRecordBuilder.ToString());

                    Type dataRecordRuntimeType = recordTypeManager.GetDataRecordRuntimeType(dataRecordStorage.DataRecordTypeId);
                    dataRecordRuntimeType.ThrowIfNull("dataRecordRuntimeType", dataRecordStorage.DataRecordTypeId);
                    classDefinitionBuilder.Replace("#DATARECORD_RUNTIMETYPE#", CSharpCompiler.TypeToString(dataRecordRuntimeType));

                    string dynamicRecordMapper;
                    string dataRecordMapper;
                    BuildMapperImplementations(dataRecordStorageSettings, recordType, out dynamicRecordMapper, out dataRecordMapper);
                    classDefinitionBuilder.Replace("#DynamicRecordMapperImplementation#", dynamicRecordMapper);
                    classDefinitionBuilder.Replace("#DataRecordMapperImplementation#", dataRecordMapper);

                    string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.RDBDataStorage");
                    string className = "DynamicManager";
                    classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                    classDefinitionBuilder.Replace("#CLASSNAME#", className);
                    string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

                    CSharpCompilationOutput compilationOutput;
                    if (!CSharpCompiler.TryCompileClass(String.Format("RDBRecordStorage_{0}", dataRecordStorage.Name), classDefinitionBuilder.ToString(), out compilationOutput))
                    {
                        StringBuilder errorsBuilder = new StringBuilder();
                        if (compilationOutput.ErrorMessages != null)
                        {
                            foreach (var errorMessage in compilationOutput.ErrorMessages)
                            {
                                errorsBuilder.AppendLine(errorMessage);
                            }
                        }
                        throw new Exception(String.Format("Compile Error when building BulkInsertWriter for record Storage Id'{0}'. Errors: {1}",
                            dataRecordStorage.DataRecordStorageId, errorsBuilder));
                    }
                    else
                    {
                        return Activator.CreateInstance(compilationOutput.OutputAssembly.GetType(fullTypeName)).CastWithValidate<IDynamicManager>("dynamicManager");
                    }
                });
        }

        private static void AppendSetRDBValueFromRecordField(StringBuilder rdbColumnsValuesBuilder, DataRecordField matchField, string rdbContextName)
        {
            string fieldName = matchField.Name;
            if (matchField.Type.StoreValueSerialized)
            {
                rdbColumnsValuesBuilder.AppendLine($"var {fieldName}Context = new Vanrise.GenericData.Entities.SerializeDataRecordFieldValueContext() {{ Object = record.{fieldName} }}; ");
                rdbColumnsValuesBuilder.AppendLine($"{rdbContextName}.Value(s_{fieldName}FieldType.SerializeValue({fieldName}Context));");
            }
            else
            {
                var runtimeType = matchField.Type.GetRuntimeType();
                if (runtimeType.IsValueType && Nullable.GetUnderlyingType(runtimeType) != null)//is nullable Type
                {
                    rdbColumnsValuesBuilder.AppendLine(
                        $@" if(record.{fieldName}.HasValue)
                                {rdbContextName}.Value(record.{fieldName}.Value);
                            else
                                {rdbContextName}.Null();");
                }
                else
                {
                    rdbColumnsValuesBuilder.AppendLine($"{rdbContextName}.Value(record.{fieldName});");
                }
            }
        }

        private static void AppendSetRDBValueFromRecordQueueItemId(StringBuilder setColumnsValuesBuilder, string rdbContextName)
        {
            setColumnsValuesBuilder.AppendLine(
                                                $@" if(record.QueueItemId != null && record.QueueItemId != default(long))
                                                        {rdbContextName}.Value(record.QueueItemId);
                                                    else
                                                        {rdbContextName}.Null();");
        }

        private void BuildMapperImplementations(RDBDataRecordStorageSettings dataRecordStorageSettings, 
            DataRecordType recordType, out string dynamicRecordMapper, out string dataRecordMapper)
        {
            
            StringBuilder dynamicRecordMapperBuilder = new StringBuilder();
            StringBuilder dataRecordMapperBuilder = new StringBuilder();
            foreach (var column in dataRecordStorageSettings.Columns)
            {
                var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == column.FieldName);
                if (matchField == null)
                    throw new NullReferenceException("matchField");
                dataRecordMapperBuilder.AppendLine($@"if(fieldNames == null || fieldNames.Contains(""{column.FieldName}""))");
                dataRecordMapperBuilder.AppendLine("{");
                string valueExpression;
                bool isNullableField = false;
                if (!matchField.Type.StoreValueSerialized)
                {
                    var runtimeType = matchField.Type.GetRuntimeType();
                    isNullableField = runtimeType.IsValueType && Nullable.GetUnderlyingType(runtimeType) != null;
                    string getReaderValueMethodName = RDBUtilities.GetGetReaderValueMethodNameWithValidate(runtimeType);
                    valueExpression = string.Format(@"reader.{0}(""{1}"")", getReaderValueMethodName, column.FieldName);
                }
                else
                {
                    StringBuilder readDeserializedValueBuilder = new StringBuilder();
                    readDeserializedValueBuilder.AppendLine(String.Format(@"var {0}Context = new Vanrise.GenericData.Entities.DeserializeDataRecordFieldValueContext() {{ Value = reader.GetString(""{0}"")}}; ", column.FieldName));
                    readDeserializedValueBuilder.AppendLine(String.Format(@"var {0}DeserializedValue = s_{0}FieldType.DeserializeValue({0}Context);", matchField.Name));
                    dynamicRecordMapperBuilder.AppendLine(readDeserializedValueBuilder.ToString());
                    dataRecordMapperBuilder.AppendLine(readDeserializedValueBuilder.ToString());
                    valueExpression = String.Format(@"{0}DeserializedValue != null? ({1}){0}DeserializedValue : null", matchField.Name, CSharpCompiler.TypeToString(matchField.Type.GetNonNullableRuntimeType()));                    
                }
                string fieldValueVariableName = $"{matchField.Name}_FieldValue";
                dynamicRecordMapperBuilder.AppendLine($"var {fieldValueVariableName} = {valueExpression};");
                dynamicRecordMapperBuilder.AppendLine($@"dataRecord.{matchField.Name} = {fieldValueVariableName};");
                dataRecordMapperBuilder.AppendLine($"var {fieldValueVariableName} = {valueExpression};");
                dataRecordMapperBuilder.AppendLine($@"fieldValues.Add(""{matchField.Name}"", {fieldValueVariableName});");
                if(column.FieldName == dataRecordStorageSettings.DateTimeField)
                {
                    dataRecordMapperBuilder.AppendLine($"if({fieldValueVariableName} != null)");
                    dataRecordMapperBuilder.AppendLine($@"dataRecord.RecordTime = {fieldValueVariableName}{(isNullableField ? ".Value" : "")};");
                }
                dataRecordMapperBuilder.AppendLine("}");
            }
            dynamicRecordMapper = dynamicRecordMapperBuilder.ToString();
            dataRecordMapper = dataRecordMapperBuilder.ToString();
        }        
    }
}
