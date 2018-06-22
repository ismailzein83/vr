using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.MainExtensions.AutomatedReport.Queries;
using Vanrise.Common;
using System.Drawing;
using System.IO;
using Vanrise.GenericData.Business;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9FAAE9B2-931E-4B3F-BDA4-B0F3B7647488"); }
        }

        public long FileTemplateId { get; set; }

        public List<AdvancedExcelFileGeneratorTableDefinition> TableDefinitions { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixDefinition> MatrixDefinitions { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            byte[] fileContent = null;

            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(this.FileTemplateId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            Workbook TableDefinitionsWorkbook = new Workbook(fileStream);
            Common.Utilities.ActivateAspose();

            if (this.TableDefinitions != null && this.TableDefinitions.Count != 0)
            {
                foreach (var tableDef in this.TableDefinitions)
                {
                    var columns = tableDef.ColumnDefinitions;
                    columns.ThrowIfNull("columns");
                    Worksheet TableDefinitionsWorksheet = TableDefinitionsWorkbook.Worksheets[tableDef.SheetIndex];
                    var dataList = context.HandlerContext.GetDataList(tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    string dataListIdentifier = string.Format("{0}_{1}", tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    dataList.ThrowIfNull("dataList", dataListIdentifier);
                    dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);
                    dataList.FieldInfos.ThrowIfNull("dataList.FieldInfos", dataListIdentifier);
                    var fieldInfos = dataList.FieldInfos;
                    int fieldInfosCount = fieldInfos.Count;
                    int titleRowIndex = tableDef.RowIndex;
                    int headerRowIndex = tableDef.RowIndex;
                    int dataRowIndex;

                    if (tableDef.IncludeTitle && tableDef.IncludeHeaders)
                    {
                        headerRowIndex = titleRowIndex + 1;
                        dataRowIndex = headerRowIndex + 1;
                    }
                    else if (tableDef.IncludeTitle && !tableDef.IncludeHeaders)
                    {
                        dataRowIndex = titleRowIndex + 1;
                    }
                    else if (!tableDef.IncludeTitle && tableDef.IncludeHeaders)
                    {
                        dataRowIndex = headerRowIndex + 1;
                    }
                    else
                    {
                        dataRowIndex = tableDef.RowIndex;
                    }

                    Dictionary<int, List<int>> colIndexByRow = new Dictionary<int, List<int>>();

                    bool setHeaders = true;
                    if (tableDef.Title != null && tableDef.IncludeTitle)
                    {
                        var title = tableDef.Title;
                        List<int> columnIndices = new List<int>();
                        foreach (var col in columns)
                        {
                            columnIndices.Add(col.ColumnIndex);
                        }
                        var orderedIndices = columnIndices.OrderBy(x => x);
                        int difference = 1;
                        if (orderedIndices.Count() > 1)
                        {       
                            difference = orderedIndices.Last() - orderedIndices.First()+1;
                        }
                        var titleColumnIndex = orderedIndices.First();
                        TableDefinitionsWorksheet.Cells.Merge(titleRowIndex, titleColumnIndex, 1, difference);
                        TableDefinitionsWorksheet.Cells[titleRowIndex, titleColumnIndex].PutValue(title);
                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(titleRowIndex, titleColumnIndex);
                        Style style = cell.GetStyle();
                        style.HorizontalAlignment = TextAlignmentType.Center;
                        style.Font.Name = "Times New Roman";
                        style.Font.Color = Color.Black;
                        style.Font.Size = 16;
                        style.Font.IsBold = true;
                        cell.SetStyle(style);
                    }

                    if (dataList.Items.Count != 0)
                    {
                        foreach (var item in dataList.Items)
                        {
                            foreach(var column in columns)
                            {
                                var field = item.Fields.FindRecord(x => x.Key == column.FieldName);
                                if (field.Value != null)
                                {
                                    var fieldInfo = fieldInfos.GetRecord(field.Key);
                                    if (tableDef.IncludeHeaders && fieldInfo != null && setHeaders)
                                    {
                                        TableDefinitionsWorksheet.Cells[headerRowIndex, column.ColumnIndex].PutValue(fieldInfo.FieldTitle);
                                        TableDefinitionsWorksheet.Cells.SetColumnWidth(column.ColumnIndex, 20);
                                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(headerRowIndex, column.ColumnIndex);
                                        Style style = cell.GetStyle();
                                        style.Font.Name = "Times New Roman";
                                        style.Font.Color = Color.Black;
                                        style.Font.Size = 14;
                                        style.Font.IsBold = true;
                                        cell.SetStyle(style);
                                        var headerIndices = colIndexByRow.GetOrCreateItem(headerRowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                        headerIndices.Add(column.ColumnIndex);
                                    }
                                    var fieldType = fieldInfo.FieldType;
                                    bool renderDescription = fieldType.RenderDescriptionByDefault();
                                    if (renderDescription)
                                    {
                                        TableDefinitionsWorksheet.Cells[dataRowIndex, column.ColumnIndex].PutValue(field.Value.Description);
                                        TableDefinitionsWorksheet.Cells.SetColumnWidth(column.ColumnIndex, 20);
                                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(dataRowIndex, column.ColumnIndex);
                                        Style style = cell.GetStyle();
                                        style.HorizontalAlignment = TextAlignmentType.Right;
                                        style.Font.Name = "Times New Roman";
                                        style.Font.Color = Color.Black;
                                        style.Font.Size = 12;
                                        style.Font.IsBold = false;
                                        cell.SetStyle(style);
                                        var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                        dataIndices.Add(column.ColumnIndex);

                                    }
                                    else
                                    {
                                        if (field.Value.Value is DateTime)
                                        {
                                            var date = Convert.ToDateTime(field.Value.Value);
                                            GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                                            string format = generalSettingsManager.GetDateTimeFormat();
                                            TableDefinitionsWorksheet.Cells[dataRowIndex, column.ColumnIndex].PutValue(date.ToString(format));
                                            TableDefinitionsWorksheet.Cells.SetColumnWidth(column.ColumnIndex, 20);
                                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(dataRowIndex, column.ColumnIndex);
                                            Style style = cell.GetStyle();
                                            style.HorizontalAlignment = TextAlignmentType.Right;
                                            style.Font.Name = "Times New Roman";
                                            style.Font.Color = Color.Black;
                                            style.Font.Size = 12;
                                            style.Font.IsBold = false;
                                            cell.SetStyle(style);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                            dataIndices.Add(column.ColumnIndex);

                                        }
                                        else if (field.Value.Value is int || field.Value.Value is double || field.Value.Value is decimal || field.Value.Value is long)
                                        {
                                            TableDefinitionsWorksheet.Cells[dataRowIndex, column.ColumnIndex].PutValue(field.Value.Value);
                                            TableDefinitionsWorksheet.Cells.SetColumnWidth(column.ColumnIndex, 20);
                                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(dataRowIndex, column.ColumnIndex);
                                            Style style = cell.GetStyle();
                                            style.HorizontalAlignment = TextAlignmentType.Left;
                                            style.Font.Name = "Times New Roman";
                                            style.Font.Color = Color.Black;
                                            style.Font.Size = 12;
                                            style.Font.IsBold = false;
                                            cell.SetStyle(style);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                            dataIndices.Add(column.ColumnIndex);
                                        }
                                        else
                                        {
                                            TableDefinitionsWorksheet.Cells[dataRowIndex, column.ColumnIndex].PutValue(field.Value.Value);
                                            TableDefinitionsWorksheet.Cells.SetColumnWidth(column.ColumnIndex, 20);
                                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(dataRowIndex, column.ColumnIndex);
                                            Style style = cell.GetStyle();
                                            style.HorizontalAlignment = TextAlignmentType.Right;
                                            style.Font.Name = "Times New Roman";
                                            style.Font.Color = Color.Black;
                                            style.Font.Size = 12;
                                            style.Font.IsBold = false;
                                            cell.SetStyle(style);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                            dataIndices.Add(column.ColumnIndex);
                                        }
                                    }
                                }
                            }
                            setHeaders = false;
                            dataRowIndex++;
                        }
                    }
                    else
                    {
                        foreach (var col in columns)
                        {
                            if (tableDef.IncludeHeaders && setHeaders)
                            {
                                TableDefinitionsWorksheet.Cells[headerRowIndex, col.ColumnIndex].PutValue(col.FieldTitle);
                                TableDefinitionsWorksheet.Cells.SetColumnWidth(col.ColumnIndex, 20);
                                Cell cell = TableDefinitionsWorksheet.Cells.GetCell(headerRowIndex, col.ColumnIndex);
                                Style style = cell.GetStyle();
                                style.Font.Name = "Times New Roman";
                                style.Font.Color = Color.Black;
                                style.Font.Size = 14;
                                style.Font.IsBold = true;
                                cell.SetStyle(style);
                                var headerIndices = colIndexByRow.GetOrCreateItem(headerRowIndex,
                                             () =>
                                             {
                                                 return new List<int>();
                                             });
                                headerIndices.Add(col.ColumnIndex);
                            }
                        }
                        setHeaders = false;
                    }
                    SetBorders(colIndexByRow, ref TableDefinitionsWorksheet);   
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = TableDefinitionsWorkbook.SaveToStream();
            fileContent = memoryStream.ToArray();
            return new VRAutomatedReportGeneratedFile()
            {
                FileContent = fileContent
            };
        }

        public override void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            if (this.TableDefinitions == null || this.TableDefinitions.Count == 0)
            {
                throw new Exception("No table definitions were added.");
            }
            var tableDefinitions = this.TableDefinitions;
            if (context.Queries != null && context.Queries.Count != 0)
            {
                var queries = context.Queries;
                context.Result = true;
                foreach (var tableDefinition in tableDefinitions)
                {
                    var matchingQuery = queries.FindRecord(x => x.VRAutomatedReportQueryId == tableDefinition.VRAutomatedReportQueryId);
                    if (matchingQuery == null)
                    {
                        context.Result = false;
                        context.ErrorMessage = string.Format("A query used in the handler has been deleted.");
                        break;
                    }
                    matchingQuery.Settings.ThrowIfNull("matchingQuery.Settings");
                    var querySettings = matchingQuery.Settings.CastWithValidate<RecordSearchQuerySettings>("matchingQuery.Settings");
                    querySettings.Columns.ThrowIfNull("querySettings.Columns");
                    var queryColumns = querySettings.Columns;
                    if (queryColumns.Count == 0)
                        throw new Exception(string.Format("No query columns were added for query {0}.", matchingQuery.QueryTitle));
                    tableDefinition.ColumnDefinitions.ThrowIfNull("tableDefinition.ColumnDefinitions");
                    var tableDefinitionColumns = tableDefinition.ColumnDefinitions;
                    if (tableDefinition.ColumnDefinitions.Count == 0)
                        throw new Exception(string.Format("No handler columns were added."));
                    List<string> missingTableFields = new List<string>();
                    foreach (var tableDefinitionColumn in tableDefinitionColumns)
                    {
                        if (!queryColumns.Any(x => x.ColumnName == tableDefinitionColumn.FieldName))
                        {
                            if (!missingTableFields.Contains(tableDefinitionColumn.FieldTitle))
                                missingTableFields.Add(tableDefinitionColumn.FieldTitle);
                        }
                       
                    }
                    if (missingTableFields != null && missingTableFields.Count == 1)
                    {
                        context.Result = false;
                        context.ErrorMessage = string.Format("The field '{0}' was not found in query '{1}' after it has been edited.", missingTableFields.First(), matchingQuery.QueryTitle);
                        break;
                    }
                    else if (missingTableFields != null && missingTableFields.Count > 1)
                    {
                        string joinedFields;
                        if (missingTableFields.Count == 2)
                        {
                            joinedFields = string.Join(" and ", missingTableFields);
                        }
                        else
                        {
                            joinedFields = string.Join(" , ", missingTableFields);
                        }
                        context.Result = false;
                        context.ErrorMessage = string.Format("The fields '{0}' were not found in query '{1}' after it has been edited.", joinedFields, matchingQuery.QueryTitle);
                        break;
                    }
                }
            }
        }
        private void SetBorders(Dictionary<int, List<int>> colIndexByRow, ref Worksheet TableDefinitionsWorksheet)
        {
            if (colIndexByRow != null && colIndexByRow.Count != 0)
            {
                if (colIndexByRow.Count > 1)
                {
                    foreach (var row in colIndexByRow)
                    {
                        if (row.Value != null && row.Value.Count != 0)
                        {
                            if (row.Key == colIndexByRow.Keys.OrderBy(x => x).First())
                            {
                                var orderedColumnIndices = row.Value.OrderBy(x => x);
                                if (orderedColumnIndices.Count() == 1)
                                {
                                    var columnIndex = orderedColumnIndices.First();
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                    Style style = cell.GetStyle();
                                    style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                                    style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                    style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                                    cell.SetStyle(style);
                                }
                                else
                                {
                                    foreach (var columnIndex in orderedColumnIndices)
                                    {
                                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                        Style style = cell.GetStyle();
                                        if (columnIndex == orderedColumnIndices.First())
                                        {
                                            style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                                        }
                                        else if (columnIndex == orderedColumnIndices.Last())
                                        {
                                            style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                        }
                                        style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                                        cell.SetStyle(style);
                                    }
                                }

                            }
                            else if (row.Key == colIndexByRow.Keys.OrderBy(x => x).Last())
                            {
                                var orderedColumnIndices = row.Value.OrderBy(x => x);
                                if (orderedColumnIndices.Count() == 1)
                                {
                                    var columnIndex = orderedColumnIndices.First();
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                    Style style = cell.GetStyle();
                                    style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                                    style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                    style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                                    cell.SetStyle(style);
                                }
                                foreach (var columnIndex in orderedColumnIndices)
                                {
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                    Style style = cell.GetStyle();
                                    if (columnIndex == orderedColumnIndices.First())
                                    {
                                        style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                                    }
                                    else if (columnIndex == orderedColumnIndices.Last())
                                    {
                                        style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                    }
                                    style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                                    cell.SetStyle(style);

                                }
                            }
                            else
                            {
                                var orderedColumnIndices = row.Value.OrderBy(x => x);
                                if (orderedColumnIndices.Count() == 1)
                                {
                                    var columnIndex = orderedColumnIndices.First();
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                    Style style = cell.GetStyle();
                                    style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                                    style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                    cell.SetStyle(style);
                                }
                                foreach (var columnIndex in orderedColumnIndices)
                                {
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, columnIndex);
                                    Style style = cell.GetStyle();
                                    if (columnIndex == orderedColumnIndices.First())
                                    {
                                        style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);

                                    }
                                    else if (columnIndex == orderedColumnIndices.Last())
                                    {
                                        style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                    }
                                    cell.SetStyle(style);
                                }
                            }
                        }
                    }
                }
                else if (colIndexByRow.Count == 1)
                {
                    var onlyRow = colIndexByRow.First();
                    if (onlyRow.Value != null && onlyRow.Value.Count != 0)
                    {
                        var orderedColumnIndices = onlyRow.Value.OrderBy(x => x);
                        if (orderedColumnIndices.Count() == 1)
                        {
                            var columnIndex = orderedColumnIndices.First();
                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(onlyRow.Key, columnIndex);
                            Style style = cell.GetStyle();
                            style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                            style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                            style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                            style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                            cell.SetStyle(style);
                        }
                        else
                        {
                            foreach (var columnIndex in orderedColumnIndices)
                            {
                                Cell cell = TableDefinitionsWorksheet.Cells.GetCell(onlyRow.Key, columnIndex);
                                Style style = cell.GetStyle();
                                if (columnIndex == orderedColumnIndices.First())
                                {
                                    style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);

                                }
                                else if (columnIndex == orderedColumnIndices.Last())
                                {
                                    style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                                }
                                style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                                cell.SetStyle(style);
                            }
                        }
                    }
                }
            }
        }
    }

    public class AdvancedExcelFileGeneratorTableDefinition
    {
        public Guid VRAutomatedReportQueryId { get; set; }

        public string ListName { get; set; }

        public string SheetName { get; set; }

        public int SheetIndex { get; set; }

        public int RowIndex { get; set; }

        public bool IncludeTitle { get; set; }

        public bool IncludeHeaders { get; set; }

        public string Title { get; set; }

        public List<AdvancedExcelFileGeneratorTableColumnDefinition> ColumnDefinitions { get; set; }
    }

    public class AdvancedExcelFileGeneratorTableColumnDefinition
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public int ColumnIndex { get; set; }

        public bool UseFieldDescription { get; set; }
    }

    public class AdvancedExcelFileGeneratorMatrixDefinition
    {
        public Guid VRAutomatedReportQueryId { get; set; }

        public string ListName { get; set; }

        public string SheetIndex { get; set; }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixRowField> RowFields { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixColumnField> ColumnFields { get; set; }

        public AdvancedExcelFileGeneratorMatrixDataField DataField { get; set; }
    }

    public class AdvancedExcelFileGeneratorMatrixRowField
    {
        public string FieldName { get; set; }
    }

    public class AdvancedExcelFileGeneratorMatrixColumnField
    {
        public string FieldName { get; set; }
    }

    public class AdvancedExcelFileGeneratorMatrixDataField
    {
        public string FieldName { get; set; }
    }

}
