using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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
        public VRExcelSheet CreateSheet()
        {
            var sheet = new VRExcelSheet();
            Sheets.Add(sheet);
            return sheet;
        }
        public byte[] GenerateExcelFile()
        {
            Workbook excelTemplate = new Workbook();
            ExcelUtilities.ActivateAspose();
            excelTemplate.Worksheets.Clear();

            VRExcelFileGenerateContext context = new VRExcelFileGenerateContext(this.Sheets);

            foreach (var sheet in this.Sheets)
            {
                Worksheet templateSheet = null;
                switch (sheet.SheetType)
                {
                    case VRExcelSheetType.Chart:
                        int chartIndex = excelTemplate.Worksheets.Add(SheetType.Chart);
                        templateSheet = excelTemplate.Worksheets[chartIndex];
                        templateSheet.Name = string.Format("{0}", sheet.SheetName);
                        break;
                    default:templateSheet = excelTemplate.Worksheets.Add(string.Format("{0}", sheet.SheetName));
                        break;
                }

                BuildImagesConfigs(templateSheet, sheet.Images);
                BuildSheetRowsConfig(sheet.RowConfigs, excelTemplate, templateSheet.Cells);
                BuildSheetColumnsConfig(sheet.ColumnConfigs, excelTemplate, templateSheet.Cells);

                foreach (var sheetCell in sheet.Cells)
                {
                    Cell cell = templateSheet.Cells[sheetCell.RowIndex, sheetCell.ColumnIndex];
                    cell.PutValue(sheetCell.Value);
                    
                    if(sheetCell.EndRowIndex.HasValue && sheetCell.EndColumnIndex.HasValue)
                    {
                        templateSheet.Cells.Merge(sheetCell.RowIndex, sheetCell.ColumnIndex, sheetCell.EndRowIndex.Value - sheetCell.RowIndex, sheetCell.EndColumnIndex.Value - sheetCell.ColumnIndex);
                    }
                    if (sheetCell.Style != null)
                    {
                        var style = BuildSheetContainerCommonConfigs(sheetCell.Style, excelTemplate);
                        var styleFlag = BuildSheetContainerCommonConfigs(sheetCell.Style);
                        cell.SetStyle(style, styleFlag);
                    }
                }
                if (sheet.Tables != null && sheet.Tables.Count > 0)
                {
                    foreach (var table in sheet.Tables)
                    {
                        table.GenerateTable(templateSheet);
                    }
                }

                if (sheet.PivotTables != null && sheet.PivotTables.Count > 0)
                {
                    foreach (var pivotTables in sheet.PivotTables)
                    {
                        pivotTables.GeneratePivotTable(context, templateSheet);
                    }
                }
                if (sheet.Charts != null && sheet.Charts.Count > 0)
                {
                    foreach (var chart in sheet.Charts)
                    {
                        chart.GenerateChart(context, templateSheet);
                    }
                }

            }
            for (var i = 0; i < this.Sheets.Count(); i++)
            {
                var sheet = this.Sheets[i];
                excelTemplate.Worksheets[i].IsVisible = !sheet.HideSheet;
            }

                MemoryStream memoryStream = new MemoryStream();
            excelTemplate.Save(memoryStream, SaveFormat.Xlsx);
            return memoryStream.ToArray();
        }

        void BuildImagesConfigs(Worksheet sheet, List<VRExcelImageConfig> images)
        {
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                {
                    MemoryStream memoryStream = new MemoryStream(image.Value);
                    sheet.Pictures.Add(image.StartingRowIndex, image.StartingColumnIndex, image.StartingRowIndex + image.NumberOFRows, image.StartingColumnIndex + image.NumberOfColumns, memoryStream);
                }
            }
        }
        void BuildSheetRowsConfig(Dictionary<int, VRExcelRowConfig> rowConfigs, Workbook excelTemplate, Cells cells)
        {
            foreach (var config in rowConfigs.Values)
            {
                int rowIndex = config.RowIndex;
                Row row = cells.Rows[rowIndex];
                if (config.RowHeight.HasValue)
                    row.Height = config.RowHeight.Value;
                var style = BuildSheetContainerCommonConfigs(config, excelTemplate);
                var styleFlag = BuildSheetContainerCommonConfigs(config);
                row.ApplyStyle(style, styleFlag);
            }

        }

        void BuildSheetColumnsConfig(Dictionary<int, VRExcelColumnConfig> ColumnConfigs, Workbook excelTemplate, Cells cells)
        {
            foreach (var config in ColumnConfigs.Values)
            {
                int ColumnIndex = config.ColumnIndex;
                Column Column = cells.Columns[ColumnIndex];
                if (config.ColumnWidth.HasValue)
                    Column.Width = config.ColumnWidth.Value;
                var style = BuildSheetContainerCommonConfigs(config, excelTemplate);
                var styleFlag = BuildSheetContainerCommonConfigs(config);
                Column.ApplyStyle(style, styleFlag);
            }

        }
        Style BuildSheetContainerCommonConfigs(VRExcelContainerConfig config, Workbook excelTemplate)
        {
            Style style = excelTemplate.CreateStyle();
            if (config.SetBorder == true)
            {
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            }

            if (config.HorizontalAlignment.HasValue)
            {
                switch (config.HorizontalAlignment.Value)
                {
                    case VRExcelContainerHorizontalAlignment.Left:
                        style.HorizontalAlignment = TextAlignmentType.Left;
                        break;
                    case VRExcelContainerHorizontalAlignment.Center:
                        style.HorizontalAlignment = TextAlignmentType.Center;
                        break;
                    case VRExcelContainerHorizontalAlignment.Right:
                        style.HorizontalAlignment = TextAlignmentType.Right;
                        break;
                }
            }
            if (config.VerticalAlignment.HasValue)
            {
                switch (config.VerticalAlignment.Value)
                {
                    case VRExcelContainerVerticalAlignment.Top:
                        style.VerticalAlignment = TextAlignmentType.Top;
                        break;
                    case VRExcelContainerVerticalAlignment.Center:
                        style.VerticalAlignment = TextAlignmentType.Center;
                        break;
                    case VRExcelContainerVerticalAlignment.Bottom:
                        style.VerticalAlignment = TextAlignmentType.Bottom;
                        break;
                }
            }
            //if (config.SetBorder == true) 
            //{
            //    style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.LightGray); //Border color matching original cell border color
            //    style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.LightGray);
            //    style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.LightGray);
            //    style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.LightGray);
            //}
            if (!string.IsNullOrEmpty(config.BGColor))
                style.ForegroundColor = ColorTranslator.FromHtml(config.BGColor);
            style.Pattern = BackgroundType.Solid;
            if (!string.IsNullOrEmpty(config.FontColor))
                style.Font.Color = ColorTranslator.FromHtml(config.FontColor);
            if (config.FontSize.HasValue)
                style.Font.Size = config.FontSize.Value;
            style.Font.IsBold = config.IsBold;
            style.Font.IsItalic = config.IsItalic;
            style.Pattern = BackgroundType.Solid;
            return style;
        }

        StyleFlag BuildSheetContainerCommonConfigs<T>(T config)where T: VRExcelContainerConfig
        {
            StyleFlag styleFlag = new StyleFlag();
            styleFlag.All = false;
            styleFlag.Font = true;
            styleFlag.Borders = true;
            //styleFlag.HorizontalAlignment = config.HorizontalAlignment.HasValue;
            //styleFlag.VerticalAlignment = config.VerticalAlignment.HasValue;
            return styleFlag;
        }



    }

    public interface IVRExcelFileGenerateContext
    {
       string GetSheetName(int sheetIndex);
    }
    public class VRExcelFileGenerateContext: IVRExcelFileGenerateContext
    {
        List<VRExcelSheet> _sheets;
        public VRExcelFileGenerateContext(List<VRExcelSheet> sheets)
        {
            _sheets = sheets;
        }
        public string GetSheetName(int sheetIndex)
        {
            if (_sheets != null && _sheets.Count() >= sheetIndex)
            {
                var sheet = _sheets[sheetIndex];
                return sheet.SheetName;
            }
            return null;
        }
    }

}