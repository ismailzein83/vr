using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class RemoteExcelManager
    {
        internal IDataRetrievalResult<T> ExportExcel<T>(IDataRetrievalResult<T> dataRetrievalResult)
        {
            ExcelResult<T> excelResult = dataRetrievalResult as ExcelResult<T>;
            return new RemoteExcelResult<T>() { Data = excelResult.ExcelFileStream.ToArray() };
        }
    }
}