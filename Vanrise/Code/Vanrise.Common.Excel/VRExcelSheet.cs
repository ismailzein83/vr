using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public enum VRExcelSheetType
    {
        Worksheet = 1,
        Chart = 2,
    }
    public class VRExcelSheet
    {
        public VRExcelSheet()
        {
            this.Cells = new List<VRExcelCell>();
            this.RowConfigs = new Dictionary<int, VRExcelRowConfig>();
            this.ColumnConfigs = new Dictionary<int, VRExcelColumnConfig>();
            this.Images = new List<VRExcelImageConfig>();
            this.Tables = new List<VRExcelTable>();

            this.PivotTables = new List<VRExcelPivotTable>();
            this.Charts = new List<VRExcelChart>();
        }
         
        public string SheetName { get; set; }
        public VRExcelSheetType SheetType { get; set; }
        public bool HideSheet { get; set; }
        internal List<VRExcelCell> Cells { get; set; }
        internal Dictionary<int, VRExcelRowConfig> RowConfigs { get; set; }
        internal Dictionary<int, VRExcelColumnConfig> ColumnConfigs { get; set; }
        internal List<VRExcelImageConfig> Images { get; set; }
        internal List<VRExcelTable> Tables { get; set; }
        internal List<VRExcelPivotTable> PivotTables { get; set; }
        internal List<VRExcelChart> Charts { get; set; }
        public void SetRowConfig(VRExcelRowConfig rowConfig)
        {
            VRExcelRowConfig config;
            if (RowConfigs.TryGetValue(rowConfig.RowIndex, out config))
                throw new Exception(string.Format("Configuration for the row {0} already exist. ", rowConfig.RowIndex));
            this.RowConfigs.Add(rowConfig.RowIndex, rowConfig);
        }
        public void SetColumnConfig(VRExcelColumnConfig columnConfig)
        {
            VRExcelColumnConfig config;
            if (ColumnConfigs.TryGetValue(columnConfig.ColumnIndex, out config))
                throw new Exception(string.Format("Configuration for the column {0} already exist. ", columnConfig.ColumnIndex));
            this.ColumnConfigs.Add(columnConfig.ColumnIndex, columnConfig);
        }
        public void AddCell(VRExcelCell cell)
        {

            Cells.Add(cell);
        }
        public VRExcelCell CreateCell()
        {
            var cell = new VRExcelCell();
            Cells.Add(cell);
            return cell;
        }
        
        public VRExcelImageConfig CreateImage()
        {
            var image = new VRExcelImageConfig();
            Images.Add(image);
            return image;
        }
        public void AddImage(VRExcelImageConfig image)
        {
            Images.Add(image);
        }
        public VRExcelTable CreateTable(int startingRowIndex, int startingColumnIndex)
        {
            var table = new VRExcelTable(startingRowIndex, startingColumnIndex);
            Tables.Add(table);
            return table;
        }
        public VRExcelChart CreateChart(VRExcelChartType chartType, int startingRow, int endingRow, int startingColumn, int endingColumn)
        {
            var chart = new VRExcelChart(chartType, startingRow, endingRow, startingColumn, endingColumn);
            Charts.Add(chart);
            return chart;
        }
        public VRExcelPivotTable CreatePivotTable(string tableName, int rowIndex, int columnindex)
        {
            var pivotTable = new VRExcelPivotTable(tableName,rowIndex, columnindex);
            PivotTables.Add(pivotTable);
            return pivotTable;
        }
    }
    public class VRExcelImageConfig
    {
        public int StartingColumnIndex { get; set; }
        public int StartingRowIndex { get; set; }
        public int NumberOfColumns { get; set; }
        public int NumberOFRows { get; set; }
        public byte[] Value { get; set; }
    }
    public enum VRExcelContainerHorizontalAlignment { Left = 0, Center = 1, Right = 2 }
    public class VRExcelContainerConfig
    {
        public string BGColor { get; set; }

        public string FontColor { get; set; }

        public int? FontSize { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public bool SetBorder { get; set; }

        public VRExcelContainerHorizontalAlignment? HorizontalAlignment { get; set; }
    }
    public class VRExcelRowConfig : VRExcelContainerConfig
    {
        public int RowIndex { get; set; }

        public double? RowHeight { get; set; }

    }
    public class VRExcelColumnConfig : VRExcelContainerConfig
    {
        public int ColumnIndex { get; set; }

        public double? ColumnWidth { get; set; }

    }
}
