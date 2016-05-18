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
            if (String.IsNullOrEmpty(measureConfig.GetValueMethod))
                return null;
            else
            {
                Type runtimeType;
                List<string> errorMessages;
                string fullTypeName;
                string classDefinition = BuildMeasureEvaluatorClassDefinition(measureConfig, out fullTypeName);
                if (TryBuildRuntimeType(classDefinition, fullTypeName, out runtimeType, out errorMessages))
                {
                    IMeasureEvaluator measureEvaluator = Activator.CreateInstance(runtimeType) as IMeasureEvaluator;
                    if (measureEvaluator == null)
                        throw new NullReferenceException("measureEvaluator");
                    return measureEvaluator;
                }
                else
                {
                    RaiseCompilationError(measureConfigId, errorMessages);
                    return null;
                }
            }
        }

        public static IDimensionEvaluator GetDimensionEvaluator(int dimensionConfigId, AnalyticDimensionConfig dimensionConfig)
        {
            if (String.IsNullOrEmpty(dimensionConfig.GetValueMethod))
                return null;
            else
            {
                Type runtimeType;
                List<string> errorMessages;
                string fullTypeName;
                string classDefinition = BuildDimensionEvaluatorClassDefinition(dimensionConfig, out fullTypeName);
                if (TryBuildRuntimeType(classDefinition, fullTypeName, out runtimeType, out errorMessages))
                {
                    IDimensionEvaluator dimensionEvaluator = Activator.CreateInstance(runtimeType) as IDimensionEvaluator;
                    if (dimensionEvaluator == null)
                        throw new NullReferenceException("dimensionEvaluator");
                    return dimensionEvaluator;
                }
                else
                {
                    RaiseCompilationError(dimensionConfigId, errorMessages);
                    return null;
                }
            }
        }

        public static bool TryBuildRuntimeType(string classDefinition, string fullTypeName, out Type runtimeType, out List<string> errorMessages)
        {
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

        private static void RaiseCompilationError(int measureConfigId, List<string> errorMessages)
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

        private static string BuildMeasureEvaluatorClassDefinition(AnalyticMeasureConfig measureConfig, out string fullTypeName)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Entities.IMeasureEvaluator
                    {
                        public dynamic GetMeasureValue(Vanrise.Analytic.Entities.IGetMeasureValueContext context)
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

            classDefinitionBuilder.Replace("#EXECUTIONCODE#", measureConfig.GetValueMethod);

            return classDefinitionBuilder.ToString();
        }

        private static string BuildDimensionEvaluatorClassDefinition(AnalyticDimensionConfig dimensionConfig, out string fullTypeName)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Entities.IDimensionEvaluator
                    {
                        public dynamic GetDimensionValue(Vanrise.Analytic.Entities.IGetDimensionValueContext context)
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            //classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "DimensionEvaluator";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            classDefinitionBuilder.Replace("#EXECUTIONCODE#", dimensionConfig.GetValueMethod);

            return classDefinitionBuilder.ToString();
        }

    }
}
