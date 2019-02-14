using Aspose.Cells;
using Aspose.Cells.Pivot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Excel
{

    public class BaseVRExcelPivotTableFieldArea
    {
        public int? FieldIndex { get; set; }
        public string FieldName { get; set; }
    }
    public class VRExcelPivotTableRowArea: BaseVRExcelPivotTableFieldArea
    {

    }
    public class VRExcelPivotTableColumnArea: BaseVRExcelPivotTableFieldArea
    {

    }
    public class VRExcelPivotTablePageArea: BaseVRExcelPivotTableFieldArea
    {

    }
    public class VRExcelPivotTableDataArea: BaseVRExcelPivotTableFieldArea
    {

    }
    public class VRExcelPivotTable
    {
        string _tableName;
        List<VRExcelPivotTableRowArea> _rowsArea;
        List<VRExcelPivotTableColumnArea> _columnsArea;
        List<VRExcelPivotTablePageArea> _pagesArea;
        List<VRExcelPivotTableDataArea> _dataArea;
        bool _isAscendSort;
        bool _isAutoSort;
        int _startColumnindex, _startRowIndex;
        int _sourceSheetIndex, _startSourceColumnIndex, _endSourceColumnIndex, _startSourceRowIndex, _endSourceRowIndex;
        public VRExcelPivotTable(string tableName, int rowIndex, int columnindex)
        {
            _startRowIndex = rowIndex;
            _startColumnindex = columnindex;
            _tableName = tableName;
        }
        void InitializeRowsArea()
        {
            if (_rowsArea == null)
                _rowsArea = new List<VRExcelPivotTableRowArea>();
        }
        void InitializeColumnsArea()
        {
            if (_columnsArea == null)
                _columnsArea = new List<VRExcelPivotTableColumnArea>();
        }
        void InitializePagesArea()
        {
            if (_pagesArea == null)
                _pagesArea = new List<VRExcelPivotTablePageArea>();
        }
        void InitializeDataArea()
        {
            if (_dataArea == null)
                _dataArea = new List<VRExcelPivotTableDataArea>();
        }
        public void SetSourceData(int sheetIndex, int startColumnIndex, int endColumnIndex, int startRowIndex, int endRowIndex)
        {
            _sourceSheetIndex = sheetIndex;
            _startSourceColumnIndex = startColumnIndex;
            _endSourceColumnIndex = endColumnIndex;
            _startSourceRowIndex = startRowIndex;
            _endSourceRowIndex = endRowIndex;
        }
      
        public void AddRowToArea(int fieldIndex)
        {
            InitializeRowsArea();
            _rowsArea.Add(new VRExcelPivotTableRowArea
            {
                FieldIndex = fieldIndex
            });
        }
        public void AddColumnToArea(int fieldIndex)
        {
            InitializeColumnsArea();
            _columnsArea.Add(new VRExcelPivotTableColumnArea
            {
                FieldIndex = fieldIndex
            });
        }
        public void AddPageToArea(int fieldIndex)
        {
            InitializePagesArea();
            _pagesArea.Add(new VRExcelPivotTablePageArea
            {
                FieldIndex = fieldIndex
            });
        }
        public void AddDataToArea(int fieldIndex)
        {
            InitializeDataArea();
            _dataArea.Add(new VRExcelPivotTableDataArea
            {
                FieldIndex = fieldIndex
            });
        }
        public void IsAscendSort()
        {
            _isAscendSort = true;
        }
        public void IsAutoSort()
        {
            _isAutoSort = true;
        }
        internal void GeneratePivotTable(IVRExcelFileGenerateContext context, Worksheet worksheet)
        {
            string columnName = string.Format("{0}{1}", Utilities.GetExcelColumnName(_startColumnindex), _startRowIndex);
            var sheetName = context.GetSheetName(_sourceSheetIndex);
            string sourceData = string.Format("{0}!${1}${2}:${3}${4}", sheetName, Utilities.GetExcelColumnName(_startSourceColumnIndex), _startSourceRowIndex, Utilities.GetExcelColumnName(_endSourceColumnIndex) , _endSourceRowIndex);
            int pt2Idx = worksheet.PivotTables.Add(sourceData, columnName, _tableName);
            PivotTable pivotTable = worksheet.PivotTables[pt2Idx];
            if(_rowsArea != null)
            {
                foreach (var rowArea in _rowsArea)
                {
                    if (rowArea.FieldIndex.HasValue)

                        pivotTable.AddFieldToArea(PivotFieldType.Row, rowArea.FieldIndex.Value);
                    else
                        pivotTable.AddFieldToArea(PivotFieldType.Row, rowArea.FieldName);

                }
            }
            if (_columnsArea != null)
            {
                foreach (var columnArea in _columnsArea)
                {
                    if (columnArea.FieldIndex.HasValue)

                        pivotTable.AddFieldToArea(PivotFieldType.Column, columnArea.FieldIndex.Value);
                    else
                        pivotTable.AddFieldToArea(PivotFieldType.Column, columnArea.FieldName);

                }
            }
            if (_pagesArea != null)
            {
                foreach (var pageArea in _pagesArea)
                {
                    if (pageArea.FieldIndex.HasValue)

                        pivotTable.AddFieldToArea(PivotFieldType.Page, pageArea.FieldIndex.Value);
                    else
                        pivotTable.AddFieldToArea(PivotFieldType.Page, pageArea.FieldName);

                }
            }
            if (_dataArea != null)
            {
                foreach (var dataArea in _dataArea)
                {
                    if (dataArea.FieldIndex.HasValue)

                        pivotTable.AddFieldToArea(PivotFieldType.Data, dataArea.FieldIndex.Value);
                    else
                        pivotTable.AddFieldToArea(PivotFieldType.Data, dataArea.FieldName);
                }
            }
            //pivotTable.RowGrand = true;
            //pivotTable.ColumnGrand = true;
            //pivotTable.IsAutoFormat = true;
            //pivotTable.AutoFormatType = Aspose.Cells.Pivot.PivotTableAutoFormatType.Report6;

            pivotTable.PivotTableStyleName = "PivotStyleLight16";
           //  pivotTable.ColumnFields.Add(pivotTable.DataField);
            //pivotTable.DataFields[0].Number = 7;
            //pivotTable.RowFields[0].IsAscendSort = _isAscendSort;
            //pivotTable.RowFields[0].IsAutoSort = _isAutoSort;
            pivotTable.CalculateData();
            pivotTable.RefreshData();
            pivotTable.RefreshDataOnOpeningFile = true;
        }
    }
}
