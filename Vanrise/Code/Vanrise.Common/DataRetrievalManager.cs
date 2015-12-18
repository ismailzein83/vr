﻿using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class DataRetrievalManager
    {
        #region Singleton Instance

        static DataRetrievalManager _instance = new DataRetrievalManager();

        public static DataRetrievalManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private DataRetrievalManager()
        {

        }

        #endregion

        public IDataRetrievalResult<T> ProcessResult<T>(DataRetrievalInput dataRetrievalInput, BigResult<T> result)
        {
            if (result == null)
                return null;
            switch (dataRetrievalInput.DataRetrievalResultType)
            {
                case DataRetrievalResultType.Excel: return ExportData(result);
                case DataRetrievalResultType.Normal: return result;
            }
            return null;
        }

        private IDataRetrievalResult<T> ExportData<T>(BigResult<T> result)
        {
            //default Export is Excel

            ExcelResult<T> excelResult = new ExcelResult<T>();
          

            Workbook wbk = new Workbook();
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            wbk.Worksheets.Clear();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Result");
            int rowIndex = 0;
            int colIndex = 0;

            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo entityProperty = null;
            PropertyInfo[] entityProperties = null;

            //filling header
            foreach (var prop in properties)
            {
                if (prop.Name == "Entity")
                {
                    entityProperty = prop;
                    entityProperties = prop.PropertyType.GetProperties();
                    continue;
                }
                RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex,colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            if (entityProperties != null)
            {
                foreach (var prop in entityProperties)
                {
                    RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                    Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                    Style style = cell.GetStyle();
                    style.Font.Name = "Times New Roman";
                    style.Font.Color = Color.FromArgb(255, 0, 0); ;
                    style.Font.Size = 14;
                    style.Font.IsBold = true;
                    cell.SetStyle(style);
                    colIndex++;
                }
            }
            rowIndex++;
            colIndex = 0;

            //filling result
            foreach (var item in result.Data)
            {
                colIndex = 0;
                foreach (var prop in properties)
                {
                    if (prop == entityProperty)
                        continue;
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.GetValue(item));
                    colIndex++;
                }
                if(entityProperty != null)
                {
                    object entityPropertyValue = entityProperty.GetValue(item);
                    if(entityPropertyValue != null)
                    {
                        foreach (var prop in entityProperties)
                        {
                            RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.GetValue(entityPropertyValue));
                            colIndex++;
                        }
                    }
                }

                rowIndex++;             
            }

            
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = wbk.SaveToStream();
           
            excelResult.ExcelFileStream = memoryStream;
            return excelResult;
        }
    }
}
