using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public class MeasureValueEvaluatorContext : IMeasureValueEvaluatorContext
    {
        StringBuilder _instanceExecutionBlockBuilder;
        StringBuilder _globalMembersBuilder;
        Dictionary<string, MeasureConfiguration> IMeasureValueEvaluatorContext.Measures
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryBuildRuntimeType(out MeasureEvaluatorRuntimeType runtimeType, out List<string> errorMessages)
        {
            _globalMembersBuilder = new StringBuilder();
            _instanceExecutionBlockBuilder = new StringBuilder();
            string fullTypeName;
            string classDefinition = BuildClassDefinition(out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
            {
                runtimeType = null;
                errorMessages = compilationOutput.ErrorMessages;
                return false;
            }
            var executorType = compilationOutput.OutputAssembly.GetType(fullTypeName);
            if (executorType == null)
                throw new NullReferenceException("executorType");
            runtimeType = new MeasureEvaluatorRuntimeType
            {
                ExecutorType = executorType
            };
            errorMessages = null;
            return true;
        }
        private string BuildClassDefinition(out string fullTypeName)
        {

            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : #EXECUTORBASE#
                    {                        
                        #GLOBALMEMBERS#

                        object #EXECUTORBASE#.Evaluate()
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            classDefinitionBuilder.Replace("#EXECUTORBASE#", typeof(IMeasureValueExecutor).FullName);
            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", _globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "MeasureValueExecutor";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }
        string IMeasureValueEvaluatorContext.GenerateUniqueMemberName(string memberName)
        {
            return String.Format("{0}_{1}", memberName, Guid.NewGuid().ToString("N"));
        }
        void IMeasureValueEvaluatorContext.AddGlobalMember(string memberDeclarationCode)
        {
            _globalMembersBuilder.AppendLine(memberDeclarationCode);
            _globalMembersBuilder.AppendLine();
        }
        void IMeasureValueEvaluatorContext.AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders)
        {
            if (placeholders != null && placeholders.Length > 0)
                _instanceExecutionBlockBuilder.AppendFormat(codeLineTemplate, placeholders);
            else
                _instanceExecutionBlockBuilder.Append(codeLineTemplate);
            _instanceExecutionBlockBuilder.AppendLine();
        }
    }
}
