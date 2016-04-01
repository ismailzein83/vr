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
            ExcelWorkbook ewb = new ExcelWorkbook();
            DataSet data = new DataSet();
            foreach (Worksheet wks in workbook.Worksheets)
            {
               
                if (wks.Cells.MaxDataRow > -1 && wks.Cells.MaxDataColumn > -1)
                    data.Tables.Add(wks.Cells.ExportDataTableAsString(0, 0, wks.Cells.MaxDataRow + 1, wks.Cells.MaxDataColumn + 1));

            }
            foreach(DataTable dt in data.Tables)
            {
                ExcelWorksheet ews = new ExcelWorksheet();
                foreach(DataRow row in dt.Rows)
                {
                    ExcelRow er = new ExcelRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        ExcelCell c = new ExcelCell(){
                            Value = row[col.ColumnName]
                        };
                        er.Cells.Add(c);
                    }
                    ews.Rows.Add(er);
                    ews.NumberOfColumns = dt.Columns.Count;

                }
                ewb.Sheets.Add(ews);
            }

            return ewb;
           // throw new NotImplementedException();
        }
    }
}
