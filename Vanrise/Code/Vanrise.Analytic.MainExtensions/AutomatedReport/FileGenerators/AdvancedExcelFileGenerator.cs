using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9FAAE9B2-931E-4B3F-BDA4-B0F3B7647488"); }
        }

        public long? FileTemplateId { get; set; }

        public List<AdvancedExcelFileGeneratorTableDefinition> TableDefinitions { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            if (this.TableDefinitions != null)
            {
                foreach (var tableDef in this.TableDefinitions)
                {
                    var dataList = context.HandlerContext.GetDataList(tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    string dataListIdentifier = string.Format("{0}_{1}", tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    dataList.ThrowIfNull("dataList", dataListIdentifier);
                    dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);

                    foreach (var item in dataList.Items)
                    {

                    }
                }
            }
            throw new NotImplementedException();
        }
    }

    public class AdvancedExcelFileGeneratorTableDefinition
    {
        public Guid VRAutomatedReportQueryId { get; set; }

        public string ListName { get; set; }

        public string SheetName { get; set; }

        public int RowIndex { get; set; }

        public List<AdvancedExcelFileGeneratorTableColumnDefinition> ColumnDefinitions { get; set; }
    }

    public class AdvancedExcelFileGeneratorTableColumnDefinition
    {
        public string FieldName { get; set; }

        public int ColumnIndex { get; set; }

        public bool UseFieldDescription { get; set; }
    }
}
