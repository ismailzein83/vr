using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class FlatFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1ECAA21-462B-4AFD-B886-34A28A35A1FE"); }
        }
        public string FileExtension { get; set; } // default csv
        public string Delimiter { get; set; } // default ,
        public List<FlatFileGeneratorField> Fields { get; set; }
        public bool WithoutHeaders { get; set; }
        public string ListName { get; set; }
        public Guid VRAutomatedReportQueryId { get; set; }
        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            throw new NotImplementedException();
        }
    }
    public class FlatFileGeneratorField
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
    }
}
