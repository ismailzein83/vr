using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Analytic.Business
{

    public interface IReportGenerationCustomCode
    {
        byte[] Generate(IReportGenerationCustomCodeContext context);
    }
    public class ReportGenerationCustomCodeManager
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
        public Type GetCustomCodeRuntimeType(Guid customCodeId)
        {
            string cacheName = String.Format("GetCustomCodeRuntimeTypeById_{0}", customCodeId);
            var vrComponentTypeManager = new VRComponentTypeManager();
            var runtimeType = vrComponentTypeManager.GetCachedOrCreate(cacheName,
                () =>
                {
                    var customCodeSettings = GetCustomCodeSettingById(customCodeId);
                    if (customCodeSettings != null)
                    {
                        List<string> errorMessages;
                        var type = GetOrCreateRuntimeType(customCodeSettings.CustomCode, out errorMessages, customCodeSettings.Classes);
                        if (type == null)
                            throw new Exception(String.Format("Compile Error when building Custom Code File. Errors: {0}", string.Join(",",errorMessages)));
                        else
                            return type;
                    }
                    else
                        return null;
                });
            if (runtimeType == null)
                throw new ArgumentException(String.Format("Cannot create runtime type from Custom Code Id '{0}'", customCodeId));
            return runtimeType;
        }
        public Type GetOrCreateRuntimeType(string customCode, out List<string> errorMessages, string classes = null)
        {
            string fullTypeName;
            var classDefinition = BuildClassDefinition(customCode, out fullTypeName, classes);
            CSharpCompilationOutput compilationOutput;
            errorMessages = new List<string>();

            if (!CSharpCompiler.TryCompileClass("ReportGenerationCustomCode", classDefinition, out compilationOutput))
            {
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorMessages.Add(errorMessage);
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return compilationOutput.OutputAssembly.GetType(fullTypeName);
            }
        }
        public CustomCodeCompilationOutput TryCompileCustomCode(CustomCodeCompilationInput input)
        {
            List<string> errorMessages;
            var type = GetOrCreateRuntimeType(input.CustomCode, out errorMessages, input.Classes);
            if (errorMessages != null && errorMessages.Count>0)
            {
                return new CustomCodeCompilationOutput()
                {
                    CompilationSucceeded = false,
                    ErrorMessages = errorMessages
                };
            }
            return new CustomCodeCompilationOutput()
            {
                CompilationSucceeded = true
            };
        }
        public ReportGenerationCustomCodeSettings GetCustomCodeSettingById(Guid vrComponentTypeId)
        {
            return GetVRAutomatedReportQueryDefinitionSettings(vrComponentTypeId);
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
        private string BuildClassDefinition(string customCode, out string fullTypeName, string classes = null)
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
                using Vanrise.Common.Business;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Vanrise.Analytic.Business.IReportGenerationCustomCode
                    {                   
                        public byte[] Generate(IReportGenerationCustomCodeContext context)
                        {
                            #CUSTOMCODE#    
                        } 
                    }
                    #CLASSES#
                }");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.Analytic.Business");
            string className = "ReportGenerationCustomCode";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            classDefinitionBuilder.Replace("#CUSTOMCODE#", customCode);
            classDefinitionBuilder.Replace("#CLASSES#", classes);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);
            return classDefinitionBuilder.ToString();
        }
        #endregion
    }
}
