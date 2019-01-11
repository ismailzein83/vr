using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.FileDataStorage
{
    internal class DynamicTypeGenerator
    {
        public IDynamicManager GetDynamicManager(DataRecordStorage dataRecordStorage, FileDataRecordStorageSettings dataRecordStorageSettings, FileRecordStoragePreparedConfig preparedConfig)
        {
            String cacheName = String.Format("FileDataStorage_DynamicTypeGenerator_GetBulkInsertWriter_{0}", dataRecordStorage.DataRecordStorageId);
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
                    public class #CLASSNAME# : Vanrise.GenericData.FileDataStorage.IDynamicManager
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

                        public void FillRecordInfoFromDynamicRecord(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo, dynamic dynamicRecord)
                        {
                            #DATARECORD_RUNTIMETYPE# record = Vanrise.Common.ExtensionMethods.CastWithValidate<#DATARECORD_RUNTIMETYPE#>(dynamicRecord, ""dynamicRecord"");
                            recordInfo.RecordTime = record.#TIMEFIELDNAME#;
                            recordInfo.RecordId = record.#IDFIELDNAME#;
                            List<string> fieldValues = new List<string>();
                            #FILLRECORDCONCATENATEDFIELDS#
                            recordInfo.RecordContent = string.Join(""#FIELDSEPARATOR#"", fieldValues);
                        }

                        public dynamic GetDynamicRecordFromRecordInfo(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo)
                        {
                            #DATARECORD_RUNTIMETYPE# record = new #DATARECORD_RUNTIMETYPE#();
                            record.#TIMEFIELDNAME# = recordInfo.RecordTime;
                            record.#IDFIELDNAME# = recordInfo.RecordId;
                            string[] fieldValues = recordInfo.RecordContent.Split('#FIELDSEPARATOR#');
                            #FILLDYNAMICRECORDFROMSTRINGFIELDVALUES#
                            return record;
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
                                        
                    classDefinitionBuilder.Replace("#TIMEFIELDNAME#", recordType.Settings.DateTimeField);
                    classDefinitionBuilder.Replace("#IDFIELDNAME#", recordType.Settings.IdField);
                    classDefinitionBuilder.Replace("#FIELDSEPARATOR#", dataRecordStorageSettings.FieldSeparator.ToString());
                    
                    StringBuilder setFillRecordConcatenatedFieldsBuilder = new StringBuilder();
                    foreach (var fieldName in preparedConfig.FieldNamesForInsert)
                    {
                        var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == fieldName);
                        matchField.ThrowIfNull("matchField", fieldName);

                        if (matchField.Type.StoreValueSerialized)
                        {
                            setFillRecordConcatenatedFieldsBuilder.AppendLine($"var {fieldName}Context = new Vanrise.GenericData.Entities.SerializeDataRecordFieldValueContext() {{ Object = record.{fieldName} }}; ");
                            setFillRecordConcatenatedFieldsBuilder.AppendLine($"fieldValues.Add(s_{fieldName}FieldType.SerializeValue({fieldName}Context));");
                        }
                        else
                        {
                            setFillRecordConcatenatedFieldsBuilder.AppendLine($@"fieldValues.Add(record.{fieldName} != null ? record.{fieldName}.ToString() : """");");
                        }
                    }
                    classDefinitionBuilder.Replace("#FILLRECORDCONCATENATEDFIELDS#", setFillRecordConcatenatedFieldsBuilder.ToString());

                    StringBuilder setFillDynamicRecordFromStringFieldValuesBuilder = new StringBuilder();
                    int fieldIndex = 0;
                    foreach (var fieldName in preparedConfig.FieldNamesForInsert)
                    {
                        var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == fieldName);
                        matchField.ThrowIfNull("matchField", fieldName);

                        string valueAsStringVariableName = $"{fieldName}ValueAsString";
                        setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine($"string {valueAsStringVariableName} = fieldValues[{fieldIndex}];");
                        setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine($"if(!string.IsNullOrEmpty({fieldName}ValueAsString))");
                        setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine("{");

                        if (matchField.Type.StoreValueSerialized)
                        {
                            setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine($"var {fieldName}Context = new Vanrise.GenericData.Entities.DeserializeDataRecordFieldValueContext() {{ Value = {valueAsStringVariableName} }}; ");
                            setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine($"record.{fieldName} = s_{fieldName}FieldType.DeserializeValue({fieldName}Context);");
                        }
                        else
                        {
                            Type runtimeType = matchField.Type.GetNonNullableRuntimeType();
                            string valueExpression = null;
                            if(runtimeType == typeof(string))
                            {
                                valueExpression = valueAsStringVariableName;
                            }
                            else if(runtimeType == typeof(int))
                            {
                                valueExpression = $"int.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(long))
                            {
                                valueExpression = $"long.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(decimal))
                            {
                                valueExpression = $"decimal.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(double))
                            {
                                valueExpression = $"double.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(float))
                            {
                                valueExpression = $"float.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(DateTime))
                            {
                                valueExpression = $"DateTime.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(Vanrise.Entities.Time))
                            {
                                valueExpression = $"new Vanrise.Entities.Time({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(bool))
                            {
                                valueExpression = $"bool.Parse({valueAsStringVariableName})";
                            }
                            else if (runtimeType == typeof(Guid))
                            {
                                valueExpression = $"Guid.Parse({valueAsStringVariableName})";
                            }
                            setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine($@"record.{fieldName} = {valueExpression};");
                        }
                        setFillDynamicRecordFromStringFieldValuesBuilder.AppendLine("}");
                        fieldIndex++;
                    }
                    classDefinitionBuilder.Replace("#FILLDYNAMICRECORDFROMSTRINGFIELDVALUES#", setFillDynamicRecordFromStringFieldValuesBuilder.ToString());

                    Type dataRecordRuntimeType = recordTypeManager.GetDataRecordRuntimeType(dataRecordStorage.DataRecordTypeId);
                    dataRecordRuntimeType.ThrowIfNull("dataRecordRuntimeType", dataRecordStorage.DataRecordTypeId);
                    classDefinitionBuilder.Replace("#DATARECORD_RUNTIMETYPE#", CSharpCompiler.TypeToString(dataRecordRuntimeType));
                    
                    string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.FileDataStorage");
                    string className = "DynamicManager";
                    classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                    classDefinitionBuilder.Replace("#CLASSNAME#", className);
                    string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

                    CSharpCompilationOutput compilationOutput;
                    if (!CSharpCompiler.TryCompileClass(String.Format("FileRecordStorage_{0}", dataRecordStorage.Name), classDefinitionBuilder.ToString(), out compilationOutput))
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
    }
}
