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

                        public void FillRecordInfoFromDynamicRecord(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo, dynamic dynamicRecord)
                        {
                            #DATARECORD_RUNTIMETYPE# record = Vanrise.Common.ExtensionMethods.CastWithValidate<#DATARECORD_RUNTIMETYPE#>(dynamicRecord, ""dynamicRecord"");
                            recordInfo.RecordTime = record.#TIMEFIELDNAME#;
                            recordInfo.RecordId = record.#IDFIELDNAME#;
                            List<string> fieldValues = new List<string>();
                            #FILLRECORDCONCATENATEDFIELDS#
                            recordInfo.RecordContent = string.Join(""#FIELDSEPARATOR#"", fieldValues);
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
                            setFillRecordConcatenatedFieldsBuilder.AppendLine($"fieldValues.Add(record.{fieldName});");
                        }
                    }
                    classDefinitionBuilder.Replace("#FILLRECORDCONCATENATEDFIELDS#", setFillRecordConcatenatedFieldsBuilder.ToString());
                    
                    //Type dataRecordRuntimeType = recordTypeManager.GetDataRecordRuntimeType(dataRecordStorage.DataRecordTypeId);
                    //dataRecordRuntimeType.ThrowIfNull("dataRecordRuntimeType", dataRecordStorage.DataRecordTypeId);
                    //classDefinitionBuilder.Replace("#DATARECORD_RUNTIMETYPE#", CSharpCompiler.TypeToString(dataRecordRuntimeType));

                    //string dynamicRecordMapper;
                    //string dataRecordMapper;
                    //BuildMapperImplementations(dataRecordStorageSettings, recordType, out dynamicRecordMapper, out dataRecordMapper);
                    //classDefinitionBuilder.Replace("#DynamicRecordMapperImplementation#", dynamicRecordMapper);
                    //classDefinitionBuilder.Replace("#DataRecordMapperImplementation#", dataRecordMapper);

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
