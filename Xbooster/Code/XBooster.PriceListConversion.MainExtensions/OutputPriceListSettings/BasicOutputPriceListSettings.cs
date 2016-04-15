using Aspose.Cells;
using System;
using System.Collections.Generic;
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

            FieldValueExecutionContext fieldValueExecutionContext = new Business.FieldValueExecutionContext();
            foreach (var item in context.Records)
            {
                foreach (var table in this.Tables)
                {
                    var workSheet = workbook.Worksheets[table.SheetIndex];
                    foreach (var field in table.FieldsMapping)
                    {

                        fieldValueExecutionContext.Record = item;
                        field.FieldValue.Execute(fieldValueExecutionContext);
                        workSheet.Cells[table.RowIndex, field.CellIndex].PutValue(fieldValueExecutionContext.FieldValue);
                    }
                    table.RowIndex++;
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();
            return memoryStream.ToArray();
        }
    }
}
