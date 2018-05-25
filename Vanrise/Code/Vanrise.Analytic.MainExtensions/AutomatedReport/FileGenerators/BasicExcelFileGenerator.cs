using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class BasicExcelFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7134E751-BFE1-4118-A7E5-AFE32F52B199"); }
        }

        public Guid VRAutomatedReportQueryId { get; set; }

        public string ListName { get; set; }

        public List<BasicExcelFileGeneratorColumnDefinition> Columns { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class BasicExcelFileGeneratorColumnDefinition
    {
        public string FieldName { get; set; }

        //public bool UseFieldDescription { get; set; }
    }
}
