using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using System.IO;
using Aspose.Cells;
using System.Drawing;


namespace Vanrise.Common.Business
{
    public class ExcelFileParserManger
    {
        public List<string> GetUploadedDataValues(long fileId,ExcelFileValueType type)
        {
            List<string> uploadedValues = new List<string>();
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileId);
            byte[] bytes = file.Content;
            MemoryStream memStreamRate = new MemoryStream(bytes);
            Workbook objExcel = new Workbook(memStreamRate);
            Worksheet worksheet = objExcel.Worksheets[0];
            int count = 1;
            while (count < worksheet.Cells.Rows.Count)
            {
                string cellValue = worksheet.Cells[count, 0].StringValue.Trim();
                if (!uploadedValues.Contains(cellValue) && !string.IsNullOrEmpty(cellValue) &&  (type == ExcelFileValueType.String || Vanrise.Common.Utilities.IsNumeric(cellValue, 0))) 
                {                        
                        uploadedValues.Add(cellValue);
                }
                count++;
            }

            return uploadedValues;
        }

        public byte[] DowloadFileExcelParserTemplate(string fieldName)
        {
            Workbook excelTemplate = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            excelTemplate.Worksheets.Clear();
            Worksheet templateSheet = excelTemplate.Worksheets.Add(string.Format("{0} Template", fieldName));
            templateSheet.Cells.SetColumnWidth(0, 20);
            templateSheet.Cells[0, 0].PutValue(fieldName);
            Cell cell = templateSheet.Cells.GetCell(0, 0);
            Style style = cell.GetStyle();
            style.Font.Name = "Times New Roman";
            style.Font.Color = Color.FromArgb(255, 0, 0); ;
            style.Font.Size = 14;
            style.Font.IsBold = true;
            cell.SetStyle(style);
            MemoryStream memoryStream = new MemoryStream();
            excelTemplate.Save(memoryStream, SaveFormat.Xlsx);         
            return memoryStream.ToArray();
        }

    }
}
