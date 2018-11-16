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
    public class ReportGenerationCustomCodeManager
    {
        VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
        #region Public Methods

        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        CacheManager GetCacheManager()
        {
            return s_cacheManager;
        }

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
            var runtimeType = GetCacheManager().GetOrCreateObject(cacheName,
                () =>
                {
                    var customCode = GetCustomCodeById(customCodeId);
                    if (customCode != null)
                    {
                        List<string> errorMessages;
                        var type = GetOrCreateRuntimeType(customCode, out errorMessages);
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
        public Type GetOrCreateRuntimeType(string customCode, out List<string> errorMessages)
        {
            string fullTypeName;
            var classDefinition = BuildClassDefinition(customCode, out fullTypeName);
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
    public class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        object _updateHandle;

        protected override bool ShouldSetCacheExpired()
        {
            return base.ShouldSetCacheExpired();
        }

    }
}
