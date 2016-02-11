using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.GenericData.SQLDataStorage
{
     
    internal class DynamicTypeGenerator
    {
        public IBulkInsertWriter GetBulkInsertWriter(int dataRecordTypeId, SQLDataRecordStorageSettings dataRecordStorageSettings)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@"
                using System;                

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.GenericData.SQLDataStorage.IBulkInsertWriter
                    {                        
                        public void WriteRecordToStream(dynamic record, Vanrise.Data.SQL.StreamForBulkInsert streamForBulkInsert)
                        {
                            streamForBulkInsert.WriteRecord(""#RECORDFORMAT#"" #COLUMNSVALUES#);
                        }
                    }
                }");
           
            StringBuilder recordFormatBuilder = new StringBuilder();
            StringBuilder columnsValuesBuider = new StringBuilder();
            int columnIndex = 0;
            foreach(var columnSettings in dataRecordStorageSettings.Columns)
            {
                if (columnIndex > 0)
                    recordFormatBuilder.Append("^");
                recordFormatBuilder.Append("{" + columnIndex.ToString() + "}");                
                columnsValuesBuider.Append(String.Format(", record.{0}", columnSettings.ValueExpression));
                columnIndex++;
            }
            classDefinitionBuilder.Replace("#RECORDFORMAT#", recordFormatBuilder.ToString());
            classDefinitionBuilder.Replace("#COLUMNSVALUES#", columnsValuesBuider.ToString());

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
                throw new Exception(String.Format("Compile Error when building BulkInsertWriter for record type Id'{0}'. Errors: {1}",
                    dataRecordTypeId, errorsBuilder));
            }
            else
                return Activator.CreateInstance(compilationOutput.OutputAssembly.GetType(fullTypeName)) as IBulkInsertWriter;
        }
    }

    public interface IBulkInsertWriter
    {
        void WriteRecordToStream(dynamic record, StreamForBulkInsert streamForBulkInsert);
    }
}
