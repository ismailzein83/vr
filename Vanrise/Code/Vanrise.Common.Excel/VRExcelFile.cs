using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public class VRExcelFile
    {
        public VRExcelFile()
        {
            this.Sheets = new List<VRExcelSheet>();
        }
        internal List<VRExcelSheet> Sheets { get; set; }

        public void AddSheet(VRExcelSheet sheet)
        {
            Sheets.Add(sheet);
        }
        public byte[] GenerateExcelFile()
        {
            Workbook excelTemplate = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            excelTemplate.Worksheets.Clear();
            foreach (var sheet in this.Sheets)
            {
                Worksheet templateSheet = excelTemplate.Worksheets.Add(string.Format("{0}", sheet.SheetName));
                foreach (var sheetCell in sheet.Cells)
                {
                    Cell cell = templateSheet.Cells[sheetCell.RowIndex, sheetCell.ColumnIndex];
                    cell.PutValue(sheetCell.Value);
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            excelTemplate.Save(memoryStream, SaveFormat.Xlsx);
            return memoryStream.ToArray();
        }
    }

}