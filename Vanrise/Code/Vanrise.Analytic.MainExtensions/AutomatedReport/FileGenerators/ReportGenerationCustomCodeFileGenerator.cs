using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Excel;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class ReportGenerationCustomCodeFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("482D20EB-78B0-4633-B6F3-4B93B2ED1190"); }
        }
        public Guid ReportGenerationCustomCodeId { get; set; }
        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context1)
        {
            ReportGenerationCustomCodeManager customCodeSettingsManager = new ReportGenerationCustomCodeManager();
            var type = customCodeSettingsManager.GetCustomCodeRuntimeType(ReportGenerationCustomCodeId);
            if (type != null)
            {
                var reportGenerator = Activator.CreateInstance(type) as IReportGenerationCustomCode;
                var context = new ReportGenerationCustomCodeContext(context1.HandlerContext.GetDataList, context1.HandlerContext.GetSubTableIdByGroupingFields);

                var output = reportGenerator.Generate(context);
                return new VRAutomatedReportGeneratedFile()
                {
                    FileContent = output,
                    FileExtension = ".xlsx"
                };
            }
            return null;
            
        }
    }
}
