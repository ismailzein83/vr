using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public interface IReportGenerationCustomCode
    {
        byte[] Generate(IReportGenerationCustomCodeContext context);
    }
    public class ReportGenerationCustomCodeSettingsManager
    {
        VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
        #region Public Methods
        public IEnumerable<ReportGenerationCustomCodeDefinitionInfo> GetReportGenerationCustomCodeSettingsInfo()
        {
            Func<ReportGenerationCustomCodeSettings, bool> filterExpression = (customCodeSettings) =>
            {
                return true;
            };
            return vrComponentTypeManager.GetComponentTypes<ReportGenerationCustomCodeSettings, ReportGenerationCustomCodeDefinition>().MapRecords(ReportGenerationCustomCodeDefinitionInfoMapper);

        }
        public Type GetOrCreateRuntimeType(string customCode)
        {
            string fullTypeName;
            var classDefinition = BuildClassDefinition(customCode, out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass("ReportGenerationCustomCode", classDefinition, out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
                }

                throw new Exception(String.Format("Compile Error when building Custom Code file. Errors: {0}", errorsBuilder));
            }
            else
                return compilationOutput.OutputAssembly.GetType(fullTypeName);
        }
        public string GetCustomCodeById(Guid vrComponentTypeId)
        {
            var reportGenerationCustomCodeSettings = GetVRAutomatedReportQueryDefinitionSettings(vrComponentTypeId);
            return reportGenerationCustomCodeSettings.CustomCode;
        }
        public ReportGenerationCustomCodeSettings GetVRAutomatedReportQueryDefinitionSettings(Guid vrComponentTypeId)
        {
            return vrComponentTypeManager.GetComponentTypeSettings<ReportGenerationCustomCodeSettings>(vrComponentTypeId);
        }
        #endregion

        #region Private Methods
        private ReportGenerationCustomCodeDefinitionInfo ReportGenerationCustomCodeDefinitionInfoMapper(ReportGenerationCustomCodeDefinition reportGenerationCustomCodeDefinition)
        {
            return new ReportGenerationCustomCodeDefinitionInfo
            {
                DefinitionId = reportGenerationCustomCodeDefinition.VRComponentTypeId,
                Name = reportGenerationCustomCodeDefinition.Name
            };
        }
        private string BuildClassDefinition(string customCode, out string fullTypeName)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.Analytic.Business;
                using Vanrise.Analytic.Entities;
                using Vanrise.Common.Excel;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Business.IReportGenerationCustomCode
                    {                   
                        public byte[] Generate(IReportGenerationCustomCodeContext context)
                        {
                            #CUSTOMCODE#    
                        } 
                    }
                }");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "ReportGenerationCustomCode";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            classDefinitionBuilder.Replace("#CUSTOMCODE#", customCode);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);
            return classDefinitionBuilder.ToString();
        }
        #endregion
    }
}
