using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.SQLDataStorage
{
     
    internal class DynamicTypeGenerator
    {
        public IBulkInsertWriter GetBulkInsertWriter(int dataRecordStorageId, SQLDataRecordStorageSettings dataRecordStorageSettings)
        {
            String cacheName = String.Format("SQLDataStorage_DynamicTypeGenerator_GetBulkInsertWriter_{0}", dataRecordStorageId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    StringBuilder classDefinitionBuilder = new StringBuilder(@"
                using System;                

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.SQLDataStorage.IBulkInsertWriter
                    {      

                        string[] _columnNames = {#COLUMNNAMES#};

                        public string[] ColumnNames 
                        {
                            get
                            {
                                return _columnNames;
                            }
                        }
                  
                        public void WriteRecordToStream(dynamic record, Vanrise.Data.SQL.StreamForBulkInsert streamForBulkInsert)
                        {
                            streamForBulkInsert.WriteRecord(""#RECORDFORMAT#"" #COLUMNSVALUES#);
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
                        if (columnSettings.SQLDataType.ToLower().Contains("decimal"))
                            columnsValuesBuider.Append(String.Format(", Vanrise.Data.BaseDataManager.GetDecimalForBCP(record.{0})", columnSettings.ValueExpression));
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

                    string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.SQLDataStorage");
                    string className = "BulkInsertWriter";
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
                            dataRecordStorageId, errorsBuilder));
                    }
                    else
                        return Activator.CreateInstance(compilationOutput.OutputAssembly.GetType(fullTypeName)) as IBulkInsertWriter;
                });
            
        }
    }

    public interface IBulkInsertWriter
    {
        string[] ColumnNames { get; }
        void WriteRecordToStream(dynamic record, StreamForBulkInsert streamForBulkInsert);
    }
}
