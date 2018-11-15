using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class ReportGenerationCustomCodeFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("482D20EB-78B0-4633-B6F3-4B93B2ED1190"); }
        }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            throw new NotImplementedException();
        }
    }
}
