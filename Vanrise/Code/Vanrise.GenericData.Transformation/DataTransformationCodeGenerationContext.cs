using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    internal class DataTransformationCodeGenerationContext : IDataTransformationCodeGenerationContext
    {
        DataTransformationDefinition _dataTransformationDefinition;

        StringBuilder _globalMembersBuilder;
        StringBuilder _instanceExecutionBlockBuilder;

        public DataTransformationCodeGenerationContext(DataTransformationDefinition dataTransformationDefinition)
        {
            _dataTransformationDefinition = dataTransformationDefinition;

        }


        #region IDataTransformationCodeGenerationContext

        string IDataTransformationCodeGenerationContext.GenerateUniqueMemberName(string memberName)
        {
            return String.Format("{0}_{1}", memberName, Guid.NewGuid().ToString("N"));
        }

        void IDataTransformationCodeGenerationContext.AddGlobalMember(string memberDeclarationCode)
        {
            _globalMembersBuilder.AppendLine(memberDeclarationCode);
            _globalMembersBuilder.AppendLine();
        }

        void IDataTransformationCodeGenerationContext.AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders)
        {
            if (placeholders != null && placeholders.Length > 0)
                _instanceExecutionBlockBuilder.AppendFormat(codeLineTemplate, placeholders);
            else
                _instanceExecutionBlockBuilder.Append(codeLineTemplate);
            _instanceExecutionBlockBuilder.AppendLine();
        }

        void IDataTransformationCodeGenerationContext.GenerateStepsCode(IEnumerable<MappingStep> steps)
        {
            IDataTransformationCodeGenerationContext codeGenContext = this as IDataTransformationCodeGenerationContext;
            int stepNumber = 0;
            foreach (var step in steps)
            {
                codeGenContext.AddCodeToCurrentInstanceExecutionBlock("try {");
                step.GenerateExecutionCode(this);
                codeGenContext.AddCodeToCurrentInstanceExecutionBlock("} catch(Exception ex) {");                
                codeGenContext.AddCodeToCurrentInstanceExecutionBlock("Exception exceptionToLog = new Exception(\"Transformation Error occured in Step Name '{0}'. Step Number '{1}'\", ex);", step.GetType().FullName, ++stepNumber);
                codeGenContext.AddCodeToCurrentInstanceExecutionBlock("throw exceptionToLog; }");
            }
        }

        List<DataTransformationRecordType> IDataTransformationCodeGenerationContext.Records
        {
            get
            {
                return this._dataTransformationDefinition.RecordTypes;
            }
        }

        #endregion

        public bool TryBuildRuntimeType(out DataTransformationRuntimeType runtimeType, out List<string> errorMessages)
        {
            _globalMembersBuilder = new StringBuilder();
            _instanceExecutionBlockBuilder = new StringBuilder();
            (this as IDataTransformationCodeGenerationContext).GenerateStepsCode(_dataTransformationDefinition.MappingSteps);
            string fullTypeName;
            string classDefinition = BuildClassDefinition(out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("DataTransformation_{0}", _dataTransformationDefinition.Name), classDefinition, out compilationOutput))
            {
                runtimeType = null;
                errorMessages = compilationOutput.ErrorMessages;
                return false;
            }
            var executorType = compilationOutput.OutputAssembly.GetType(fullTypeName);
            if (executorType == null)
                throw new NullReferenceException("executorType");
            runtimeType = new DataTransformationRuntimeType
            {
                ExecutorType = executorType
            };
            errorMessages = null;
            return true;
        }

        private string BuildClassDefinition(out string fullTypeName)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            foreach (var recordType in _dataTransformationDefinition.RecordTypes)
            {
                if (recordType.IsArray)
                {
                    string dataRecordRuntimeType = !String.IsNullOrEmpty(recordType.FullTypeName) ? recordType.FullTypeName : "dynamic";
                    (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public List<{0}> {1} = new List<{0}>();", dataRecordRuntimeType, recordType.RecordName));
                }
                else
                {
                    string dataRecordRuntimeType;
                    if (recordType.DataRecordTypeId.HasValue)
                    {
                        dataRecordRuntimeType = CSharpCompiler.TypeToString(dataRecordTypeManager.GetDataRecordRuntimeType(recordType.DataRecordTypeId.Value));
                        (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public {0} {1} = new {0}();", dataRecordRuntimeType, recordType.RecordName));
                    }
                    else
                    {
                        dataRecordRuntimeType = !String.IsNullOrEmpty(recordType.FullTypeName) ? recordType.FullTypeName : "dynamic";
                        (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public {0} {1};", dataRecordRuntimeType, recordType.RecordName));
                    }
                }
            }
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using System.Linq;
                using Vanrise.Common;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : #EXECUTORBASE#
                    {                        
                        #GLOBALMEMBERS#

                        void #EXECUTORBASE#.Execute()
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            classDefinitionBuilder.Replace("#EXECUTORBASE#", typeof(IDataTransformationExecutor).FullName);
            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", _globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.Transformation.Runtime");
            string className = "DataTransformationExecutor";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }
    }
}
