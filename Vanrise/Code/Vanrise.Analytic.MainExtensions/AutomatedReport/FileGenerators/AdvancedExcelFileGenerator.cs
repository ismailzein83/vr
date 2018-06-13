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

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9FAAE9B2-931E-4B3F-BDA4-B0F3B7647488"); }
        }

        public long? FileTemplateId { get; set; }

        public List<AdvancedExcelFileGeneratorTableDefinition> TableDefinitions { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixDefinition> MatrixDefinitions { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            byte[] fileContent = null;

            if (this.TableDefinitions != null && this.TableDefinitions.Count!=0)
            {
                Workbook TableDefinitionsWorkbook = new Workbook();
                Vanrise.Common.Utilities.ActivateAspose();
                TableDefinitionsWorkbook.Worksheets.Clear();

                foreach (var tableDef in this.TableDefinitions)
                {
                    var columns = tableDef.ColumnDefinitions;
                    columns.ThrowIfNull("columns");
                    Worksheet TableDefinitionsWorksheet = TableDefinitionsWorkbook.Worksheets.Add(tableDef.SheetName);
                    var dataList = context.HandlerContext.GetDataList(tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    string dataListIdentifier = string.Format("{0}_{1}", tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    dataList.ThrowIfNull("dataList", dataListIdentifier);
                    dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);
                    dataList.FieldInfos.ThrowIfNull("dataList.FieldInfos", dataListIdentifier);
                    var fieldInfos = dataList.FieldInfos;
                    int fieldInfosCount = fieldInfos.Count;

                    int titleIndex = tableDef.RowIndex;
                    int headerIndex = tableDef.RowIndex;
                    int rowIndex;

                    if (tableDef.IncludeTitle && tableDef.IncludeHeaders)
                    {
                        headerIndex = titleIndex + 1;
                        rowIndex = headerIndex + 1;
                    }
                    else if (tableDef.IncludeTitle && !tableDef.IncludeHeaders)
                    {
                        rowIndex = titleIndex + 1;
                    }
                    else if (!tableDef.IncludeTitle && tableDef.IncludeHeaders)
                    {
                        rowIndex = headerIndex + 1;
                    }
                    else
                    {
                        rowIndex = tableDef.RowIndex;
                    }
                  
                    Dictionary<int, List<int>> colIndexByRow = new Dictionary<int, List<int>>();
             
                    bool setHeaders = true;
                    if (tableDef.TitleDefinition != null && tableDef.IncludeTitle)
                    {
                        var titleDefinition = tableDef.TitleDefinition;
                        TableDefinitionsWorksheet.Cells[titleIndex, titleDefinition.ColumnIndex].PutValue(titleDefinition.Title);
                        TableDefinitionsWorksheet.Cells.SetColumnWidth(titleDefinition.ColumnIndex, 20);
                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(titleIndex, titleDefinition.ColumnIndex);
                        Style style = cell.GetStyle();
                        style.Font.Name = "Times New Roman";
                        style.Font.Color = Color.Black;
                        style.Font.Size = 16;
                        style.Font.IsBold = true;
                        cell.SetStyle(style);
                    }
                    
                    foreach (var item in dataList.Items)
                    {
                        foreach (var field in item.Fields)
                        {
                            var matchingColumn = columns.FindRecord(x => x.FieldName == field.Key);
                            if (matchingColumn != null)
                            {
                                if (field.Value != null)
                                {
                                    var fieldInfo = fieldInfos.GetRecord(field.Key);
                                    if (tableDef.IncludeHeaders && fieldInfo!=null && setHeaders)
                                    {
                                        TableDefinitionsWorksheet.Cells[headerIndex, matchingColumn.ColumnIndex].PutValue(fieldInfo.FieldTitle);
                                        TableDefinitionsWorksheet.Cells.SetColumnWidth(matchingColumn.ColumnIndex, 20);
                                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(headerIndex, matchingColumn.ColumnIndex);
                                        Style style = cell.GetStyle();
                                        style.Font.Name = "Times New Roman";
                                        style.Font.Color = Color.Black;
                                        style.Font.Size = 14;
                                        style.Font.IsBold = true;
                                        cell.SetStyle(style);
                                        var headerIndices = colIndexByRow.GetOrCreateItem(headerIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                        headerIndices.Add(matchingColumn.ColumnIndex);
                                    }
                                    var fieldType = fieldInfo.FieldType;
                                    bool renderDescription = fieldType.RenderDescriptionByDefault();
                                    if(renderDescription)
                                    {
                                        TableDefinitionsWorksheet.Cells[rowIndex, matchingColumn.ColumnIndex].PutValue(field.Value.Description);
                                        TableDefinitionsWorksheet.Cells.SetColumnWidth(matchingColumn.ColumnIndex, 20);
                                        Cell cell = TableDefinitionsWorksheet.Cells.GetCell(rowIndex, matchingColumn.ColumnIndex);
                                        Style style = cell.GetStyle();
                                        style.Font.Name = "Times New Roman";
                                        style.Font.Color = Color.Black;
                                        style.Font.Size = 12;
                                        style.Font.IsBold = false;
                                        cell.SetStyle(style);
                                        var dataIndices = colIndexByRow.GetOrCreateItem(rowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                        dataIndices.Add(matchingColumn.ColumnIndex);

                                    }
                                    else
                                    {
                                        if (field.Value.Value  is DateTime)
                                        {
                                            var date = Convert.ToDateTime(field.Value.Value);
                                            GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                                            string format = generalSettingsManager.GetDateTimeFormat();
                                            TableDefinitionsWorksheet.Cells[rowIndex, matchingColumn.ColumnIndex].PutValue(date.ToString(format));
                                            TableDefinitionsWorksheet.Cells.SetColumnWidth(matchingColumn.ColumnIndex, 20);
                                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(rowIndex, matchingColumn.ColumnIndex);
                                            Style style = cell.GetStyle();
                                            style.Font.Name = "Times New Roman";
                                            style.Font.Color = Color.Black;
                                            style.Font.Size = 14;
                                            style.Font.IsBold = false;
                                            cell.SetStyle(style);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(rowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                            dataIndices.Add(matchingColumn.ColumnIndex);

                                        }
                                        else
                                        {
                                            TableDefinitionsWorksheet.Cells[rowIndex, matchingColumn.ColumnIndex].PutValue(field.Value.Value);
                                            TableDefinitionsWorksheet.Cells.SetColumnWidth(matchingColumn.ColumnIndex, 20);
                                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(rowIndex, matchingColumn.ColumnIndex);
                                            Style style = cell.GetStyle();
                                            style.Font.Name = "Times New Roman";
                                            style.Font.Color = Color.Black;
                                            style.Font.Size = 14;
                                            style.Font.IsBold = false;
                                            cell.SetStyle(style);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(rowIndex,
                                            () =>
                                            {
                                                return new List<int>();
                                            });
                                            dataIndices.Add(matchingColumn.ColumnIndex);
                                        }
                                    }
                                }
                            }
                        }
                        setHeaders = false;
                        rowIndex++;
                    }
                    if (colIndexByRow != null && colIndexByRow.Count !=0)
                    {
                        if(colIndexByRow.Count >=1)
                        {
                            foreach (var row in colIndexByRow)
                            {
                                if (row.Value != null && row.Value.Count != 0)
                                {
                                    if (row.Key == colIndexByRow.Keys.OrderBy(x => x).First())
                                    {
                                        var orderedColumnIndices = row.Value.OrderBy(x => x);
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
                                    else if (row.Key == colIndexByRow.Keys.OrderBy(x => x).Last())
                                    {
                                        var orderedColumnIndices = row.Value.OrderBy(x => x);
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
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream = TableDefinitionsWorkbook.SaveToStream();
                    fileContent = memoryStream.ToArray();
                }
            }
            return new VRAutomatedReportGeneratedFile()
            {
                FileName = "AutomatedReport.xls",
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
                foreach (var tableDefinition in tableDefinitions)
                {
                    var matchingQuery = queries.FindRecord(x => x.VRAutomatedReportQueryId == tableDefinition.VRAutomatedReportQueryId);
                    matchingQuery.ThrowIfNull("matchingQuery");
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
                    foreach (var tableDefinitionColumn in tableDefinitionColumns)
                    {
                        if (!queryColumns.Any(x => x.ColumnName == tableDefinitionColumn.FieldName))
                        {
                            context.Result = false;
                            context.ErrorMessage = string.Format("The field '{0}' was not found in query '{1}' after it has been edited.", tableDefinitionColumn.FieldTitle, matchingQuery.QueryTitle);
                            break;
                        }
                        else
                        {
                            context.Result = true;
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

        public AdvancedExcelFileGeneratorTableTitleDefinition TitleDefinition { get; set; }

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

    public class AdvancedExcelFileGeneratorTableTitleDefinition
    {
        public string Title { get; set; }

        public int ColumnIndex { get; set; }
    }
}
