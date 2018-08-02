using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Excel;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business.ExcelFileUploader
{
    public class ExcelFileUploaderManager
    {
        VRFileManager fileManager = new VRFileManager();

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
                Extension = "xlsx",
                FileUniqueId = Guid.NewGuid()
            };
            output.FileId = fileManager.AddFile(file);
            output.IsSucceeded = true;
            output.FileUniqueId = file.FileUniqueId.Value;
            return output;
        }
    }
}
