using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    public interface IMeasureExpressionContext
    {
        Object GetMeasureValue(string measureName);

        Decimal GetMeasureValueAsDecimal(string measureName);

        DateTime GetMeasureValueAsDateTime(string measureName);

        Object GetSQLExprValue();
    }
    public interface IMeasureExpression
    {
        Object Evaluate(IMeasureExpressionContext context);
    }

    public interface IMeasureSQLExpression
    {
        string GetSQLExpression();
    }

    public class MeasureValueEvaluatorContext : IMeasureValueEvaluatorContext
    {
        public bool TryBuildRuntimeType(out MeasureEvaluatorRuntimeType runtimeType, out List<string> errorMessages)
        {            
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

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Business.IMeasureExpression
                    { 
                        public object Evaluate(Vanrise.Analytic.Business.IMeasureExpressionContext context)
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            //classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "MeasureValueExecutor";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classDefinitionBuilder.ToString();
        }
    }
}
