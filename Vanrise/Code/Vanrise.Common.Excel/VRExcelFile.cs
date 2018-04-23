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
        public byte[] GenerateExcelFile()
        {
            Workbook excelTemplate = new Workbook();
            Vanrise.Common.Utilities.ActivateAspose();
            excelTemplate.Worksheets.Clear();
            foreach (var sheet in this.Sheets)
            {
                Worksheet templateSheet = excelTemplate.Worksheets.Add(string.Format("{0}", sheet.SheetName));

                BuildSheetRowsConfig(sheet.RowConfigs, excelTemplate, templateSheet.Cells);
                BuildSheetColumnsConfig(sheet.ColumnConfigs, excelTemplate, templateSheet.Cells);

                foreach (var sheetCell in sheet.Cells)
                {
                    Cell cell = templateSheet.Cells[sheetCell.RowIndex, sheetCell.ColumnIndex];
                    cell.PutValue(sheetCell.Value);
                    if (sheetCell.Style != null)
                    {
                        var style = BuildSheetContainerCommonConfigs(sheetCell.Style, excelTemplate);
                        var styleFlag = BuildSheetContainerCommonConfigs();
                        cell.SetStyle(style, styleFlag);
                    }
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            excelTemplate.Save(memoryStream, SaveFormat.Xlsx);
            return memoryStream.ToArray();
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
                var styleFlag = BuildSheetContainerCommonConfigs();
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
                var styleFlag = BuildSheetContainerCommonConfigs();
                Column.ApplyStyle(style, styleFlag);
            }

        }
        Style BuildSheetContainerCommonConfigs(VRExcelContainerConfig config, Workbook excelTemplate)
        {
            Style style = excelTemplate.CreateStyle();

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

        StyleFlag BuildSheetContainerCommonConfigs()
        {
            StyleFlag styleFlag = new StyleFlag();
            styleFlag.All = true;
            styleFlag.CellShading = true;
            return styleFlag;
        }
    }

}