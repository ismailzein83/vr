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

        public bool CompressFile { get; set; }

        public List<AdvancedExcelFileGeneratorTableDefinition> TableDefinitions { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixDefinition> MatrixDefinitions { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(this.FileTemplateId).Content;
            Workbook TableDefinitionsWorkbook = new Workbook(new System.IO.MemoryStream(bytes));
            Common.Utilities.ActivateAspose();
        

            if (this.TableDefinitions != null && TableDefinitions.Count > 0)
            {
                int tablesCount = this.TableDefinitions.Count;
                int tablesDone = 0;
                int tablesLeft = tablesCount;

                if (context.HandlerContext.EvaluatorContext != null)
                    context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("Started mapping tables.");
                foreach (var tableDef in this.TableDefinitions)
                {
                    tableDef.ColumnDefinitions.ThrowIfNull("tableDef.ColumnDefinitions");
                    Worksheet TableDefinitionsWorksheet = TableDefinitionsWorkbook.Worksheets[tableDef.SheetIndex];
                    var dataList = context.HandlerContext.GetDataList(tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    string dataListIdentifier = string.Format("{0}_{1}", tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    dataList.ThrowIfNull("dataList", dataListIdentifier);
                    dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);
                    dataList.FieldInfos.ThrowIfNull("dataList.FieldInfos", dataListIdentifier);

                    //Dictionary<int, bool> isSubTableByIndex = new Dictionary<int, bool>();
                    int titleRowIndex = tableDef.RowIndex;
                    int headerRowIndex = tableDef.RowIndex;
                    int dataRowIndex = tableDef.RowIndex;

                    if (tableDef.IncludeTitle && tableDef.Title!=null)
                    {
                        List<int> columnIndices = new List<int>();
                        foreach (var col in tableDef.ColumnDefinitions)
                        {
                            columnIndices.Add(col.ColumnIndex);
                        }
                        var orderedIndices = columnIndices.OrderBy(x => x);
                        int difference = 1;
                        var titleColumnIndex = orderedIndices.First();
                        if (orderedIndices.Count() > 1)
                        {
                            difference = orderedIndices.Last() - titleColumnIndex + 1;
                        }
                        TableDefinitionsWorksheet.Cells.Merge(titleRowIndex, titleColumnIndex, 1, difference);
                        SetStyleAndValue(ref TableDefinitionsWorksheet, titleRowIndex, titleColumnIndex, tableDef.Title, 16, true, TextAlignmentType.Center);
                        
                        headerRowIndex++;
                        dataRowIndex++;
                    }
                    if (tableDef.IncludeHeaders)
                    {
                        dataRowIndex++;
                    }
                    Dictionary<int, List<int>> colIndexByRow = new Dictionary<int, List<int>>();
                    bool setHeaders = true;

                    if (dataList.Items.Count > 0)
                    {
                        foreach (var item in dataList.Items)
                        {
                            foreach (var column in tableDef.ColumnDefinitions)
                            {
                                var field = item.Fields.FindRecord(x => x.Key == column.FieldName);
                                var fieldInfo = dataList.FieldInfos.GetRecord(field.Key);
                                if (field.Value != null)
                                {
                                    if (fieldInfo != null)
                                    {
                                        if (tableDef.IncludeHeaders && setHeaders)
                                        {
                                            SetStyleAndValue(ref TableDefinitionsWorksheet, headerRowIndex, column.ColumnIndex, fieldInfo.FieldTitle, 14, true, TextAlignmentType.Left);
                                            var headerIndices = colIndexByRow.GetOrCreateItem(headerRowIndex);
                                            headerIndices.Add(column.ColumnIndex);
                                        }
                                        if (fieldInfo.FieldType.RenderDescriptionByDefault() && field.Value.Description!=null)
                                        {
                                            SetStyleAndValue(ref TableDefinitionsWorksheet, dataRowIndex, column.ColumnIndex, field.Value.Description, 12, false, TextAlignmentType.Left);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex);
                                            dataIndices.Add(column.ColumnIndex);
                                            continue;
                                        }
                                    }
                                    if (field.Value.Value != null)
                                    {
                                        if (field.Value.Value is DateTime)
                                        {
                                            var date = Convert.ToDateTime(field.Value.Value);
                                            GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
                                            SetStyleAndValue(ref TableDefinitionsWorksheet, dataRowIndex, column.ColumnIndex, date.ToString(generalSettingsManager.GetDateTimeFormat()), 12, false, TextAlignmentType.Right);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex);
                                            dataIndices.Add(column.ColumnIndex);
                                            continue;

                                        }
                                        else if (field.Value.Value is int || field.Value.Value is double || field.Value.Value is decimal || field.Value.Value is long)
                                        {
                                            SetStyleAndValue(ref TableDefinitionsWorksheet, dataRowIndex, column.ColumnIndex, field.Value.Value, 12, false, TextAlignmentType.Right);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex);
                                            dataIndices.Add(column.ColumnIndex);
                                            continue;

                                        }
                                        else
                                        {
                                            SetStyleAndValue(ref TableDefinitionsWorksheet, dataRowIndex, column.ColumnIndex, field.Value.Value, 12, false, TextAlignmentType.Left);
                                            var dataIndices = colIndexByRow.GetOrCreateItem(dataRowIndex);
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
                        if (context.HandlerContext.EvaluatorContext != null)
                            context.HandlerContext.EvaluatorContext.WriteWarningBusinessTrackingMsg("No data was found for file.");
                        if (tableDef.IncludeHeaders && setHeaders)
                        {
                            foreach (var col in tableDef.ColumnDefinitions)
                            {

                                SetStyleAndValue(ref TableDefinitionsWorksheet, headerRowIndex, col.ColumnIndex, col.FieldTitle, 14, true, TextAlignmentType.Left);
                                var headerIndices = colIndexByRow.GetOrCreateItem(headerRowIndex);
                                headerIndices.Add(col.ColumnIndex);
                            }
                        }
                        setHeaders = false;
                    }
                    SetBorders(colIndexByRow, TableDefinitionsWorksheet);
                    if (context.HandlerContext.EvaluatorContext != null)
                    {
                        tablesDone++;
                        tablesLeft = tablesCount - tablesDone;
                        if(tablesDone==1)
                            context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("Finished mapping 1 table. The number of tables left is {0} out of {1} tables.", tablesLeft, tablesCount);
                        else
                            context.HandlerContext.EvaluatorContext.WriteInformationBusinessTrackingMsg("Finished mapping {0} tables. The number of tables left is {1} out of {2} tables.", tablesDone, tablesLeft, tablesCount);
                    }
                }
            }
            else
            {
                if (context.HandlerContext.EvaluatorContext != null)
                    context.HandlerContext.EvaluatorContext.WriteErrorBusinessTrackingMsg("No tables were mapped.");
            }
          
            MemoryStream memoryStream = new MemoryStream();
            memoryStream = TableDefinitionsWorkbook.SaveToStream();
        
            return new VRAutomatedReportGeneratedFile()
            {
                FileContent = memoryStream.ToArray()
            };

        }

        public override void Validate(IVRAutomatedReportHandlerValidateContext context)
        {
            this.TableDefinitions.ThrowIfNull("No table definitions were added.");
            if (context.Queries != null && context.Queries.Count > 0)
            {
                foreach (var tableDefinition in this.TableDefinitions)
                {
                    var matchingQuery = context.Queries.FindRecord(x => x.VRAutomatedReportQueryId == tableDefinition.VRAutomatedReportQueryId);
                    if (matchingQuery == null)
                    {
                        context.Result = QueryHandlerValidatorResult.Failed;
                        context.ErrorMessage = string.Format("A query used in the handler has been deleted.");
                        break;
                    }
                    matchingQuery.Settings.ThrowIfNull("matchingQuery.Settings");
                    if (matchingQuery.Settings is RecordSearchQuerySettings)
                    {
                        var querySettings = matchingQuery.Settings.CastWithValidate<RecordSearchQuerySettings>("matchingQuery.Settings");
                        querySettings.Columns.ThrowIfNull("querySettings.Columns", matchingQuery.VRAutomatedReportQueryId);
                        tableDefinition.ColumnDefinitions.ThrowIfNull("tableDefinition.ColumnDefinitions");
                        List<string> missingTableFields = new List<string>();
                        foreach (var tableDefinitionColumn in tableDefinition.ColumnDefinitions)
                        {
                            if (!querySettings.Columns.Any(x => x.ColumnName == tableDefinitionColumn.FieldName))
                            {
                                if (!missingTableFields.Contains(tableDefinitionColumn.FieldTitle))
                                    missingTableFields.Add(tableDefinitionColumn.FieldTitle);
                            }

                        }
                        if (missingTableFields != null && missingTableFields.Count == 1)
                        {
                            context.Result = QueryHandlerValidatorResult.Failed;
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
                            context.Result = QueryHandlerValidatorResult.Failed;
                            context.ErrorMessage = string.Format("The fields '{0}' were not found in query '{1}' after it has been edited.", joinedFields, matchingQuery.QueryTitle);
                            break;
                        }
                    }
                    else if (matchingQuery.Settings is AnalyticTableQuerySettings)
                    {
                        var querySettings = matchingQuery.Settings.CastWithValidate<AnalyticTableQuerySettings>("matchingQuery.Settings");
                        querySettings.Dimensions.ThrowIfNull("querySettings.Dimensions", matchingQuery.VRAutomatedReportQueryId);
                        querySettings.Measures.ThrowIfNull("querySettings.Measures", matchingQuery.VRAutomatedReportQueryId);

                        if (tableDefinition.ColumnDefinitions != null && tableDefinition.ColumnDefinitions.Count>0)
                        {
                            List<string> missingTableFields = new List<string>();
                            foreach (var tableDefinitionColumn in tableDefinition.ColumnDefinitions)
                            {
                                if (!querySettings.Dimensions.Any(x => x.DimensionName == tableDefinitionColumn.FieldName) && !querySettings.Measures.Any(x => x.MeasureName == tableDefinitionColumn.FieldName))
                                {
                                    if (!missingTableFields.Contains(tableDefinitionColumn.FieldTitle))
                                        missingTableFields.Add(tableDefinitionColumn.FieldTitle);
                                }

                            }
                            if (missingTableFields != null && missingTableFields.Count == 1)
                            {
                                context.Result = QueryHandlerValidatorResult.Failed;
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
                                context.Result = QueryHandlerValidatorResult.Failed;
                                context.ErrorMessage = string.Format("The fields '{0}' were not found in query '{1}' after it has been edited.", joinedFields, matchingQuery.QueryTitle);
                                break;
                            }
                        }
                        if (tableDefinition.SubTableDefinitions != null && tableDefinition.SubTableDefinitions.Count > 0)
                        {
                            List<string> missingTableFields = new List<string>();
                            if (querySettings.SubTables == null || querySettings.SubTables.Count == 0)
                            {
                                context.Result = QueryHandlerValidatorResult.Failed;
                                context.ErrorMessage = string.Format("No subtables were found in query '{0}'.", matchingQuery.QueryTitle);
                                break;
                            }
                            foreach (var subTable in tableDefinition.SubTableDefinitions)
                            {
                                var matchingSubTable = querySettings.SubTables.FindRecord(x => x.SubTableId == subTable.SubTableId);
                                if (matchingSubTable == null)
                                {
                                    context.Result = QueryHandlerValidatorResult.Failed;
                                    context.ErrorMessage = string.Format("A subtable from query '{0}' used in the handler has been deleted.", matchingQuery.QueryTitle);
                                    break;
                                }
                                foreach (var field in subTable.SubTableFields)
                                {
                                    if (!matchingSubTable.Dimensions.Any(x => x == field.FieldName) && !matchingSubTable.Measures.Any(x => x == field.FieldName))
                                    {
                                        if (!missingTableFields.Contains(field.FieldName))
                                            missingTableFields.Add(field.FieldName);
                                    }
                                }
                                if (missingTableFields != null && missingTableFields.Count == 1)
                                {
                                    context.Result = QueryHandlerValidatorResult.Failed;
                                    context.ErrorMessage = string.Format("The field '{0}' was not found in query '{1}' in subtable '{2}' after it has been edited.", missingTableFields.First(), matchingQuery.QueryTitle, subTable.SubTableName);
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
                                    context.Result = QueryHandlerValidatorResult.Failed;
                                    context.ErrorMessage = string.Format("The fields '{0}' were not found in query '{1}' in subtable '{2}' after it has been edited.", joinedFields, matchingQuery.QueryTitle, subTable.SubTableName);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void SetBorders(Dictionary<int, List<int>> colIndexByRow, Worksheet TableDefinitionsWorksheet)
        {
            if (colIndexByRow != null && colIndexByRow.Count > 0)
            {
                if (colIndexByRow.Count > 1)
                {
                    foreach (var row in colIndexByRow)
                    {
                        if (row.Value != null && row.Value.Count > 0)
                        {
                            if (row.Key == colIndexByRow.Keys.OrderBy(x => x).First())
                            {
                                var orderedColumnIndices = row.Value.OrderBy(x => x);
                                if (orderedColumnIndices.Count() == 1)
                                {
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, orderedColumnIndices.First());
                                    Style style = cell.GetStyle();
                                    SetBorder(style, true, true, false, true);
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
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, orderedColumnIndices.First());
                                    Style style = cell.GetStyle();
                                    SetBorder(style, false, true, true, true);
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
                                    Cell cell = TableDefinitionsWorksheet.Cells.GetCell(row.Key, orderedColumnIndices.First());
                                    Style style = cell.GetStyle();
                                    SetBorder(style, false, true, false, true);
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
                    if (onlyRow.Value != null && onlyRow.Value.Count > 0)
                    {
                        var orderedColumnIndices = onlyRow.Value.OrderBy(x => x);
                        if (orderedColumnIndices.Count() == 1)
                        {
                            Cell cell = TableDefinitionsWorksheet.Cells.GetCell(onlyRow.Key, orderedColumnIndices.First());
                            Style style = cell.GetStyle();
                            SetBorder(style, true, true, true, true);
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

        private void SetStyleAndValue(ref Worksheet worksheet, int rowIndex, int columnIndex, object value, int fontSize, bool isBold, TextAlignmentType alignment)
        {
            if(fontSize!=16)
                worksheet.Cells.SetColumnWidth(columnIndex, 20);
            worksheet.Cells[rowIndex, columnIndex].PutValue(value);
            Cell cell = worksheet.Cells.GetCell(rowIndex, columnIndex);
            Style style = cell.GetStyle();
            style.Font.Name = "Times New Roman";
            style.Font.Color = Color.Black;
            style.Font.Size = fontSize;
            style.Font.IsBold = isBold;
            style.HorizontalAlignment = alignment;
            cell.SetStyle(style);
        }
        private void SetBorder(Style style, bool setTopBorder, bool setRightBorder, bool setBottomBorder, bool setLeftBorder)
        {
            if(setTopBorder)
            {
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
            }
            if (setRightBorder)
            {
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
            }
            if (setBottomBorder)
            {
                style.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            }
            if (setLeftBorder)
            {
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
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

        public List<AdvancedExcelFileGeneratorSubTableDefinition> SubTableDefinitions { get; set; }
    }

    public class AdvancedExcelFileGeneratorTableColumnDefinition
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public int ColumnIndex { get; set; }

        public bool UseFieldDescription { get; set; }
    }

    public class AdvancedExcelFileGeneratorSubTableDefinition
    {
        public int ColumnIndex { get; set; }

        public Guid SubTableId { get; set; }

        public string SubTableName { get; set; }

        //public List<string> SubTableFields { get; set; }

        public List<AdvancedExcelFileGeneratorSubTableColumnDefinition> SubTableFields { get; set; }
    }

    public class AdvancedExcelFileGeneratorSubTableColumnDefinition
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }
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
