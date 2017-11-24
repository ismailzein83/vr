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
        public static void BuildMeasureEvaluators(Guid analyticTableId, IEnumerable<AnalyticMeasure> measureConfigs)
        {
            Dictionary<Guid, string> measureFullTypeById = new Dictionary<Guid, string>();
            StringBuilder codeBuilder = new StringBuilder(@"using System;
                                                            using System.Linq;");
            foreach(var measureConfig in measureConfigs)
            {
                if (String.IsNullOrEmpty(measureConfig.Config.GetValueMethod))
                    throw new NullReferenceException(String.Format("measureConfig.Config.GetValueMethod. '{0}'", measureConfig.AnalyticMeasureConfigId));
                StringBuilder classDefinitionBuilder = new StringBuilder(@"                 

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
                string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

                classDefinitionBuilder.Replace("#EXECUTIONCODE#", measureConfig.Config.GetValueMethod);
                codeBuilder.AppendLine(classDefinitionBuilder.ToString());
                measureFullTypeById.Add(measureConfig.AnalyticMeasureConfigId, fullTypeName);
            }

            AnalyticTableManager analyticTableManager = new AnalyticTableManager();           
            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("AnalyticTableMeasures_{0}", analyticTableManager.GetAnalyticTableName(analyticTableId)), codeBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }
                throw new Exception(String.Format("Compile Error when building Measure Evaluator for Analytic Table Id '{0}'. Errors: {1}",
                    analyticTableId, errorsBuilder));
            }
                 
            foreach(var measureConfig in measureConfigs)
            {
                var measureFullType = measureFullTypeById[measureConfig.AnalyticMeasureConfigId];
                var runtimeType = compilationOutput.OutputAssembly.GetType(measureFullType);
                if (runtimeType == null)
                    throw new NullReferenceException("runtimeType");
                measureConfig.Evaluator = Activator.CreateInstance(runtimeType) as IMeasureEvaluator;
                if(measureConfig.Evaluator == null)
                    throw new NullReferenceException(String.Format("measureConfig.Evaluator '{0}'", measureConfig.AnalyticMeasureConfigId));
            }
        }

        public static void BuildDimensionEvaluators(Guid analyticTableId, IEnumerable<AnalyticDimension> dimensionConfigs)
        {
            Dictionary<Guid, string> dimensionFullTypeById = new Dictionary<Guid, string>();
            StringBuilder codeBuilder = new StringBuilder(@"using System;
                                                            using System.Linq;");

            foreach (var dimensionConfig in dimensionConfigs)
            {
                if (String.IsNullOrEmpty(dimensionConfig.Config.GetValueMethod))
                    continue;
                StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                
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


                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
                string className = "DimensionEvaluator";
                classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                classDefinitionBuilder.Replace("#CLASSNAME#", className);
                string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

                classDefinitionBuilder.Replace("#EXECUTIONCODE#", dimensionConfig.Config.GetValueMethod);
                codeBuilder.AppendLine(classDefinitionBuilder.ToString());
                dimensionFullTypeById.Add(dimensionConfig.AnalyticDimensionConfigId, fullTypeName);
            }

            AnalyticTableManager analyticTableManager = new AnalyticTableManager();
            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("AnalyticTableDimensions_{0}", analyticTableManager.GetAnalyticTableName(analyticTableId)), codeBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }
                throw new Exception(String.Format("Compile Error when building Dimension Evaluator for Analytic Table Id '{0}'. Errors: {1}",
                    analyticTableId, errorsBuilder));
            }

            foreach (var dimensionConfig in dimensionConfigs)
            {
                if (String.IsNullOrEmpty(dimensionConfig.Config.GetValueMethod))
                    continue;
                var dimensionFullType = dimensionFullTypeById[dimensionConfig.AnalyticDimensionConfigId];
                var runtimeType = compilationOutput.OutputAssembly.GetType(dimensionFullType);
                if (runtimeType == null)
                    throw new NullReferenceException("runtimeType");
                dimensionConfig.Evaluator = Activator.CreateInstance(runtimeType) as IDimensionEvaluator;
                if (dimensionConfig.Evaluator == null)
                    throw new NullReferenceException(String.Format("dimensionConfig.Evaluator '{0}'", dimensionConfig.AnalyticDimensionConfigId));
            }
        }

    }
}
