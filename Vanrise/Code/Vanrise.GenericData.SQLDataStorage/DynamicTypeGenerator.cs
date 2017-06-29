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
                            #COLUMNSVALUES#
                            streamForBulkInsert.WriteRecord(valuesBuilder.ToString());
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

                    //StringBuilder recordFormatBuilder = new StringBuilder();
                    StringBuilder columnsValuesBuilder = new StringBuilder("System.Text.StringBuilder valuesBuilder = new System.Text.StringBuilder();");
                    StringBuilder columnNamesBuilder = new StringBuilder();
                    int columnIndex = 0;
                    object[] data = new object[1];

                    foreach (var columnSettings in dataRecordStorageSettings.Columns)
                    {
                        if (columnIndex > 0)
                            columnsValuesBuilder.AppendLine("valuesBuilder.Append(\"^\");");
                        //recordFormatBuilder.Append("{" + columnIndex.ToString() + "}");
                        string sqlDataType = columnSettings.SQLDataType.ToLower();
                        if (sqlDataType.Contains("decimal"))
                        {
                            int precision = 0;
                            if (sqlDataType.Contains(","))
                            {
                                string[] parts = sqlDataType.Split(',');
                                if (parts.Length == 2)
                                {
                                    string precisionPart = parts[1].Replace(")", "").Trim();
                                    int.TryParse(precisionPart, out precision);
                                }
                            }
                            columnsValuesBuilder.AppendLine(String.Format("valuesBuilder.Append(record.{0} != null ? Decimal.Round(record.{0}, {1}) : String.Empty);", columnSettings.ValueExpression, precision));
                        }
                        else if (sqlDataType == "bit")
                            columnsValuesBuilder.AppendLine(String.Format("valuesBuilder.Append(record.{0} != null ? record.{0} == true ? \"1\" : \"0\" : String.Empty);", columnSettings.ValueExpression));
                        else if (sqlDataType == "datetime")
                            columnsValuesBuilder.AppendLine(String.Format("valuesBuilder.Append(record.{0} != null && record.{0} != default(DateTime) ? Vanrise.Data.BaseDataManager.GetDateTimeForBCP(record.{0}) : String.Empty);", columnSettings.ValueExpression));
                        else
                            columnsValuesBuilder.AppendLine(String.Format("valuesBuilder.Append(record.{0} != null ? record.{0} : String.Empty);", columnSettings.ValueExpression));
                        columnIndex++;
                        if (columnNamesBuilder.Length > 0)
                            columnNamesBuilder.Append(", ");
                        columnNamesBuilder.AppendFormat("\"{0}\"", columnSettings.ColumnName);
                    }

                    if (dataRecordStorageSettings.IncludeQueueItemId)
                    {
                        if (columnIndex > 0)
                            columnsValuesBuilder.AppendLine("valuesBuilder.Append(\"^\");");
                        //recordFormatBuilder.Append("{" + dataRecordStorageSettings.Columns.Count.ToString() + "}");

                        if (columnNamesBuilder.Length > 0)
                            columnNamesBuilder.Append(", ");
                        columnNamesBuilder.AppendFormat("\"QueueItemId\"");

                        columnsValuesBuilder.AppendLine("valuesBuilder.Append(record.QueueItemId != null && record.QueueItemId != default(long) ? record.QueueItemId : String.Empty);");
                    }

                    //classDefinitionBuilder.Replace("#RECORDFORMAT#", recordFormatBuilder.ToString());
                    classDefinitionBuilder.Replace("#COLUMNSVALUES#", columnsValuesBuilder.ToString());
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
            foreach (var column in dataRecordStorageSettings.Columns)
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
            foreach (var column in dataRecordStorageSettings.Columns)
            {
                var matchField = recordType.Fields.FirstOrDefault(itm => itm.Name == column.ValueExpression);
                if (matchField == null)
                    throw new NullReferenceException("matchField");
                dtSchemaBuilder.AppendLine(String.Format(@"dt.Columns.Add(""{0}"", typeof({1}));", column.ColumnName, CSharpCompiler.TypeToString(matchField.Type.GetNonNullableRuntimeType())));
                dtRowsBuilder.AppendLine(String.Format(@"dr[""{0}""] = record.{1} != null ? record.{1} : DBNull.Value;", column.ColumnName, column.ValueExpression));
            }

            if (dataRecordStorageSettings.IncludeQueueItemId)
            {
                dtSchemaBuilder.AppendLine(@"dt.Columns.Add(""QueueItemId"", typeof(Int64));");
                dtRowsBuilder.AppendLine(@"dr[""QueueItemId""] = record.QueueItemId != null && record.QueueItemId != default(long) ? record.QueueItemId : DBNull.Value;");
            }

            builder.Replace("#DTSCHEMA#", dtSchemaBuilder.ToString());
            builder.Replace("#ROWBUILDER#", dtRowsBuilder.ToString());

            return builder.ToString();
        }

    }


}
