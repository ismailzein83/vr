using Aspose.Cells;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace ExcelConversion.Business
{
    public class ExcelManager
    {
        public ExcelWorkbook ReadExcelFile(long fileId)
        {
            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(fileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", fileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", fileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            ExcelWorkbook ewb = new ExcelWorkbook() { Sheets = new List<ExcelWorksheet>() };
            foreach(var sheet in workbook.Worksheets)
            {
                ExcelWorksheet eSheet = new ExcelWorksheet() { Rows = new List<ExcelRow>() };
                ewb.Sheets.Add(eSheet);
                eSheet.Name = sheet.Name;
                int nbOfSheetColumns = 0;
                foreach(Row row in sheet.Cells.Rows)
                {
                    ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };
                    eSheet.Rows.Add(eRow);
                    int nbOfRowColumns = row.LastCell.Column + 1;
                    if (nbOfRowColumns > nbOfSheetColumns)
                        nbOfSheetColumns = nbOfRowColumns;
                    for (int colIndex = 0; colIndex < nbOfRowColumns; colIndex++)
                    {
                        var cell = row.GetCellOrNull(colIndex);
                        ExcelCell eCell = new ExcelCell();
                        if (cell != null)
                            eCell.Value = cell.Value;
                        eRow.Cells.Add(eCell);
                    }
                }
                eSheet.NumberOfColumns = nbOfSheetColumns;
            }

            return ewb;
        }
    }
}
