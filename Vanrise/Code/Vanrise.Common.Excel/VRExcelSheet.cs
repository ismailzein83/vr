using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public class VRExcelSheet
    {
        public VRExcelSheet()
        {
            this.Cells = new List<VRExcelCell>();
            this.RowConfigs = new Dictionary<int, VRExcelRowConfig>();
            this.ColumnConfigs = new Dictionary<int, VRExcelColumnConfig>();
        }

        public string SheetName { get; set; }
        internal List<VRExcelCell> Cells { get; set; }
        internal Dictionary<int, VRExcelRowConfig> RowConfigs { get; set; }
        internal Dictionary<int, VRExcelColumnConfig> ColumnConfigs { get; set; }

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

    }
    public class VRExcelContainerConfig
    {
        public string BGColor { get; set; }

        public string FontColor { get; set; }

        public int? FontSize { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public bool SetBorder { get; set; }
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
