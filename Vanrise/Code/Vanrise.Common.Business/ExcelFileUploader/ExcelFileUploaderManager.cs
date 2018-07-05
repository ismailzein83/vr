using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Excel;
using Vanrise.Entities;

namespace Vanrise.Common.Business.ExcelFileUploader
{
    public class ExcelFileUploaderManager
    {

        public ExcelUploaderOutput UploadExcelFile(ExcelUploaderInput input)
        {
            ExcelUploaderOutput output = new ExcelUploaderOutput();
            output.IsSucceeded = false;

            if (input.Sheets == null || input.Sheets.Count == 0)
            {
                return output;
            }

            VRFile file;
            VRExcelSheet excelSheet;
            VRExcelFile excelFile = new VRExcelFile();
            VRFileManager fileManager = new VRFileManager();
                     
            foreach (string sheet in input.Sheets)
            {
                excelSheet = new VRExcelSheet()
                {
                   SheetName = sheet
                };
                excelFile.AddSheet(excelSheet);
            }
                      
            byte[] byteArray = excelFile.GenerateExcelFile();

            file = new VRFile()
            {
                Content = byteArray,
                Name = "MyExcel.xlsx",
                Extension = "xlsx"
            };
            output.FileId = fileManager.AddFile(file);
            output.IsSucceeded = true;

            return output;
        }

    }
}
