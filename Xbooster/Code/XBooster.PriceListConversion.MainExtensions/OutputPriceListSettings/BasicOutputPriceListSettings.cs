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
            
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");

           
           
            foreach (var table in this.Tables)
            {
                var workSheet = workbook.Worksheets[table.SheetIndex];
                var rowIndex = table.RowIndex;
                foreach (var item in context.Records)
                {
                    for (var i = 0; i <= workSheet.Cells.Rows[table.RowIndex].LastCell.Column ; i++)
                    {
                        foreach (var field in table.FieldsMapping)
                        {
                            if (this.RepeatOtherValues && i != field.CellIndex)
                            {
                                workSheet.Cells[rowIndex, i].PutValue(workSheet.Cells[table.RowIndex, i].Value);
                            }
                            else if (i == field.CellIndex)
                            {
                                FieldValueExecutionContext fieldValueExecutionContext = new Business.FieldValueExecutionContext();
                                fieldValueExecutionContext.Record = item;
                                field.FieldValue.Execute(fieldValueExecutionContext);
                                var fieldValue = fieldValueExecutionContext.FieldValue;
                                if (fieldValueExecutionContext.FieldValue is DateTime)
                                    fieldValue = ((DateTime)fieldValueExecutionContext.FieldValue).ToString(this.DateTimeFormat);
                                workSheet.Cells[rowIndex, field.CellIndex].PutValue(fieldValue);
                            }


                        }
                    }
                   
                    rowIndex++;
                 }
            }
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();
            return memoryStream.ToArray();
        }
    }
}
