using Aspose.Cells;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace ExcelConversion.MainExtensions.OutputPriceList
{
    public class OutputPriceListSettings : OutputPriceListConfiguration
    {
        public int FileId { get; set; }
        public OutputPriceListFields OutputPriceListFields { get; set; }

        public override byte[] Execute(IOutputPriceListContext context)
        {
            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(this.FileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", this.FileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", this.FileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);
            var workSheet = workbook.Worksheets[this.OutputPriceListFields.SheetIndex];
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            int rowIndex = this.OutputPriceListFields.FirstRowIndex;
            int zoneCellIndex = this.OutputPriceListFields.ZoneCellIndex;
            int codeCellIndex = this.OutputPriceListFields.CodeCellIndex;
            int rateCellIndex = this.OutputPriceListFields.RateCellIndex;
            int effectiveDateCellIndex = this.OutputPriceListFields.EffectiveDateCellIndex;


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
