using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGeneratorManager
    {
        public VRAutomatedReportGeneratedFile DownloadAttachmentGenerator(DownloadAttachmentGeneratorInput input)
        {
            input.FileGenerator.Settings.ThrowIfNull("input.FileGenerator.Settings");
            input.Queries.ThrowIfNull("input.Queries");
            var generatedFile = input.FileGenerator.Settings.GenerateFile(new VRAutomatedReportFileGeneratorGenerateFileContext()
            {
                HandlerContext = new VRAutomatedReportHandlerExecuteContext(input.Queries, null, null)
            });
            return generatedFile;
        }
    }
}
