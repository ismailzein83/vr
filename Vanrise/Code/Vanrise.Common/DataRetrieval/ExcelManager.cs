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
    public class ExcelManager
    {
        const string DEFAULT_EXCEL_SHEET_NAME = "Result";

        internal IDataRetrievalResult<T> ExportExcel<T>(BigResult<T> result, ExcelExportHandler<T> exportExcelHandler)
        {
            ExportExcelSheet excelSheet;
            if (exportExcelHandler != null)
            {
                ConvertResultToExcelDataContext<T> convertResultToExcelContext = new ConvertResultToExcelDataContext<T> { BigResult = result };
                exportExcelHandler.ConvertResultToExcelData(convertResultToExcelContext);
                if (convertResultToExcelContext.MainSheet == null)
                    throw new NullReferenceException("convertResultToExcelContext.MainSheet");
                excelSheet = convertResultToExcelContext.MainSheet;
            }
            else
                excelSheet = ConvertResultToDefaultExcelFormat(result);

            return ExportExcel<T>(excelSheet);

            ExcelResult<T> excelResult = new ExcelResult<T>();


            Workbook wbk = new Workbook();
            Common.Utilities.ActivateAspose();
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
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
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
                if (entityProperty != null)
                {
                    object entityPropertyValue = entityProperty.GetValue(item);
                    if (entityPropertyValue != null)
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

        public ExcelResult<T> ExportExcel<T>(ExportExcelSheet excelSheet)
        {
            if (excelSheet == null)
                throw new ArgumentNullException("excelSheet");
            if (excelSheet.Header == null)
                throw new ArgumentNullException("excelSheet.Header");
            if (excelSheet.Header.Cells == null)
                throw new ArgumentNullException("excelSheet.Header.Cells");
            if (excelSheet.Rows == null)
                throw new ArgumentNullException("excelSheet.Rows");
            ExcelResult<T> excelResult = new ExcelResult<T>();

            Workbook wbk = new Workbook();
            Common.Utilities.ActivateAspose();
            wbk.Worksheets.Clear();

            Worksheet RateWorkSheet = wbk.Worksheets.Add(excelSheet.SheetName);
            
            int rowIndex = 0;
            int colIndex = 0;
            
            //filling header
            foreach (var headerCell in excelSheet.Header.Cells)
            {
                RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(headerCell.Title);
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            
            rowIndex++;
            colIndex = 0;

            //filling result
            foreach (var excelRow in excelSheet.Rows)
            {
                if (excelRow.Cells == null)
                    throw new NullReferenceException(String.Format("excelRow.Cells. RowIndex '{0}'", rowIndex));
                if (excelRow.Cells.Count != excelSheet.Header.Cells.Count)
                    throw new Exception(String.Format("Invalid Row Cell Count. RowIndex '{0}'. Row Cell Count '{1}' Header Cell Count '{2}'", rowIndex, excelRow.Cells.Count, excelSheet.Header.Cells.Count));
                colIndex = 0;
                foreach(var cell in excelRow.Cells)
                {
                    var excelCell = RateWorkSheet.Cells[rowIndex, colIndex];
                    if (colIndex >= excelSheet.Header.Cells.Count)
                        throw new Exception(String.Format("Cell Index '{0}' in row '{1}' is greater than header cell count '{2}'", colIndex, rowIndex, excelSheet.Header.Cells.Count));
                    var headerCell = excelSheet.Header.Cells[colIndex];
                    if (headerCell.CellType.HasValue)
                        SetExcelCellFormat(excelCell, headerCell);                        
                    excelCell.PutValue(cell.Value);
                    colIndex++;
                }

                rowIndex++;
            }


            MemoryStream memoryStream = new MemoryStream();
            memoryStream = wbk.SaveToStream();

            excelResult.ExcelFileStream = memoryStream;
            return excelResult;
        }

        private void SetExcelCellFormat(Cell excelCell, ExportExcelHeaderCell headerCell)
        {
            var cellStyle = excelCell.GetDisplayStyle();
            switch(headerCell.CellType.Value)
            {
                case ExcelCellType.DateTime: 
                    if (!headerCell.DateTimeType.HasValue)
                        throw new NullReferenceException("headerCell.DateTimeType");
                    cellStyle.Custom = Utilities.GetDateTimeFormat(headerCell.DateTimeType.Value);
                    break;
            }
        }

        private ExportExcelSheet ConvertResultToDefaultExcelFormat<T>(BigResult<T> result)
        {
            ExportExcelSheet excelSheet = new ExportExcelSheet() { SheetName = DEFAULT_EXCEL_SHEET_NAME };
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo entityProperty = null;
            PropertyInfo[] entityProperties = null;

            excelSheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
            //filling header
            foreach (var prop in properties)
            {
                if (prop.Name == "Entity")
                {
                    entityProperty = prop;
                    entityProperties = prop.PropertyType.GetProperties();
                    continue;
                }
                excelSheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = prop.Name });
            }
            if (entityProperties != null)
            {
                foreach (var prop in entityProperties)
                {
                    excelSheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = prop.Name });                   
                }
            }

            excelSheet.Rows = new List<ExportExcelRow>();
            //filling result
            foreach (var item in result.Data)
            {
                ExportExcelRow excelRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                excelSheet.Rows.Add(excelRow);
                foreach (var prop in properties)
                {
                    if (prop == entityProperty)
                        continue;
                    excelRow.Cells.Add(new ExportExcelCell { Value = prop.GetValue(item) });
                }
                if (entityProperty != null)
                {
                    object entityPropertyValue = entityProperty.GetValue(item);
                    if (entityPropertyValue != null)
                    {
                        foreach (var prop in entityProperties)
                        {
                            excelRow.Cells.Add(new ExportExcelCell { Value = prop.GetValue(entityPropertyValue) });
                        }
                    }
                }
            }
            return excelSheet;
        }
    }
}
