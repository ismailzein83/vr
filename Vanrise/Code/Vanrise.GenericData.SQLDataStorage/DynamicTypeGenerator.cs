using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
     
    internal class DynamicTypeGenerator
    {
        public IDynamicManager GetDynamicManager(DataRecordStorage dataRecordStorage, SQLDataRecordStorageSettings dataRecordStorageSettings)
        {
            String cacheName = String.Format("SQLDataStorage_DynamicTypeGenerator_GetBulkInsertWriter_{0}", dataRecordStorage.DataRecordStorageId);
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
                using System.Collections.Generic;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Data.BaseDataManager, Vanrise.GenericData.SQLDataStorage.IDynamicManager
                    {      
                        public  #CLASSNAME#() : base("" "", false)
                        {
                        }

                        string[] _columnNames = {#COLUMNNAMES#};

                        public string[] ColumnNames 
                        {
                            get
                            {
                                return _columnNames;
                            }
                        }

                        string _columnNamesCommaDelimited = ""#COLUMNNAMESCOMMADELIMITED#"";
                        public string ColumnNamesCommaDelimited 
                        {
                            get
                            {
                                return _columnNamesCommaDelimited;
                            }
                        }
                  
                        public void WriteRecordToStream(dynamic record, Vanrise.Data.SQL.StreamForBulkInsert streamForBulkInsert)
                        {
                            streamForBulkInsert.WriteRecord(""#RECORDFORMAT#"" #COLUMNSVALUES#);
                        }

                        public void FillDataRecordFromReader(dynamic dataRecord, IDataReader reader)
                        {
                            #FillDataRecordFromReaderImplementation#
                        }

                        public DataTable ConvertDataRecordsToTable(IEnumerable<dynamic> dataRecords)
                        {
                            #ConvertDataRecordsToTableImplementation#
                        }
                    }
                }");

                    StringBuilder recordFormatBuilder = new StringBuilder();
                    StringBuilder columnsValuesBuider = new StringBuilder();
                    StringBuilder columnNamesBuilder = new StringBuilder();
                    int columnIndex = 0;
                    foreach (var columnSettings in dataRecordStorageSettings.Columns)
                    {
                        if (columnIndex > 0)
                            recordFormatBuilder.Append("^");
                        recordFormatBuilder.Append("{" + columnIndex.ToString() + "}");                        
                        string sqlDataType = columnSettings.SQLDataType.ToLower();
                        if (sqlDataType.Contains("decimal"))
                            columnsValuesBuider.Append(String.Format(", Vanrise.Data.BaseDataManager.GetDecimalForBCP(record.{0})", columnSettings.ValueExpression));
                        else if(sqlDataType == "bit")
                            columnsValuesBuider.Append(String.Format(", record.{0} != null ? (record.{0} == true ? \"1\" : \"0\") : \"\"", columnSettings.ValueExpression));
                        else if (sqlDataType == "datetime")
                            columnsValuesBuider.Append(String.Format(", (record.{0} != null && record.{0} != default(DateTime)) ? record.{0} : \"\"", columnSettings.ValueExpression));
                        else
                            columnsValuesBuider.Append(String.Format(", record.{0}", columnSettings.ValueExpression));
                        columnIndex++;
                        if (columnNamesBuilder.Length > 0)
                            columnNamesBuilder.Append(", ");
                        columnNamesBuilder.AppendFormat("\"{0}\"", columnSettings.ColumnName);
                    }
                    classDefinitionBuilder.Replace("#RECORDFORMAT#", recordFormatBuilder.ToString());
                    classDefinitionBuilder.Replace("#COLUMNSVALUES#", columnsValuesBuider.ToString());
                    classDefinitionBuilder.Replace("#COLUMNNAMES#", columnNamesBuilder.ToString());
                    classDefinitionBuilder.Replace("#COLUMNNAMESCOMMADELIMITED#", String.Join(",", dataRecordStorageSettings.Columns.Select(itm => itm.ColumnName)));
                    classDefinitionBuilder.Replace("#FillDataRecordFromReaderImplementation#", BuildFillDataRecordFromReaderImpl(dataRecordStorageSettings, recordType));
                    classDefinitionBuilder.Replace("#ConvertDataRecordsToTableImplementation#", BuildConvertDataRecordsToTableImpl(dataRecordStorageSettings, recordType));

                    string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.SQLDataStorage");
                    string className = "DynamicManager";
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
                        throw new Exception(String.Format("Compile Error when building BulkInsertWriter for record Storage Id'{0}'. Errors: {1}",
                            dataRecordStorage.DataRecordStorageId, errorsBuilder));
                    }
                    else
                        return Activator.CreateInstance(compilationOutput.OutputAssembly.GetType(fullTypeName)) as IDynamicManager;
                });
            
        }

        private string BuildFillDataRecordFromReaderImpl(SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordType recordType)
        {
            StringBuilder builder = new StringBuilder();
            foreach(var column in dataRecordStorageSettings.Columns)
            {
                var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == column.ValueExpression);
                if (matchField == null)
                    throw new NullReferenceException("matchField");
                builder.AppendLine(String.Format(@"dataRecord.{0} = GetReaderValue<{1}>(reader, ""{2}"");", matchField.Name, CSharpCompiler.TypeToString(matchField.Type.GetRuntimeType()), column.ColumnName));
            }
            return builder.ToString();
        }
        private string BuildConvertDataRecordsToTableImpl(SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordType recordType)
        {
            StringBuilder builder = new StringBuilder(@"DataTable dt = new DataTable();
                                    #DTSCHEMA#
                                    dt.BeginLoadData();
                                    foreach(var record in dataRecords)
                                    {
                                        var dr = dt.NewRow();
                                        #ROWBUILDER#
                                        dt.Rows.Add(dr);
                                    }
                                    dt.EndLoadData();
                                    return dt;");
            StringBuilder dtSchemaBuilder = new StringBuilder(0);
            StringBuilder dtRowsBuilder = new StringBuilder();
            foreach(var column in dataRecordStorageSettings.Columns)
            {
                var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == column.ValueExpression);
                if (matchField == null)
                    throw new NullReferenceException("matchField");
                dtSchemaBuilder.AppendLine(String.Format(@"dt.Columns.Add(""{0}"", typeof({1}));", column.ColumnName, CSharpCompiler.TypeToString(matchField.Type.GetRuntimeType())));
                dtRowsBuilder.AppendLine(String.Format(@"dr[""{0}""] = record.{1};", column.ColumnName, column.ValueExpression));
            }
            builder.Replace("#DTSCHEMA#", dtSchemaBuilder.ToString());
            builder.Replace("#ROWBUILDER#", dtRowsBuilder.ToString());

            return builder.ToString();
        }

    }

    
}
