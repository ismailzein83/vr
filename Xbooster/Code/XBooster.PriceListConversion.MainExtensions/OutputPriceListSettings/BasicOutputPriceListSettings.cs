using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.MainExtensions.OutputPriceListSettings
{
    public class BasicOutputPriceListSettings : Entities.OutputPriceListSettings
    {
        public int TemplateFileId { get; set; }
        public int SheetIndex { get; set; }
        public int FirstRowIndex { get; set; }
        public int CodeCellIndex { get; set; }
        public int ZoneCellIndex { get; set; }
        public int RateCellIndex { get; set; }
        public int EffectiveDateCellIndex { get; set; }

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
            var workSheet = workbook.Worksheets[this.SheetIndex];
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            int rowIndex = this.FirstRowIndex;
            int zoneCellIndex = this.ZoneCellIndex;
            int codeCellIndex = this.CodeCellIndex;
            int rateCellIndex = this.RateCellIndex;
            int effectiveDateCellIndex = this.EffectiveDateCellIndex;


            foreach (var item in context.Records)
            {
                workSheet.Cells[rowIndex, zoneCellIndex].PutValue(item.Zone);

                workSheet.Cells[rowIndex, codeCellIndex].PutValue(item.Code);

                workSheet.Cells[rowIndex, rateCellIndex].PutValue(item.Rate);

                workSheet.Cells[rowIndex, effectiveDateCellIndex].PutValue(item.EffectiveDate);
                rowIndex++;
            }


            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();

            return memoryStream.ToArray();
        }
    }
}
