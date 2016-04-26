using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    internal static class DynamicTypeGenerator
    {
        public static IMeasureEvaluator GetMeasureEvaluator(int measureConfigId, AnalyticMeasureConfig measureConfig)
        {
            if (String.IsNullOrEmpty(measureConfig.GetSQLExpressionMethod))
                return null;
            else
            {
                Type runtimeType;
                List<string> errorMessages;
                if(TryBuildRuntimeType(measureConfig, out runtimeType, out errorMessages))
                {
                    IMeasureEvaluator measureEvaluator = Activator.CreateInstance(runtimeType) as IMeasureEvaluator;
                    if (measureEvaluator == null)
                        throw new NullReferenceException("measureEvaluator");
                    return measureEvaluator;
                }
                else
                {
                    StringBuilder errorsBuilder = new StringBuilder();
                    if (errorMessages != null)
                    {
                        foreach (var errorMessage in errorMessages)
                        {
                            errorsBuilder.AppendLine(errorMessage);
                        }
                    }
                    throw new Exception(String.Format("Compile Error when building Measure Evaluator for Analytic Measure Config Id '{0}'. Errors: {1}",
                        measureConfigId, errorsBuilder));
                }
            }
        }

        public static bool TryBuildRuntimeType(AnalyticMeasureConfig measureConfig, out Type runtimeType, out List<string> errorMessages)
        {
            string fullTypeName;
            string classDefinition = BuildClassDefinition(measureConfig, out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
            {
                runtimeType = null;
                errorMessages = compilationOutput.ErrorMessages;
                return false;
            }
            runtimeType = compilationOutput.OutputAssembly.GetType(fullTypeName);
            if (runtimeType == null)
                throw new NullReferenceException("runtimeType");
            errorMessages = null;
            return true;
        }
        private static string BuildClassDefinition(AnalyticMeasureConfig measureConfig, out string fullTypeName)
        {

            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Entities.IMeasureEvaluator
                    { 
                        public string GetMeasureExpression(Vanrise.Analytic.Entities.IGetMeasureExpressionContext context)
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            //classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "MeasureEvaluator";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            classDefinitionBuilder.Replace("#EXECUTIONCODE#", measureConfig.GetSQLExpressionMethod);

            return classDefinitionBuilder.ToString();
        }
    }
}
