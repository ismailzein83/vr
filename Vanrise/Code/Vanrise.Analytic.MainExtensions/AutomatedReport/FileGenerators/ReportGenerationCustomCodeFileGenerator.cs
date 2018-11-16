using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Excel;

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
            ReportGenerationCustomCodeManager customCodeSettingsManager = new ReportGenerationCustomCodeManager();
            var type = customCodeSettingsManager.GetCustomCodeRuntimeType(ReportGenerationCustomCodeId);
            if (type != null)
            {
                var reportGenerator = Activator.CreateInstance(type) as IReportGenerationCustomCode;
                var reportGenerationContext = new ReportGenerationCustomCodeContext(context.HandlerContext.GetDataList);


                //var vrExcelFile = reportGenerationContext.CreateExcelFile();
                //var sheet = vrExcelFile.CreateSheet();

                //var table = sheet.CreateTable(1, 1);
                //table.EnableMergeHeaders();
                //table.EnableTableBorders();

                //var incomingHeaderRow = table.CreateHeaderRow();
                //var incomingHeaderCell = incomingHeaderRow.CreateCell();
                //incomingHeaderCell.SetValue("Incoming");
                //var incomingHeaderCellStyle = incomingHeaderCell.CreateStyle();
                //incomingHeaderCellStyle.BGColor = "White";
                //incomingHeaderCellStyle.FontSize = 11;
                //incomingHeaderCellStyle.IsBold = true;
                //incomingHeaderCellStyle.FontColor = "Black";
                //incomingHeaderCellStyle.HorizontalAlignment = VRExcelContainerHorizontalAlignment.Left;
                //var emptyRow = table.CreateHeaderRow();

                var output = reportGenerator.Generate(reportGenerationContext);
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
