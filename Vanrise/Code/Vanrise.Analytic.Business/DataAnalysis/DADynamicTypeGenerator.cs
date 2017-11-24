using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Business
{
    internal static class DAProfCalcDynamicTypeGenerator
    {
        public static void BuildCalculationEvaluators(Guid outputItemDefinitionId, IEnumerable<DAProfCalcCalculationFieldDetail> calculationFields)
        {
            Dictionary<string, string> calculationFullTypeById = new Dictionary<string, string>();
            StringBuilder codeBuilder = new StringBuilder(@"using System;
                                                            using System.Linq;");
            foreach (var calculationField in calculationFields)
            {
                if (String.IsNullOrEmpty(calculationField.Entity.Expression))
                    throw new NullReferenceException(String.Format("calculationField.Entity.Expression. '{0}'", calculationField.Entity.FieldName));
                StringBuilder classDefinitionBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Entities.IDAProfCalcCalculationEvaluator
                    {
                        public dynamic GetCalculationValue(Vanrise.Analytic.Entities.IDAProfCalcGetCalculationValueContext context)
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
                string className = "MeasureEvaluator";
                classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
                classDefinitionBuilder.Replace("#CLASSNAME#", className);
                string fullTypeName = String.Format("{0}.{1}", classNamespace, className);

                classDefinitionBuilder.Replace("#EXECUTIONCODE#", calculationField.Entity.Expression);
                codeBuilder.AppendLine(classDefinitionBuilder.ToString());
                calculationFullTypeById.Add(calculationField.Entity.FieldName, fullTypeName);
            }

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("DAProfCalcOutputItemDef_{0}", outputItemDefinitionId.ToString().Replace("-", "")), codeBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }
                throw new Exception(String.Format("Compile Error when building Calculation Evaluator for Output Item Definition Id '{0}'. Errors: {1}",
                    outputItemDefinitionId, errorsBuilder));
            }

            foreach (var calculationField in calculationFields)
            {
                var calculationFullType = calculationFullTypeById[calculationField.Entity.FieldName];
                var runtimeType = compilationOutput.OutputAssembly.GetType(calculationFullType);
                if (runtimeType == null)
                    throw new NullReferenceException("runtimeType");
                calculationField.Evaluator = Activator.CreateInstance(runtimeType) as IDAProfCalcCalculationEvaluator;
                if (calculationField.Evaluator == null)
                    throw new NullReferenceException(String.Format("calculationField.Evaluator '{0}'", calculationField.Entity.FieldName));
            }
        }
    }
}