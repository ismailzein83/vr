using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{
    public class VRExcelTable
    {
        int _startingRowIndex;
        int _startingColumnIndex;
        List<VRExcelTableHeaderRow> _tableHeaderRows;
        List<VRExcelTableDataRow> _tableDataRows;
        List<VRExcelTableFooterRow> _tableFooterRows;
        bool _autoMergeHeader;
        bool _enableTableBorders;
        bool _autoRowVerticalMerge;

        public VRExcelTable(int startingRowIndex, int startingColumnIndex)
        {
            this._tableHeaderRows = new List<VRExcelTableHeaderRow>();
            this._tableDataRows = new List<VRExcelTableDataRow>();
            this._tableFooterRows = new List<VRExcelTableFooterRow>();
            _startingRowIndex = startingRowIndex;
            _startingColumnIndex = startingColumnIndex;
        }
        public void EnableMergeHeaders()
        {
            _autoMergeHeader = true;
        }
        public void EnableRowVerticalMerge()
        {
            _autoRowVerticalMerge = true;
        }
        public void EnableTableBorders()
        {
            _enableTableBorders = true;
        }
        public VRExcelTableHeaderRow CreateHeaderRow()
        {
            var headerRow = new VRExcelTableHeaderRow();
            _tableHeaderRows.Add(headerRow);
            return headerRow;
        }
        public VRExcelTableDataRow CreateDataRow()
        {
            var dataRow = new VRExcelTableDataRow();
            _tableDataRows.Add(dataRow);
            return dataRow;
        }
        public VRExcelTableFooterRow CreateFooterRow()
        {
            var footerRow = new VRExcelTableFooterRow();
            _tableFooterRows.Add(footerRow);
            return footerRow;
        }
        internal void GenerateTable(Worksheet worksheet)
        {
            
            int currentRowIndex = _startingRowIndex;
            if (_tableHeaderRows != null && _tableHeaderRows.Count > 0)
            {
                VRExcelTableGeneratedRowInfo parentRowInfo = null;
                foreach (var tableHeader in _tableHeaderRows)
                {
                    parentRowInfo = tableHeader.GenerateRow(worksheet, currentRowIndex, _startingColumnIndex, _autoMergeHeader, parentRowInfo, _enableTableBorders);
                    currentRowIndex++;
                }
            }
            if (_tableDataRows != null && _tableDataRows.Count > 0)
            {
                VRExcelTableGeneratedRowInfo parentRowInfo = null;
                foreach (var tableDataRow in _tableDataRows)
                {
                    tableDataRow.GenerateRow(worksheet, currentRowIndex, _startingColumnIndex, _autoMergeHeader, parentRowInfo, _enableTableBorders);
                    currentRowIndex++;
                }
                if (_autoRowVerticalMerge)
                    ApplyColumnMerge(worksheet, _startingRowIndex + _tableHeaderRows.Count, _tableDataRows);

            }
            if (_tableFooterRows != null && _tableFooterRows.Count > 0)
            {
                VRExcelTableGeneratedRowInfo parentRowInfo = null;
                foreach (var tableFooterRow in _tableFooterRows)
                {
                    tableFooterRow.GenerateRow(worksheet, currentRowIndex, _startingColumnIndex, _autoMergeHeader, parentRowInfo, _enableTableBorders);
                    currentRowIndex++;
                }
            }
        }

        protected void ApplyColumnMerge<T>(Worksheet worksheet, int startRowIndex,List<T> tableRows) where T: VRExcelTableRow
        {
            VRExcelTableGeneratedColumnInfo parentColumnInfo = null;
            var tableRowCellsCount = tableRows[0].GetTableRowCellsCount();

            if (tableRowCellsCount > 0)
            {
                for (var i = 0; i < tableRowCellsCount; i++)
                {
                    if (parentColumnInfo != null)
                    {
                        if (parentColumnInfo.MergeInfo != null && parentColumnInfo.MergeInfo.Count > 0)
                        {
                            var mergeInfo = new List<VRExcelTableCellMergeRange>();
                            foreach (var mergeItem in parentColumnInfo.MergeInfo)
                            {
                                mergeInfo.AddRange(MergeColumn(worksheet, i, startRowIndex, mergeItem.StartIndex, mergeItem.EndIndex, tableRows, tableRowCellsCount));
                            }
                            parentColumnInfo.MergeInfo = mergeInfo;
                        }

                    }
                    else
                    {
                        parentColumnInfo = new VRExcelTableGeneratedColumnInfo();
                        parentColumnInfo.MergeInfo = MergeColumn(worksheet, i, startRowIndex, 0, tableRows.Count - 1, tableRows, tableRowCellsCount);
                    }

                }
            }
        }
        private List<VRExcelTableCellMergeRange> MergeColumn<T>(Worksheet worksheet, int columnIndex, int startRowIndex, int startIndex, int endIndex, List<T> tableRows,int tableRowCellsCount) where T: VRExcelTableRow
        {
            var mergeInfo = new List<VRExcelTableCellMergeRange>();
            VRExcelTableCellMergeRange vrExcelTableCellMergeRange = null;
            VRExcelTableRowCell preTableColumnCell = null;
            for (var i = startIndex; i <= endIndex; i++)
            {

                    var tableRowCell = tableRows[i].GetTableRowCellValue(columnIndex);

                    if (preTableColumnCell == null)
                    {
                        vrExcelTableCellMergeRange = new VRExcelTableCellMergeRange
                        {
                            StartIndex = i,
                            EndIndex = i
                        };
                    }
                    else
                    {
                        if (tableRowCell.ValueEquals(preTableColumnCell.GetValue()))
                        {
                            vrExcelTableCellMergeRange.EndIndex++;
                        }
                        else
                        {
                            if (vrExcelTableCellMergeRange.StartIndex != vrExcelTableCellMergeRange.EndIndex)
                            {
                                worksheet.Cells.Merge(vrExcelTableCellMergeRange.StartIndex + startRowIndex, columnIndex, (vrExcelTableCellMergeRange.EndIndex - vrExcelTableCellMergeRange.StartIndex + 1), 1);
                                mergeInfo.Add(vrExcelTableCellMergeRange);
                            }
                            vrExcelTableCellMergeRange = new VRExcelTableCellMergeRange
                            {
                                StartIndex = i,
                                EndIndex = i
                            };
                        }
                    }
                    preTableColumnCell = tableRowCell;
            }
            if (vrExcelTableCellMergeRange != null)
            {
                if (vrExcelTableCellMergeRange.StartIndex != vrExcelTableCellMergeRange.EndIndex)
                {
                    worksheet.Cells.Merge(vrExcelTableCellMergeRange.StartIndex + startRowIndex, columnIndex, (vrExcelTableCellMergeRange.EndIndex - vrExcelTableCellMergeRange.StartIndex + 1), 1);
                    mergeInfo.Add(vrExcelTableCellMergeRange);
                }
            }
            return mergeInfo;
        }
    }
    public class VRExcelTableCellMergeRange
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
    public class VRExcelTableGeneratedRowInfo
    {
        public List<VRExcelTableCellMergeRange> MergeInfo { get; set; }
    }
    public class VRExcelTableGeneratedColumnInfo
    {
        public List<VRExcelTableCellMergeRange> MergeInfo { get; set; }
    }
    public class VRExcelTableGeneratedCellInfo
    {
    }
    public class VRExcelTableRow
    {
        public VRExcelTableRow()
        {
            this._tableRowCells = new List<VRExcelTableRowCell>();
        }
        VRExcelTableRowStyle _style;
        List<VRExcelTableRowCell> _tableRowCells;
        public VRExcelTableRowCell CreateCell()
        {
            var cell = new VRExcelTableRowCell();
            _tableRowCells.Add(cell);
            return cell;
        }
        public VRExcelTableRowStyle CreateStyle()
        {
            if (_style == null)
                _style = new VRExcelTableRowStyle();
            return _style;
        }
        internal virtual VRExcelTableGeneratedRowInfo GenerateRow(Worksheet worksheet, int rowIndex, int startingColumnIndex, bool autoMerge, VRExcelTableGeneratedRowInfo parentRowInfo, bool enableTableBorders)
        {
            VRExcelTableGeneratedRowInfo vrExcelTableGeneratedRowInfo = null;
            if (_tableRowCells != null && _tableRowCells.Count > 0)
            {
                vrExcelTableGeneratedRowInfo = new VRExcelTableGeneratedRowInfo();
                var currentColumnIndex = startingColumnIndex;
                foreach (var tableRowCell in _tableRowCells)
                {
                    tableRowCell.GenerateCell(worksheet, rowIndex, currentColumnIndex, enableTableBorders);
                    currentColumnIndex++;
                }
                if (autoMerge)
                {
                    vrExcelTableGeneratedRowInfo.MergeInfo = ApplyRowMerge(worksheet, rowIndex, startingColumnIndex, parentRowInfo);
                }
            }
            return vrExcelTableGeneratedRowInfo;
        }
        protected List<VRExcelTableCellMergeRange> ApplyRowMerge(Worksheet worksheet, int rowIndex, int startingColumnIndex, VRExcelTableGeneratedRowInfo parentRowInfo)
        {
            if (parentRowInfo != null)
            {
                if (parentRowInfo.MergeInfo != null && parentRowInfo.MergeInfo.Count > 0)
                {
                    var mergeInfo = new List<VRExcelTableCellMergeRange>();
                    foreach (var mergeItem in parentRowInfo.MergeInfo)
                    {
                        mergeInfo.AddRange(MergeRow(worksheet, rowIndex, startingColumnIndex, mergeItem.StartIndex, mergeItem.EndIndex));
                    }
                    return mergeInfo;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return MergeRow(worksheet, rowIndex, startingColumnIndex, 0, _tableRowCells.Count - 1);
            }
        }
        private List<VRExcelTableCellMergeRange> MergeRow(Worksheet worksheet, int rowIndex, int startColumnIndex, int startIndex, int endIndex)
        {
            var mergeInfo = new List<VRExcelTableCellMergeRange>();
            VRExcelTableCellMergeRange vrExcelTableCellMergeRange = null;
            VRExcelTableRowCell preTableRowCell = null;
            for (var i = startIndex; i <= endIndex; i++)
            {
                var tableRowCell = _tableRowCells[i];
                if (preTableRowCell == null)
                {
                    vrExcelTableCellMergeRange = new VRExcelTableCellMergeRange
                    {
                        StartIndex = i,
                        EndIndex = i
                    };
                }
                else
                {
                    if (tableRowCell.ValueEquals(preTableRowCell.GetValue()))
                    {
                        vrExcelTableCellMergeRange.EndIndex++;
                    }
                    else
                    {
                        if (vrExcelTableCellMergeRange.StartIndex != vrExcelTableCellMergeRange.EndIndex)
                        {
                            worksheet.Cells.Merge(rowIndex, vrExcelTableCellMergeRange.StartIndex + startColumnIndex, 1, (vrExcelTableCellMergeRange.EndIndex - vrExcelTableCellMergeRange.StartIndex +1));
                            mergeInfo.Add(vrExcelTableCellMergeRange);
                        }
                        vrExcelTableCellMergeRange = new VRExcelTableCellMergeRange
                        {
                            StartIndex = i,
                            EndIndex = i
                        };
                    }
                }
                preTableRowCell = tableRowCell;
            }
            if (vrExcelTableCellMergeRange != null)
            {
                if (vrExcelTableCellMergeRange.StartIndex != vrExcelTableCellMergeRange.EndIndex)
                {
                    worksheet.Cells.Merge(rowIndex, vrExcelTableCellMergeRange.StartIndex + startColumnIndex, 1, (vrExcelTableCellMergeRange.EndIndex - vrExcelTableCellMergeRange.StartIndex + 1));
                    mergeInfo.Add(vrExcelTableCellMergeRange);
                }
            }
            return mergeInfo;
        }
        internal int GetTableRowCellsCount()
        {
            if (_tableRowCells!=null)
                return this._tableRowCells.Count;
            return 0;
        }
        internal VRExcelTableRowCell GetTableRowCellValue(int columnIndex)
        {
            if (this._tableRowCells != null)
                return _tableRowCells[columnIndex];
            return null;
        }

    }


    public class VRExcelTableHeaderRow : VRExcelTableRow
    {
    }
    public class VRExcelTableDataRow : VRExcelTableRow
    {

    }
    public class VRExcelTableFooterRow : VRExcelTableRow
    {

    }
    public class VRExcelTableRowCell
    {
        VRExcelTableRowCellStyle _style;
        Object _cellValue;
        public VRExcelTableRowCell()
        {
        }
        internal Object GetValue()
        {
            return _cellValue;
        }
        internal bool ValueEquals(Object value)
        {
            if (_cellValue == null && value == null)
                return true;
            if (_cellValue == null)
                return false;
            if (value == null)
                return false;
            return _cellValue.Equals(value);
        }
        public void SetValue(object value)
        {
            _cellValue = value;
        }
        public VRExcelTableRowCellStyle CreateStyle()
        {
            if (_style == null)
                _style = new VRExcelTableRowCellStyle();
            return _style;
        }

        internal VRExcelTableGeneratedCellInfo GenerateCell(Worksheet worksheet, int rowIndex, int columnIndex, bool enableTableBorders)
        {

            Cell cell = worksheet.Cells[rowIndex, columnIndex];
            if (_style != null)
            {
                var cellStyle = cell.GetStyle();
                if(enableTableBorders)
                {
                    cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                    cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                    cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                }

                BuildVRExcelTableRowCellStyle(_style, cellStyle);
                cell.SetStyle(cellStyle, BuildSheetContainerCommonConfigs());
              
            }
            cell.PutValue(_cellValue);
            return null;
        }
        void BuildVRExcelTableRowCellStyle(VRExcelTableRowCellStyle styleSetting, Style style)
        {
            if (styleSetting.SetBorder == true)
            {
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            }

            if (styleSetting.HorizontalAlignment.HasValue)
            {
                switch (styleSetting.HorizontalAlignment.Value)
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

            if (styleSetting.VerticalAlignment.HasValue)
            {
                switch (styleSetting.VerticalAlignment.Value)
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
            if (!string.IsNullOrEmpty(styleSetting.BGColor))
                style.ForegroundColor = ColorTranslator.FromHtml(styleSetting.BGColor);
            style.Pattern = BackgroundType.Solid;
            if (!string.IsNullOrEmpty(styleSetting.FontColor))
                style.Font.Color = ColorTranslator.FromHtml(styleSetting.FontColor);
            if (styleSetting.FontSize.HasValue)
                style.Font.Size = styleSetting.FontSize.Value;
            style.Font.IsBold = styleSetting.IsBold;
            style.Font.IsItalic = styleSetting.IsItalic;
            style.Pattern = BackgroundType.Solid;
        }
        StyleFlag BuildSheetContainerCommonConfigs()
        {
            StyleFlag styleFlag = new StyleFlag();
            styleFlag.All = false;
            styleFlag.Font = true;
            styleFlag.Borders = true;
            styleFlag.HorizontalAlignment = true;
            styleFlag.VerticalAlignment = _style.VerticalAlignment.HasValue;
            return styleFlag;
        }

    }
    public class VRExcelTableRowCellStyle : VRExcelContainerConfig
    {
    }
    public class VRExcelTableRowStyle : VRExcelContainerConfig
    {
    }
}
