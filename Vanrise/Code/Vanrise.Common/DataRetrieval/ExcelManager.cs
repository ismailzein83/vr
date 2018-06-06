using Aspose.Cells;
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

        int _normalPrecision;
        int _longPrecision;  

        public ExcelManager()
        {
            IGeneralSettingsManager generalSettingsManager = BusinessManagerFactory.GetManager<IGeneralSettingsManager>();
            _normalPrecision = generalSettingsManager.GetNormalPrecisionValue();
            _longPrecision = generalSettingsManager.GetLongPrecisionValue();
        }

        internal IDataRetrievalResult<T> ExportExcel<T>(BigResult<T> result, ExcelExportHandler<T> exportExcelHandler, DataRetrievalInput input)
        {
            ExportExcelSheet excelSheet;
            if (exportExcelHandler != null)
            {
                ConvertResultToExcelDataContext<T> convertResultToExcelContext = new ConvertResultToExcelDataContext<T> { BigResult = result, Input = input };
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
                style.Font.Size = 12;
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
                    style.Font.Size = 12;
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

            Workbook wbk = new Workbook(FileFormatType.Xlsx);
            Common.Utilities.ActivateAspose();
            wbk.Worksheets.Clear();

            Worksheet RateWorkSheet = wbk.Worksheets.Add(excelSheet.SheetName);

            int rowIndex = 0;
            int colIndex = 0;

            if (excelSheet.SummaryRows != null)
            {
                foreach (var summaryRow in excelSheet.SummaryRows)
                {
                    if (summaryRow.Cells == null)
                        throw new NullReferenceException(String.Format("excelRow.Cells. RowIndex '{0}'", rowIndex));
                    colIndex = 0;
                    foreach (var cell in summaryRow.Cells)
                    {
                        var excelCell = RateWorkSheet.Cells[rowIndex, colIndex];
                        excelCell.PutValue(cell.Value);
                        colIndex++;
                    }
                    rowIndex++;
                }
            }
            colIndex = 0;
            //filling header
            foreach (var headerCell in excelSheet.Header.Cells)
            {
                if (headerCell.Width.HasValue)
                    RateWorkSheet.Cells.SetColumnWidth(colIndex, headerCell.Width.Value);
                else
                    RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);

                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(headerCell.Title);
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0);

                style.Font.Size = 12;
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
                foreach (var cell in excelRow.Cells)
                {
                    var excelCell = RateWorkSheet.Cells[rowIndex, colIndex];
                    if (colIndex >= excelSheet.Header.Cells.Count)
                        throw new Exception(String.Format("Cell Index '{0}' in row '{1}' is greater than header cell count '{2}'", colIndex, rowIndex, excelSheet.Header.Cells.Count));
                    var headerCell = excelSheet.Header.Cells[colIndex];
                    SetExcelCellFormat(excelCell, headerCell, cell.Style);
                    excelCell.PutValue(cell.Value);
                    colIndex++;
                }

                rowIndex++;
            }

            if (excelSheet.AutoFitColumns)
                RateWorkSheet.AutoFitColumns();

            MemoryStream memoryStream = new MemoryStream();

            wbk.Save(memoryStream, SaveFormat.Xlsx);
            memoryStream.Seek(0, SeekOrigin.Begin);

            excelResult.ExcelFileStream = memoryStream;
            return excelResult;
        }

        public ExcelResult ExportExcel(List<ExportExcelSheet> excelSheets)
        {
            Workbook wbk = new Workbook();
            Common.Utilities.ActivateAspose();
            wbk.Worksheets.Clear();
            ExcelResult excelResult = new ExcelResult();

            foreach (var excelSheet in excelSheets)
            {
                BuildWorkBookSheet(wbk, excelSheet);
            }
            MemoryStream memoryStream = new MemoryStream();
            wbk.Save(memoryStream, SaveFormat.Xlsx);
            memoryStream.Seek(0, SeekOrigin.Begin);

            excelResult.ExcelFileStream = memoryStream;
            return excelResult;
        }

        private void BuildWorkBookSheet(Workbook wbk, ExportExcelSheet excelSheet)
        {
            ValidateSheet(excelSheet);
            Worksheet workSheet = wbk.Worksheets.Add(excelSheet.SheetName);
            int rowIndex = 0;
            int colIndex = 0;
            BuildHeaderCells(workSheet, excelSheet.Header.Cells, rowIndex, colIndex);  //filling header
            rowIndex++;
            colIndex = 0;
            BuildSheetRows(workSheet, excelSheet, excelSheet.Rows, rowIndex, colIndex);//filling result
            if (excelSheet.AutoFitColumns)
                workSheet.AutoFitColumns();
        }

        private void BuildHeaderCells(Worksheet workSheet, List<ExportExcelHeaderCell> exportExcelHeaderCells, int rowIndex, int colIndex)
        {
            foreach (var headerCell in exportExcelHeaderCells)
            {
                workSheet.Cells.SetColumnWidth(colIndex, 20);
                workSheet.Cells[rowIndex, colIndex].PutValue(headerCell.Title);
                Cell cell = workSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 12;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
        }

        private void BuildSheetRows(Worksheet workSheet, ExportExcelSheet excelSheet, List<ExportExcelRow> exportExcelRows, int rowIndex, int colIndex)
        {
            foreach (var excelRow in exportExcelRows)
            {
                if (excelRow.Cells == null)
                    throw new NullReferenceException(String.Format("excelRow.Cells. RowIndex '{0}'", rowIndex));
                if (excelRow.Cells.Count != excelSheet.Header.Cells.Count)
                    throw new Exception(String.Format("Invalid Row Cell Count. RowIndex '{0}'. Row Cell Count '{1}' Header Cell Count '{2}'", rowIndex, excelRow.Cells.Count, excelSheet.Header.Cells.Count));
                colIndex = 0;
                foreach (var cell in excelRow.Cells)
                {
                    var excelCell = workSheet.Cells[rowIndex, colIndex];
                    if (colIndex >= excelSheet.Header.Cells.Count)
                        throw new Exception(String.Format("Cell Index '{0}' in row '{1}' is greater than header cell count '{2}'", colIndex, rowIndex, excelSheet.Header.Cells.Count));
                    var headerCell = excelSheet.Header.Cells[colIndex];
                    SetExcelCellFormat(excelCell, headerCell, cell.Style);
                    excelCell.PutValue(cell.Value);
                    colIndex++;
                }

                rowIndex++;
            }
        }

        private void ValidateSheet(ExportExcelSheet excelSheet)
        {
            if (excelSheet == null)
                throw new ArgumentNullException("excelSheet");
            if (excelSheet.Header == null)
                throw new ArgumentNullException("excelSheet.Header");
            if (excelSheet.Header.Cells == null)
                throw new ArgumentNullException("excelSheet.Header.Cells");
            if (excelSheet.Rows == null)
                throw new ArgumentNullException("excelSheet.Rows");
        }

        private void SetExcelCellFormat(Cell excelCell, ExportExcelHeaderCell headerCell, ExcelCellStyle style)
        {
            var cellStyle = excelCell.GetDisplayStyle();

            if (headerCell != null && headerCell.CellType.HasValue)
            {
                switch (headerCell.CellType.Value)
                {
                    case ExcelCellType.DateTime:
                        if (!headerCell.DateTimeType.HasValue)
                            throw new NullReferenceException("headerCell.DateTimeType");
                        cellStyle.Custom = Utilities.GetDateTimeFormat(headerCell.DateTimeType.Value);
                        break;

                    case ExcelCellType.Number:
                        if (!headerCell.NumberType.HasValue)
                            throw new NullReferenceException("headerCell.NumberType");
                        cellStyle.Custom = this.GetNumberFormat(headerCell.NumberType.Value);
                        break;
                }
            }

            if (style != null)
            {
                cellStyle.Font.IsBold = style.IsBold;
                if (style.Color.HasValue)
                {
                    var color = LabelColorManager.GetLabelParsedColor(style.Color.Value);
                    cellStyle.BackgroundColor = color;
                    cellStyle.ForegroundColor = color;
                    cellStyle.Pattern = BackgroundType.VerticalStripe;
                    cellStyle.Font.Color = Color.White;
                }
            }

            excelCell.SetStyle(cellStyle);
        }

        private string GetNumberFormat(Vanrise.Entities.NumberType numberType)
        {
            switch (numberType)
            {
                case Vanrise.Entities.NumberType.Int:
                case Vanrise.Entities.NumberType.BigInt: return "#,##0";
                case Vanrise.Entities.NumberType.NormalDecimal:
                    return string.Concat("#,##0.", new StringBuilder(_normalPrecision).Insert(0, "0", _normalPrecision).ToString());
                case Vanrise.Entities.NumberType.LongDecimal:
                    return string.Concat("#,##0.", new StringBuilder(_longPrecision).Insert(0, "0", _longPrecision).ToString());
                default: throw new NotSupportedException(String.Format("numberType '{0}'", numberType));
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
