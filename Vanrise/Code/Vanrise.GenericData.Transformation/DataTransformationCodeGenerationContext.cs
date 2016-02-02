using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    internal class DataTransformationCodeGenerationContext : IDataTransformationCodeGenerationContext
    {
        DataTransformationDefinition _dataTransformationDefinition;
        public DataTransformationCodeGenerationContext(DataTransformationDefinition dataTransformationDefinition)
        {
            _dataTransformationDefinition = dataTransformationDefinition;
            
        }

        StringBuilder _globalMembersBuilder;
        StringBuilder _instanceExecutionBlockBuilder;

        string IDataTransformationCodeGenerationContext.GenerateUniqueMemberName(string memberName)
        {
            return String.Format("{0}_{1}", memberName, Guid.NewGuid().ToString().Replace("-", ""));
        }

        void IDataTransformationCodeGenerationContext.AddGlobalMember(string memberDeclarationCode)
        {
            _globalMembersBuilder.AppendLine(memberDeclarationCode);
            _globalMembersBuilder.AppendLine();
        }

        void IDataTransformationCodeGenerationContext.AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders)
        {
            _instanceExecutionBlockBuilder.AppendFormat(codeLineTemplate, placeholders);
            _instanceExecutionBlockBuilder.AppendLine();
        }

        public Type GetExecutorType()
        {
            _globalMembersBuilder = new StringBuilder();
            _instanceExecutionBlockBuilder = new StringBuilder();
            foreach (var step in _dataTransformationDefinition.MappingSteps)
            {
                step.GenerateExecutionCode(this);
            }

            string classDefinition = BuildClassDefinition();

            string assemblyName = String.Format("GenericData.Transformation_{0}_{1:yyyyMMdd_HHmmss}", _dataTransformationDefinition.DataTransformationDefinitionId, DateTime.Now);

            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions["CompilerVersion"] = "v4.0";
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

            CompilerParameters parameters = new CompilerParameters();
            parameters.OutputAssembly = assemblyName;
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = true;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");

            parameters.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            string path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                parameters.ReferencedAssemblies.Add(info.FullName);
            }

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, classDefinition);

            StringBuilder errorsBuilder = new StringBuilder();
            if(results.Errors != null && results.Errors.Count > 0)
            {
                foreach(CompilerError error in results.Errors)
                {
                    if(!error.IsWarning)
                    {
                        errorsBuilder.AppendFormat("Error {0}: {1}. Line Number {2}", error.ErrorNumber, error.ErrorText, error.Line);
                        errorsBuilder.AppendLine();
                    }
                }
            }
            if (errorsBuilder.Length > 0)
                throw new Exception(String.Format("Compile Error when building executor type for data transformation definition Id '{0}'. Errors: {1}", _dataTransformationDefinition.DataTransformationDefinitionId, errorsBuilder));
            return results.CompiledAssembly.GetType("Vanrise.GenericData.Transformation.Runtime.DataTransformationExecutor");
        }

        private string BuildClassDefinition()
        {           
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            foreach (var recordType in _dataTransformationDefinition.RecordTypes)
            {
                var dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(recordType.DataRecordTypeId);
                (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public {0} {1} = new {0}();", dataRecordRuntimeType.FullName, recordType.RecordName));
            }
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using Vanrise.Integration.Entities;
                using Vanrise.Integration.Mappers;

                namespace Vanrise.GenericData.Transformation.Runtime
                {
                    public class DataTransformationExecutor : #EXECUTORBASE#
                    {                        
                        #GLOBALMEMBERS#
                        public void Execute()
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            classDefinitionBuilder.Replace("#EXECUTORBASE#", typeof(IDataTransformationExecutor).FullName);
            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", _globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());         

            return classDefinitionBuilder.ToString();
        }
    }
}
