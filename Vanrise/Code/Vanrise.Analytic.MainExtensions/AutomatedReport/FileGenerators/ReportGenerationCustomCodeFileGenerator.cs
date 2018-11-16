using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class ReportGenerationCustomCodeFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("482D20EB-78B0-4633-B6F3-4B93B2ED1190"); }
        }
        public Guid ReportGenerationCustomCodeId { get; set; }
        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            ReportGenerationCustomCodeSettingsManager customCodeSettingsManager = new ReportGenerationCustomCodeSettingsManager();
            var customCode = customCodeSettingsManager.GetCustomCodeById(ReportGenerationCustomCodeId);
            if (customCode != null)
            {
                var type = customCodeSettingsManager.GetOrCreateRuntimeType(customCode);
                if (type != null)
                {
                    var reportGenerator = Activator.CreateInstance(type) as IReportGenerationCustomCode;
                    var reportGenerationContext = new ReportGenerationCustomCodeContext();
                    var output = reportGenerator.Generate(reportGenerationContext);
                    return new VRAutomatedReportGeneratedFile()
                    {
                        FileContent = output,
                        FileExtension = ".xlsx"
                    };
                }
            
            }
            return null;
        }
    }
}
