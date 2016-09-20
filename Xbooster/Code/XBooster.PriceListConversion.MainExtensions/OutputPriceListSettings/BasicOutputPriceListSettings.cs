using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using XBooster.PriceListConversion.Business;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.MainExtensions.OutputPriceListSettings
{
    public class BasicOutputPriceListSettings : Entities.OutputPriceListSettings
    {
        public override Guid ConfigId { get { return new Guid("66268265-af69-42d7-aaed-9185ea2fe466"); } }

        public int TemplateFileId { get; set; }
        public List<OutputTable> Tables { get; set; }
        public string DateTimeFormat { get; set; }
        public bool RepeatOtherValues { get; set; }
        public override byte[] Execute(IOutputPriceListExecutionContext context)
        {
            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(this.TemplateFileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", this.TemplateFileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", this.TemplateFileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            Vanrise.Common.Utilities.ActivateAspose();
           
            GenerateOutputExcel(context, workbook);
 
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();
            return memoryStream.ToArray();
        }
        private void GenerateOutputExcel(IOutputPriceListExecutionContext context,Workbook workbook)
        {
            foreach (var table in this.Tables)
            {
                var workSheet = workbook.Worksheets[table.SheetIndex];
                AddRecordToExcel(context, workSheet, table);
            }
        }
        private void AddRecordToExcel(IOutputPriceListExecutionContext context, Worksheet workSheet,OutputTable table)
        {
            var rowIndex = table.RowIndex;
            foreach (var item in context.Records)
            {
                foreach (var code in item.Codes)
                {
                    var lastCell = workSheet.Cells.Rows[table.RowIndex].LastCell;
                    if (lastCell != null)
                    {
                        for (var i = 0; i <= lastCell.Column; i++)
                        {
                            FillCellDataForEachMappingField(table, code, item, workSheet, rowIndex, i);
                        }
                    }
                    else
                    {
                        FillCellDataForEachMappingField(table, code, item, workSheet, rowIndex, null);
                    }

                    rowIndex++;
                }
            }

        }
        private void FillCellDataForEachMappingField(OutputTable table, PriceListCode code,PriceListRecord item, Worksheet workSheet, int rowIndex, int? cellIndex)
        {
            if (cellIndex != null && this.RepeatOtherValues && !table.FieldsMapping.Any(x => x.CellIndex == cellIndex))
            {
                workSheet.Cells[rowIndex, (int)cellIndex].PutValue(workSheet.Cells[table.RowIndex, (int)cellIndex].Value);
            }
            else
            {
                foreach (var field in table.FieldsMapping)
                {
                    FillCellData(code, item, field, workSheet, rowIndex);
                }
            }
            

        }
        private void FillCellData(PriceListCode code, PriceListRecord item, OutputFieldMapping field, Worksheet workSheet, int rowIndex)
        {
            FieldValueExecutionContext fieldValueExecutionContext = new Business.FieldValueExecutionContext();
            fieldValueExecutionContext.EffectiveDate = code.CodeEffectiveDate;
            fieldValueExecutionContext.Rate = item.Rate;
            fieldValueExecutionContext.Zone = item.Zone;
            fieldValueExecutionContext.Code = code.Code;

            field.FieldValue.Execute(fieldValueExecutionContext);

            var fieldValue = fieldValueExecutionContext.FieldValue;
            if (fieldValueExecutionContext.FieldValue is DateTime)
                fieldValue = ((DateTime)fieldValueExecutionContext.FieldValue).ToString(this.DateTimeFormat);
            workSheet.Cells[rowIndex, field.CellIndex].PutValue(fieldValue);
        }
    }
}
