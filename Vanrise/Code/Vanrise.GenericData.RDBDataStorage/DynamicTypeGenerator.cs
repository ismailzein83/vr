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

                        public void WriteFieldsToRecordStream(dynamic record, Vanrise.Data.RDB.RDBBulkInsertQueryWriteRecordContext bulkInsertRecordContext)
                        {
                            #SetRDBBulkInsertColumnsFromRecordImplementation#
                        }

                        public void SetRDBInsertColumnsFromRecord(dynamic record, Vanrise.Data.RDB.RDBInsertQuery insertQuery)
                        {
                            #SetRDBInsertColumnsFromRecordImplementation#
                        }

                        public dynamic GetDynamicRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader)
                        {
                            dynamic dataRecord = new #DATARECORD_RUNTIMETYPE#();
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
                    StringBuilder setRDBInsertColumnsFromRecordBuilder = new StringBuilder();


                    foreach (var columnSettings in dataRecordStorageSettings.Columns)
                    {
                        var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == columnSettings.FieldName);
                        matchField.ThrowIfNull("matchField", columnSettings.FieldName);
                        if (!columnSettings.IsIdentity)
                            AppendSetRDBValueFromRecordField(setRDBBulkInsertColumnsValuesBuilder, matchField, "bulkInsertRecordContext");
                        AppendSetRDBValueFromRecordField(setRDBInsertColumnsFromRecordBuilder, matchField, "insertQuery");
                    }

                    if (dataRecordStorageSettings.IncludeQueueItemId)
                    {                        
                        setRDBBulkInsertColumnsValuesBuilder.AppendLine(
                                    @"  if(record.QueueItemId != null && record.QueueItemId != default(long))
                                            bulkInsertRecordContext.Value(record.QueueItemId));
                                        else
                                            bulkInsertRecordContext.NullValue();");
                    }

                    classDefinitionBuilder.Replace("#SetRDBBulkInsertColumnsFromRecordImplementation#", setRDBBulkInsertColumnsValuesBuilder.ToString());
                    classDefinitionBuilder.Replace("#SetRDBInsertColumnsFromRecordImplementation#", setRDBInsertColumnsFromRecordBuilder.ToString());

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

        private static void AppendSetRDBValueFromRecordField(StringBuilder rdbBulkInsertColumnsValuesBuilder, DataRecordField matchField, string rdbContextName)
        {
            string fieldName = matchField.Name;
            if (matchField.Type.StoreValueSerialized)
            {
                rdbBulkInsertColumnsValuesBuilder.AppendLine($"var {fieldName}Context = new Vanrise.GenericData.Entities.SerializeDataRecordFieldValueContext() {{ Object = record.{fieldName} }}; ");
                rdbBulkInsertColumnsValuesBuilder.AppendLine($"{rdbContextName}.Value(s_{fieldName}FieldType.SerializeValue({fieldName}Context));");
            }
            else
            {
                var runtimeType = matchField.Type.GetRuntimeType();
                if (runtimeType.IsValueType && Nullable.GetUnderlyingType(runtimeType) != null)//is nullable Type
                {
                    rdbBulkInsertColumnsValuesBuilder.AppendLine(
                        $@" if(record.{fieldName}.HasValue)
                                {rdbContextName}.Value(record.{fieldName}.Value));
                            else
                                {rdbContextName}.NullValue();");
                }
                else
                {
                    rdbBulkInsertColumnsValuesBuilder.AppendLine($"{rdbContextName}.Value(record.{fieldName}));");
                }
            }
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
                if (!matchField.Type.StoreValueSerialized)
                {
                    var runtimeType = matchField.Type.GetRuntimeType();
                    string getReaderValueMethodName = GetGetReaderValueMethodName(runtimeType);
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
                dynamicRecordMapperBuilder.AppendLine($@"dataRecord.{matchField.Name} = {valueExpression};");
                dataRecordMapperBuilder.AppendLine($@"fieldValues.Add(""{matchField.Name}"", {valueExpression});");
                if(column.FieldName == dataRecordStorageSettings.CreatedTimeField)
                {
                    dataRecordMapperBuilder.AppendLine($@"dataRecord.RecordTime = {valueExpression};");
                }
                dataRecordMapperBuilder.AppendLine("}");
            }
            dynamicRecordMapper = dynamicRecordMapperBuilder.ToString();
            dataRecordMapper = dataRecordMapperBuilder.ToString();
        }

        private string GetGetReaderValueMethodName(Type runtimeType)
        {
            Dictionary<Type, string> methodNamesByType = GetRDBReaderMethodNamesByType();

            string functionName;
            if (!methodNamesByType.TryGetValue(runtimeType, out functionName))
                throw new Exception($"No GetReaderValueMethod found for type {runtimeType}");
            return functionName;
        }

        static Dictionary<Type, string> s_methodNamesByType = new Dictionary<Type, string>();

        private static Dictionary<Type, string> GetRDBReaderMethodNamesByType()
        {
            if(s_methodNamesByType.Count == 0)
            {
                lock(s_methodNamesByType)
                {
                    if(s_methodNamesByType.Count == 0)
                    {
                        foreach (var methodInfo in typeof(IRDBDataReader).GetMethods())
                        {
                            string existingMethodName;
                            if (!s_methodNamesByType.TryGetValue(methodInfo.ReturnType, out existingMethodName))
                            {
                                s_methodNamesByType.Add(methodInfo.ReturnType, methodInfo.Name);
                            }
                            else
                            {
                                if (methodInfo.Name.Contains("WithNullHandling"))
                                    s_methodNamesByType[methodInfo.ReturnType] = methodInfo.Name;
                            }
                        }
                    }
                }
            }

            return s_methodNamesByType;
        }
    }
}
