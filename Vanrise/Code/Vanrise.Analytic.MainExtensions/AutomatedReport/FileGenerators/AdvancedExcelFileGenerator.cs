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
using System.Collections;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators
{
    public class AdvancedExcelFileGenerator : VRAutomatedReportFileGeneratorSettings
    {
        GeneralSettingsManager generalSettingsManager = new GeneralSettingsManager();
        public override Guid ConfigId
        {
            get { return new Guid("9FAAE9B2-931E-4B3F-BDA4-B0F3B7647488"); }
        }
        public override void OnAfterSaveAction(IVRAutomatedReportFileGeneratorOnAfterSaveActionContext context)
        {
            VRFileManager fileManager = new VRFileManager();


            var filesettings = new VRFileSettings
            {
                ExtendedSettings = new Vanrise.Analytic.Business.ReportGenerationFileSettings
                {
                    TaskId = context.TaskId,
                    VRReportGenerationId = context.VRReportGenerationId
                }
            };

            fileManager.SetFileUsedAndUpdateSettings(FileUniqueId, filesettings);



        }

        public Guid FileUniqueId { get; set; }

        public bool CompressFile { get; set; }

        public List<AdvancedExcelFileGeneratorTableDefinition> TableDefinitions { get; set; }

        public List<AdvancedExcelFileGeneratorMatrixDefinition> MatrixDefinitions { get; set; }

        public override VRAutomatedReportGeneratedFile GenerateFile(IVRAutomatedReportFileGeneratorGenerateFileContext context)
        {
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFileByUniqueId(this.FileUniqueId).Content;
            Workbook tableDefinitionsWorkbook = new Workbook(new System.IO.MemoryStream(bytes));
            Common.Utilities.ActivateAspose();
            if (this.TableDefinitions != null && TableDefinitions.Count > 0)
            {
                int tablesCount = this.TableDefinitions.Count;
                int tablesDone = 0;
                int tablesLeft = tablesCount;

                foreach (var tableDef in this.TableDefinitions)
                {
                    var dataList = context.HandlerContext.GetDataList(tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    string dataListIdentifier = string.Format("{0}_{1}", tableDef.VRAutomatedReportQueryId, tableDef.ListName);
                    dataList.ThrowIfNull("dataList", dataListIdentifier);
                    dataList.Items.ThrowIfNull("dataList.Items", dataListIdentifier);
                    dataList.FieldInfos.ThrowIfNull("dataList.FieldInfos", dataListIdentifier);

                    if (dataList.Items.Count > 0)
                    {
                        BuildTableExcelTableDefinition(tableDef, tableDefinitionsWorkbook, dataList);
                    }
                    else
                    {
                        if (context.HandlerContext.EvaluatorContext != null)
                            context.HandlerContext.EvaluatorContext.WriteWarningBusinessTrackingMsg("No data was found.");
                    }

                    if (context.HandlerContext.EvaluatorContext != null)
                    {
                        tablesDone++;
                        tablesLeft = tablesCount - tablesDone;
                        if (tablesDone == 1)
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
            memoryStream = tableDefinitionsWorkbook.SaveToStream();
            return new VRAutomatedReportGeneratedFile()
            {
                FileContent = memoryStream.ToArray()
            };

        }
        private void BuildTableExcelTableDefinition(AdvancedExcelFileGeneratorTableDefinition tableDef, Workbook tableDefinitionsWorkbook, VRAutomatedReportResolvedDataList dataList)
        {
            Worksheet worksheet = tableDefinitionsWorkbook.Worksheets[tableDef.SheetIndex];
            int titleRowIndex = tableDef.RowIndex;

            var tableDefinitionInfoContext = new TableDefinitionInfoContext(tableDef.SubTableDefinitions, tableDef.ColumnDefinitions, dataList.SubTablesInfo, dataList.FieldInfos);

            var indexesNeeded = EvaluateStartingRows(tableDef.RowIndex, tableDef.IncludeHeaders, tableDef.IncludeTitle, tableDef.Titles, tableDefinitionInfoContext);

            List<int> allColumnIndices = new List<int>(); // this is the list of the actual column indices mapped including the ones programmatically added if a subtable is present. They are used for the title(s)
            int subTableValuesCount = 0;//this refers to how much a subtable required columns and hence how many columns should be shifted for the next field to be mapped
           
            List<int> insertedRows = new List<int>();
            List<int> insertedCols = new List<int>();
            #region Title Rows
            if (tableDef.IncludeTitle)
            {
                int currentTitleIndex = titleRowIndex;
                if (tableDef.Titles != null)
                {
                    for (int i = 0; i < tableDef.Titles.Count - 1; i++)
                    {
                        InsertRow(insertedRows, currentTitleIndex, worksheet);
                        currentTitleIndex++;
                    }
                }
            }
            #endregion

            if (tableDef.IncludeHeaders)
            {
                BuildTableHeaders(tableDefinitionInfoContext,worksheet, indexesNeeded, out subTableValuesCount, insertedRows, insertedCols);
            }
            if (dataList.Items.Count > 0)
            {
                int summaryRowIndex = indexesNeeded.ColDataRowIndex + dataList.Items.Count;
                BuildTableData(tableDefinitionInfoContext,worksheet, allColumnIndices, dataList, indexesNeeded, subTableValuesCount, insertedRows, tableDef.IncludeTitle, tableDef.IncludeHeaders, insertedCols);
                if (tableDef.IncludeSummary && dataList.SummaryDataItem != null)
                {
                    BuildTableSummaryData(tableDefinitionInfoContext,worksheet, dataList, summaryRowIndex, insertedRows);
                }
            }
            else
            {
                if (tableDef.IncludeHeaders)
                {
                    foreach (var col in tableDef.ColumnDefinitions)
                    {
                        SetStyleAndValue(worksheet, indexesNeeded.HeaderRowIndex, col.ColumnIndex, col.FieldTitle, 14, true, TextAlignmentType.Left, true);
                    }
                }
            }
            if (tableDef.IncludeTitle && tableDef.Titles != null && allColumnIndices.Count > 0)
            {
                var orderedIndices = allColumnIndices.Distinct().OrderBy(x => x);
                int difference = 1;
                var titleColumnIndex = orderedIndices.First();
                if (orderedIndices.Count() > 1)
                {
                    difference = orderedIndices.Last() - titleColumnIndex + 1;
                }
                foreach (var title in tableDef.Titles)
                {
                    worksheet.Cells.Merge(titleRowIndex, titleColumnIndex, 1, difference);
                    SetStyleAndValue(worksheet, titleRowIndex, titleColumnIndex, title, 16, true, TextAlignmentType.Center, false);
                    titleRowIndex++;
                }
            }
        }
        private IndexesNeeded EvaluateStartingRows(int rowIndex, bool includeHeaders, bool includeTitle, List<string> titles, ITableDefinitionInfoContext context)
        {
            IndexesNeeded indexesNeeded = new IndexesNeeded
            {
                HeaderRowIndex = rowIndex,
                SubTableDataRowIndex = rowIndex,
                ColDataRowIndex = rowIndex,
            };
            if (includeHeaders)
            {
                var presentSubTablesInfo = context.GetSubTablesInfo();
                if (presentSubTablesInfo != null && presentSubTablesInfo.Count > 0)
                {
                    AdvancedExcelFileGeneratorSubTableDefinition subTableDefWithMostHeaders = null;
                    foreach (var item in presentSubTablesInfo)
                    {

                        var fieldsCount = item.ReportTableInfo.FieldsOrder.Count();//this is the number of headers in a subtable
                        var subTableFieldsCount = item.TableDefinition.SubTableFields.Count();//this is the number of field titles in a subtable
                        if (subTableFieldsCount > 1)//if the number of field titles is more than one then they should be displayed and hence a row should be added
                        {
                            fieldsCount++;
                        }
                        if (fieldsCount > indexesNeeded.MaxHeaderRows)
                        {
                            indexesNeeded.MaxHeaderRows = fieldsCount;//this is to make sure that the maxHeaderRows includes the field titles' row if there is more than one field title
                            subTableDefWithMostHeaders = item.TableDefinition;
                        }
                    }
                    indexesNeeded.MaxSubTableHeaders = indexesNeeded.MaxHeaderRows;
                    indexesNeeded.HeaderRowIndex += indexesNeeded.MaxHeaderRows - 1;

                    if (subTableDefWithMostHeaders != null && subTableDefWithMostHeaders.SubTableTitle != null && presentSubTablesInfo.Count > 0)
                    {
                        indexesNeeded.SubTableDataRowIndex++;
                        indexesNeeded.MaxHeaderRows++;
                        indexesNeeded.HeaderRowIndex++;
                        indexesNeeded.ColDataRowIndex++;
                    }
                }
                indexesNeeded.ColDataRowIndex = indexesNeeded.HeaderRowIndex + 1;

            }
            if (includeTitle && titles != null && titles.Count > 0)
            {
                int titlesCount = titles.Count;
                indexesNeeded.HeaderRowIndex += titlesCount;
                indexesNeeded.ColDataRowIndex += titlesCount;
                indexesNeeded.SubTableDataRowIndex += titlesCount;
            }
            return indexesNeeded;
        }
        private void InsertTitleRows(bool includeTitle, int titleRowIndex, List<string> titles, Worksheet worksheet, List<int> insertedRows)
        {
            if (includeTitle)
            {
                int currentTitleIndex = titleRowIndex;
                if (titles != null)
                {
                    for (int i = 0; i < titles.Count - 1; i++)
                    {
                        if (!insertedRows.Contains(currentTitleIndex + 1))
                        {
                            worksheet.Cells.InsertRow(currentTitleIndex + 1);
                            insertedRows.Add(currentTitleIndex + 1);
                        }
                        currentTitleIndex++;
                    }
                }
            }
        }
        private void BuildTableHeaders(ITableDefinitionInfoContext context, Worksheet worksheet, IndexesNeeded indexesNeeded, out int subTableValuesCount, List<int> insertedRows, List<int> insertedCols)
        {
            int columnIndexState = context.GetSortedColumnsIndexes().First();
            int maxSubTableRow = 0;//this is used to know how many rows the headers took and hence the rowIndex of the subtable data
            subTableValuesCount = 0;
            int preColumnIndex = 0;
            InsertRowsForHeaders(insertedRows, insertedCols, worksheet, indexesNeeded.HeaderRowIndex, indexesNeeded.SubTableDataRowIndex, indexesNeeded.MaxSubTableHeaders, indexesNeeded.MaxHeaderRows);

            foreach (var columnIndex in context.GetSortedColumnsIndexes())
            {
                columnIndexState += (columnIndex - preColumnIndex);//columnIndexState should be shifted by the difference between it and the next column index always

                var column = context.GetColumnDefinitionByColIndex(columnIndex);
                if (column != null)
                {
                    if (column.FieldTitle != null)
                    {
                        SetStyleAndValue(worksheet, indexesNeeded.HeaderRowIndex, columnIndexState, column.FieldTitle, 14, true, TextAlignmentType.Center, true);
                    }
                    else
                    {
                        var fieldInfo = context.GetFieldInfo(column.FieldName);
                        if (fieldInfo != null)
                        {
                            SetStyleAndValue(worksheet, indexesNeeded.HeaderRowIndex, columnIndexState, fieldInfo.FieldTitle, 14, true, TextAlignmentType.Center, true);
                        }
                    }
                }
                else
                {
                    var subTableDef = context.GetSubTableDefinitionByColIndex(columnIndex);
                    if (subTableDef != null)
                    {
                        int subTableFieldsCount = subTableDef.SubTableFields.Count;
                        var subTableInfo = context.GetSubTableInfo(subTableDef.SubTableId);
                        int currentSubTableRowIndex = indexesNeeded.SubTableDataRowIndex;
                        int subTableTitleRowIndex = currentSubTableRowIndex;
                        int subTableTitleColIndex = columnIndexState;

                        int subTableFirstRowIndex = 0;
                        if (subTableInfo != null && subTableInfo.FieldsInfo != null && subTableInfo.FieldsInfo.Count > 0 && subTableInfo.FieldsOrder != null && subTableInfo.FieldsOrder.Count > 0)
                        {
                            int fieldsOrderCount = subTableInfo.FieldsOrder.Count;
                            if (subTableFieldsCount > 1)
                                fieldsOrderCount++;

                            if (maxSubTableRow < fieldsOrderCount)
                                maxSubTableRow = fieldsOrderCount;

                            if (indexesNeeded.MaxSubTableHeaders > fieldsOrderCount)
                            {
                                currentSubTableRowIndex += (indexesNeeded.MaxSubTableHeaders - fieldsOrderCount);//this is to accomodate for the difference between the current subtable headers and the maximum subtable headers
                                subTableTitleRowIndex = currentSubTableRowIndex;
                            }
                            subTableFirstRowIndex = currentSubTableRowIndex;
                            
                            List<FieldValueRange> parentRanges = new List<FieldValueRange>();//this list is for merging purposes. at every row, merging can happen only if the repeated values happen to fall under a merged cell in the parent row

                            for (var i = 0; i < subTableInfo.FieldsOrder.Count; i++)
                            {
                                var field =  subTableInfo.FieldsOrder[i];
                                var fieldStartingIndex = columnIndexState;
                                var fieldValues = subTableInfo.FieldsInfo[field];
                                List<FieldValueRange> currentRanges = new List<FieldValueRange>();
                                if (fieldValues != null && fieldValues.FieldValues != null && fieldValues.FieldValues.Count > 0 && fieldValues.FieldType != null)
                                {
                                    subTableValuesCount = fieldValues.FieldValues.Count * subTableFieldsCount;//this is in case there is more than one field
                                    object previousValue = null;
                                    var fieldValueRange = new FieldValueRange
                                    {
                                        StartIndex = -1,
                                        EndIndex = -1
                                    };
                                    for (int iteration = 0; iteration < fieldValues.FieldValues.Count; iteration++)
                                    {
                                        int currentSubTableField = 0;
                                        var fieldValue = fieldValues.FieldValues[iteration];
                                        if (fieldValue != null)
                                        {

                                            Object fieldValueObject = fieldValue.Value;
                                            if (fieldValues.FieldType.RenderDescriptionByDefault())
                                            {
                                                fieldValueObject = fieldValue.Description;
                                            }
                                            else if (fieldValue.Value is DateTime)
                                            {
                                                var date = Convert.ToDateTime(fieldValue.Value);
                                                fieldValueObject = date.ToString(generalSettingsManager.GetDateTimeFormat());
                                            }
                                            SetColumnValue(worksheet, insertedCols, subTableFieldsCount, currentSubTableRowIndex, subTableFirstRowIndex, parentRanges, ref fieldStartingIndex, currentRanges, ref previousValue, fieldValueRange, iteration, ref currentSubTableField, fieldValueObject, fieldValues.FieldValues.Count);
                                           
                                            if (subTableValuesCount == 1 && subTableFieldsCount > 1)
                                            {
                                                worksheet.Cells.Merge(currentSubTableRowIndex, fieldValueRange.StartIndex, 1, subTableFieldsCount);
                                                fieldValueRange.EndIndex +=  subTableFieldsCount;
                                                currentRanges.Add(fieldValueRange);
                                            }
                                            if (i == subTableInfo.FieldsOrder.Count - 1 && subTableDef.SubTableFields != null && subTableDef.SubTableFields.Count > 1)
                                            {
                                                var subTableFieldStartingIndex = fieldStartingIndex - subTableDef.SubTableFields.Count;
                                                foreach (var subTableField in subTableDef.SubTableFields)
                                                {
                                                    SetStyleAndValue(worksheet, currentSubTableRowIndex + 1, subTableFieldStartingIndex, subTableField.FieldName, 14, true, TextAlignmentType.Center, true);
                                                    subTableFieldStartingIndex++;
                                                }
                                            }
                                        }
                                    }
                                    MergeCells(worksheet, currentSubTableRowIndex, subTableFirstRowIndex, fieldValueRange, currentRanges, parentRanges);
                                    parentRanges = currentRanges;
                                }
                                currentSubTableRowIndex++;
                            }
                            columnIndexState += subTableValuesCount - 1;//this is to shift the columnIndexState to accomodate the existence of a subtable
                        }
                        #region BuildSubTableTitle
                        if (subTableDef.SubTableTitle != null)
                        {
                            SetStyleValueAndBordersForTitleCell(worksheet, subTableTitleRowIndex - 1, subTableTitleColIndex, subTableDef.SubTableTitle, subTableValuesCount);
                            worksheet.Cells.Merge(subTableTitleRowIndex - 1, subTableTitleColIndex, 1, subTableValuesCount);
                        }
                        #endregion
                    }
                }
                preColumnIndex = columnIndex;
            }
            indexesNeeded.SubTableDataRowIndex += maxSubTableRow;
        }
        private void BuildTableData( ITableDefinitionInfoContext context,Worksheet worksheet, List<int> allColumnIndices, VRAutomatedReportResolvedDataList dataList, IndexesNeeded indexesNeeded, int subTableValuesCount, List<int> insertedRows, bool includeTitle, bool includeHeaders, List<int> insertedCols)
        {
            var items = dataList.Items;
            var fieldInfos = dataList.FieldInfos;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int columnIndexState = context.GetSortedColumnsIndexes().First();
                int preColumnIndex = 0;
                foreach (var col in context.GetSortedColumnsIndexes())
                {
                   columnIndexState += (col - preColumnIndex);
                   var column = context.GetColumnDefinitionByColIndex(col); ;
                    if (column != null)
                    {
                        if (i != items.Count - 1)
                        {
                            InsertRow(insertedRows, indexesNeeded.ColDataRowIndex, worksheet);
                        }
                        allColumnIndices.Add(columnIndexState);
                        var field = item.Fields.GetRecord(column.FieldName);
                        var fieldInfo = fieldInfos.GetRecord(column.FieldName);
                        if (field != null && fieldInfo != null && fieldInfo.FieldType != null)
                        {
                            Object fieldValueObject = field.Value;
                            var textAlignmentType = TextAlignmentType.Left;
                            if (fieldInfo.FieldType.RenderDescriptionByDefault())
                            {
                                fieldValueObject = field.Description;
                            }
                            else
                            {
                                if (field.Value is DateTime)
                                {
                                    var date = Convert.ToDateTime(field.Value);
                                    fieldValueObject = date.ToString(generalSettingsManager.GetDateTimeFormat());
                                    textAlignmentType = TextAlignmentType.Right;
                                }
                                else if (field.Value is int || field.Value is double || field.Value is decimal || field.Value is long)
                                {
                                    textAlignmentType = TextAlignmentType.Right;
                                }
                            }
                            SetStyleAndValue(worksheet, indexesNeeded.ColDataRowIndex, columnIndexState, fieldValueObject, 12, false, textAlignmentType, true);
                        }
                    }
                    else
                    {
                        var subTableDef = context.GetSubTableDefinitionByColIndex(col);
                        if (subTableDef != null)
                        {
                            int subTableFirstRowIndex = indexesNeeded.SubTableDataRowIndex + i;
                            int subTableFieldsCount = subTableDef.SubTableFields.Count;
                            item.SubTables.ThrowIfNull("item.SubTables");
                            var subTableItem = item.SubTables.GetRecord(subTableDef.SubTableId);
                            if (subTableItem != null && subTableItem.Fields != null && subTableItem.Fields.Count > 0 && subTableDef.SubTableFields != null && subTableDef.SubTableFields.Count > 0)
                            {
                                int valuesCount = 0;
                                bool insertColumns = true;

                                for (int k = 0; k < subTableDef.SubTableFields.Count; k++)
                                {

                                    var fieldDef = subTableDef.SubTableFields[k];
                                    var fieldStartingIndex = columnIndexState + k;

                                    var field = subTableItem.Fields.GetRecord(fieldDef.FieldName);
                                    if (i != items.Count - 1)
                                    {
                                        InsertRow(insertedRows, subTableFirstRowIndex, worksheet);
                                    }
                                    if (field != null && field.FieldValues != null && field.FieldValues.Count > 0)
                                    {
                                        valuesCount = field.FieldValues.Count * subTableFieldsCount;
                                        if (!includeHeaders && subTableFieldsCount > 1 && insertColumns)
                                        {
                                            int colCount = 0;
                                            while (colCount < valuesCount - 1)
                                            {
                                                InsertColumn(insertedCols, columnIndexState + colCount, worksheet);
                                                colCount++;
                                            }
                                            insertColumns = false;
                                        }
                                        for (int o = 0; o < field.FieldValues.Count; o++)
                                        {
                                            var fieldValue = field.FieldValues[o];
                                            if (o > 0 && subTableFieldsCount > 1)
                                            {
                                                fieldStartingIndex += subTableDef.SubTableFields.Count;
                                            }
                                            if (!includeHeaders && subTableFieldsCount == 1)
                                            {
                                                if (o != field.FieldValues.Count - 1)
                                                {
                                                    InsertColumn(insertedCols, fieldStartingIndex, worksheet);
                                                }
                                            }
                                            allColumnIndices.Add(fieldStartingIndex);
                                           
                                            Object fieldValueObject = fieldValue.Value;
                                            var textAlignmentType = TextAlignmentType.Left;
                                            if (fieldValue.Value is DateTime)
                                            {
                                                var date = Convert.ToDateTime(fieldValue.Value);
                                                fieldValueObject = date.ToString(generalSettingsManager.GetDateTimeFormat());
                                                textAlignmentType = TextAlignmentType.Right;
                                            }
                                            else if (fieldValue.Value is int || fieldValue.Value is double || fieldValue.Value is decimal || fieldValue.Value is long)
                                            {
                                                textAlignmentType = TextAlignmentType.Right;
                                            }
                                            SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, fieldValueObject, 12, false, textAlignmentType, true);
                                            if (subTableFieldsCount == 1)
                                                fieldStartingIndex++;
                                        }
                                    }
                                }
                                subTableFirstRowIndex++;
                                columnIndexState += valuesCount - 1;
                            }
                            else
                            {
                                int valuesCount = 0;
                                foreach (var fieldDef in subTableDef.SubTableFields)
                                {
                                    var fieldStartingIndex = columnIndexState;
                                    valuesCount = subTableValuesCount;

                                    InsertRow(insertedRows, subTableFirstRowIndex, worksheet);
                                   
                                    for (int j = 0; j < subTableValuesCount; j++)
                                    {
                                        SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, null, 12, false, TextAlignmentType.Left, true);
                                        fieldStartingIndex++;
                                    }
                                    subTableFirstRowIndex++;
                                }
                                columnIndexState += valuesCount - 1;
                            }
                        }
                    }
                    preColumnIndex = col;
               }
                indexesNeeded.ColDataRowIndex++;
            }
        }
        private void BuildTableSummaryData( ITableDefinitionInfoContext context, Worksheet worksheet, VRAutomatedReportResolvedDataList dataList, int summaryRowIndex, List<int> insertedRows)
        {
            var fieldInfos = dataList.FieldInfos;
            var subTablesInfo = dataList.SubTablesInfo;
            var summaryRecord = dataList.SummaryDataItem;
            int columnIndexState = context.GetSortedColumnsIndexes().First();
            int preColumnIndex = 0;
            foreach (var col in context.GetSortedColumnsIndexes())
            {
                columnIndexState += (col - preColumnIndex);
                var column = context.GetColumnDefinitionByColIndex(col);
                if (column != null)
                {
                    var summaryField = (summaryRecord != null && summaryRecord.Fields != null) ? summaryRecord.Fields.GetRecord(column.FieldName) : null;
                    var fieldInfo = fieldInfos.GetRecord(column.FieldName);
                    if (summaryField != null && fieldInfo != null && fieldInfo.FieldType != null)
                    {
                        InsertRow(insertedRows, summaryRowIndex - 1, worksheet);
                        Object fieldValueObject = summaryField.Value;
                        var textAlignmentType = TextAlignmentType.Left;
                        if (fieldInfo.FieldType.RenderDescriptionByDefault())
                        {
                            fieldValueObject = summaryField.Description;
                        }
                        else
                        {
                            if (summaryField.Value is DateTime)
                            {
                                var summaryDate = Convert.ToDateTime(summaryField.Value);
                                fieldValueObject = summaryDate.ToString(generalSettingsManager.GetDateTimeFormat());
                                textAlignmentType = TextAlignmentType.Right;
                            }
                            else if (summaryField.Value is int || summaryField.Value is double || summaryField.Value is decimal || summaryField.Value is long)
                            {
                                textAlignmentType = TextAlignmentType.Right;
                            }
                        }
                        SetStyleAndValue(worksheet, summaryRowIndex, columnIndexState, fieldValueObject, 14, true, textAlignmentType, true);
                    }
                }
                else
                {
                    var subTableDef = context.GetSubTableDefinitionByColIndex(col);
                    if (subTableDef != null)
                    {
                        var subTableInfo = subTablesInfo.GetRecord(subTableDef.SubTableId);
                        int subTableFieldsCount = subTableDef.SubTableFields.Count;
                        VRAutomatedReportResolvedDataItemSubTable summarySubTableFields = (summaryRecord != null && summaryRecord.SubTables != null) ? summaryRecord.SubTables.GetRecord(subTableDef.SubTableId) : null;
                        if (summarySubTableFields != null && summarySubTableFields.Fields != null && summarySubTableFields.Fields.Count > 0 && subTableDef.SubTableFields != null && subTableDef.SubTableFields.Count > 0)
                        {
                            InsertRow(insertedRows, summaryRowIndex - 1, worksheet);
                            int valuesCount = 0;
                            for (int k = 0; k < subTableDef.SubTableFields.Count; k++)
                            {
                                var fieldDef = subTableDef.SubTableFields[k];
                                var fieldStartingIndex = columnIndexState + k;
                                var summaryField = (summarySubTableFields != null && summarySubTableFields.Fields != null) ? summarySubTableFields.Fields.GetRecord(fieldDef.FieldName) : null;
                                if (summaryField != null && summaryField.FieldValues != null && summaryField.FieldValues.Count > 0)
                                {
                                    valuesCount = summaryField.FieldValues.Count * subTableFieldsCount;
                                    for (int o = 0; o < summaryField.FieldValues.Count; o++)
                                    {
                                        var summaryFieldValue = (summaryField != null && summaryField.FieldValues != null) ? summaryField.FieldValues[o] : null;
                                        if (o > 0 && subTableFieldsCount > 1)
                                        {
                                            fieldStartingIndex += subTableDef.SubTableFields.Count;
                                        }


                                        Object fieldValueObject = summaryFieldValue.Value;
                                        var textAlignmentType = TextAlignmentType.Left;
                                        if (summaryFieldValue.Value is DateTime)
                                        {
                                            var summaryDate = Convert.ToDateTime(summaryFieldValue.Value);
                                            fieldValueObject = summaryDate.ToString(generalSettingsManager.GetDateTimeFormat());
                                            textAlignmentType = TextAlignmentType.Right;
                                        }
                                        else if (summaryFieldValue.Value is int || summaryFieldValue.Value is double || summaryFieldValue.Value is decimal || summaryFieldValue.Value is long)
                                        {
                                            textAlignmentType = TextAlignmentType.Right;
                                        }

                                        SetStyleAndValue(worksheet, summaryRowIndex, fieldStartingIndex, fieldValueObject, 14, true, textAlignmentType, true);
                                        if (subTableFieldsCount == 1)
                                            fieldStartingIndex++;
                                    }
                                }
                            }
                            columnIndexState += valuesCount - 1;
                        }
                    }
                }
                preColumnIndex = col;
            }
        }
        private void InsertRowsForHeaders(List<int> insertedRows, List<int> insertedCols, Worksheet worksheet, int headerRowIndex, int subTableDataRowIndex, int maxSubTableHeaders, int maxHeaderRows)
        {
            if (maxHeaderRows != 0)
            {
                int startingIndex = headerRowIndex - maxHeaderRows + 1;
                while (startingIndex < headerRowIndex)
                {
                    if (!insertedRows.Contains(startingIndex + 1))
                    {
                        worksheet.Cells.InsertRow(startingIndex + 1);
                        insertedRows.Add(startingIndex + 1);
                    }
                    startingIndex++;
                }
            }
            else
            {
                return;
            }
        }
        private void MergeCells(Worksheet worksheet, int currentSubTableRowIndex, int subTableFirstRowIndex, FieldValueRange fieldValueRange, List<FieldValueRange> currentRanges, List<FieldValueRange> parentRanges)
        {
            if (fieldValueRange.StartIndex != fieldValueRange.EndIndex)
            {
                if (currentSubTableRowIndex == subTableFirstRowIndex && parentRanges.Count == 0)
                {
                    worksheet.Cells.Merge(currentSubTableRowIndex, fieldValueRange.StartIndex, 1, fieldValueRange.EndIndex - fieldValueRange.StartIndex + 1);
                    currentRanges.Add(new FieldValueRange()
                    {
                        StartIndex = fieldValueRange.StartIndex,
                        EndIndex = fieldValueRange.EndIndex
                    });
                }
                else if (parentRanges.Count > 0)
                {
                    if (!parentRanges.Any(x => x.StartIndex <= fieldValueRange.EndIndex && x.EndIndex >= fieldValueRange.StartIndex))
                    {
                        return;
                    }

                    if (parentRanges.Any(x => x.StartIndex <= fieldValueRange.StartIndex && x.EndIndex >= fieldValueRange.EndIndex))
                    {
                        worksheet.Cells.Merge(currentSubTableRowIndex, fieldValueRange.StartIndex, 1, fieldValueRange.EndIndex - fieldValueRange.StartIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = fieldValueRange.StartIndex,
                            EndIndex = fieldValueRange.EndIndex
                        });
                    }

                    var parentLeftSide = parentRanges.FindRecord(x => x.StartIndex >= fieldValueRange.StartIndex && x.EndIndex >= fieldValueRange.EndIndex && fieldValueRange.EndIndex > x.StartIndex);
                    if (parentLeftSide != null)
                    {
                        worksheet.Cells.Merge(currentSubTableRowIndex, parentLeftSide.StartIndex, 1, fieldValueRange.EndIndex - parentLeftSide.StartIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = parentLeftSide.StartIndex,
                            EndIndex = fieldValueRange.EndIndex
                        });
                    }


                    var parentRightSide = parentRanges.FindRecord(x => x.StartIndex <= fieldValueRange.StartIndex && x.EndIndex <= fieldValueRange.EndIndex && fieldValueRange.StartIndex < x.EndIndex);
                    if (parentRightSide != null)
                    {
                        worksheet.Cells.Merge(currentSubTableRowIndex, fieldValueRange.StartIndex, 1, parentRightSide.EndIndex - fieldValueRange.StartIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = fieldValueRange.StartIndex,
                            EndIndex = parentRightSide.EndIndex
                        });
                    }

                    var parents = parentRanges.FindAllRecords(x => x.StartIndex > fieldValueRange.StartIndex && x.EndIndex < fieldValueRange.EndIndex);
                    if (parents != null)
                    {
                        foreach (var parentItem in parents)
                        {
                            worksheet.Cells.Merge(currentSubTableRowIndex, parentItem.StartIndex, 1, parentItem.EndIndex - parentItem.StartIndex + 1);
                            currentRanges.Add(new FieldValueRange()
                            {
                                StartIndex = parentItem.StartIndex,
                                EndIndex = parentItem.EndIndex
                            });
                        }
                    }

                }
            }
        }
        private void SetFlagValues(ref object previousValue, object newValue, FieldValueRange fieldValueRange, int newIndex)
        {
            previousValue = newValue;
            fieldValueRange.StartIndex = newIndex;
            fieldValueRange.EndIndex = newIndex;
        }
        private void SetStyleValueAndBordersForTitleCell(Worksheet worksheet, int subTableTitleRowIndex, int subTableTitleColIndex,string subTableTitle, int subTableValuesCount)
        {
            if (subTableValuesCount > 1)
            {
                int count = 0;
                while (count < subTableValuesCount)
                {
                    worksheet.Cells[subTableTitleRowIndex, subTableTitleColIndex + count].PutValue(subTableTitle);
                    Cell cell = worksheet.Cells.GetCell(subTableTitleRowIndex, subTableTitleColIndex + count);
                    Style style = cell.GetStyle();
                    if (count == 0)
                    {
                        style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                        style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    }
                    else if (count == subTableValuesCount - 1)
                    {
                        style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                        style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    }
                    else
                    {
                        style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    }
                    style.Font.Name = "Times New Roman";
                    style.Font.Size = 15;
                    style.Font.Color = Color.Black;
                    style.Font.IsBold = true;
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    cell.SetStyle(style);
                    count++;
                }
            }
            else if(subTableValuesCount==1)
            {
                worksheet.Cells[subTableTitleRowIndex, subTableTitleColIndex].PutValue(subTableTitle);
                Cell cell = worksheet.Cells.GetCell(subTableTitleRowIndex, subTableTitleColIndex);
                Style style = cell.GetStyle();
                style.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                style.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                style.Font.Name = "Times New Roman";
                style.Font.Size = 15;
                style.Font.Color = Color.Black;
                style.Font.IsBold = true;
                style.HorizontalAlignment = TextAlignmentType.Center;
                cell.SetStyle(style);
            }
        }
        private void SetStyleAndValue(Worksheet worksheet, int rowIndex, int columnIndex, object value, int fontSize, bool isBold, TextAlignmentType alignment, bool setBorders)
        {
            if (fontSize != 16)
                worksheet.Cells.SetColumnWidth(columnIndex, 20);
            worksheet.Cells[rowIndex, columnIndex].PutValue(value);
            Cell cell = worksheet.Cells.GetCell(rowIndex, columnIndex);
            Style style = cell.GetStyle();
            style.Font.Name = "Times New Roman";
            style.Font.Color = Color.Black;
            style.Font.Size = fontSize;
            style.Font.IsBold = isBold;
            style.HorizontalAlignment = alignment;
            if (setBorders)
            {
                SetBorder(style, true, true, true, true);
            }
            cell.SetStyle(style);
        }
        private void SetBorder(Style style, bool setTopBorder, bool setRightBorder, bool setBottomBorder, bool setLeftBorder)
        {
            if (setTopBorder)
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

                        if (tableDefinition.ColumnDefinitions != null && tableDefinition.ColumnDefinitions.Count > 0)
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
                                if (subTable.SubTableFields != null && subTable.SubTableFields.Count > 0)
                                {
                                    foreach (var field in subTable.SubTableFields)
                                    {
                                        if (!matchingSubTable.Measures.Any(x => x == field.FieldName))
                                        {
                                            if (!missingTableFields.Contains(field.FieldName))
                                                missingTableFields.Add(field.FieldName);
                                        }
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
        private void InsertRow(List<int> insertedRows, int rowIndex, Worksheet worksheet)
        {
            if (!insertedRows.Contains(rowIndex + 1))
            {
                worksheet.Cells.InsertRow(rowIndex + 1);
                insertedRows.Add(rowIndex + 1);
            }
        }
        private void InsertColumn(List<int> insertedColumns, int colIndex, Worksheet worksheet)
        {
            if (!insertedColumns.Contains(colIndex + 1))
            {
                worksheet.Cells.InsertColumn(colIndex + 1);
                insertedColumns.Add(colIndex + 1);
            }
        }
        private void SetColumnValue(Worksheet worksheet, List<int> insertedCols, int subTableFieldsCount, int currentSubTableRowIndex, int subTableFirstRowIndex, List<FieldValueRange> parentRanges, ref int fieldStartingIndex, List<FieldValueRange> currentRanges,ref object previousValue, FieldValueRange fieldValueRange, int iteration, ref int currentSubTableField, Object fieldValue, int fieldValuesCount)
        {
            while (currentSubTableField < subTableFieldsCount)
            {
                if (!(iteration == fieldValuesCount - 1 && currentSubTableField == subTableFieldsCount - 1))//this serves for the column insertion not to be executed at the last column
                {
                    InsertColumn(insertedCols, fieldStartingIndex, worksheet);
                }
                SetStyleAndValue(worksheet, currentSubTableRowIndex , fieldStartingIndex, fieldValue, 14, true, TextAlignmentType.Center, true);
                CheckAndSetValue(worksheet, fieldValue, currentSubTableRowIndex, subTableFirstRowIndex, currentRanges, parentRanges, fieldStartingIndex, iteration, ref previousValue, fieldValueRange, currentSubTableField);
                currentSubTableField++;
                fieldStartingIndex++;
            }
        }
        private void CheckAndSetValue(Worksheet worksheet, Object value, int currentSubTableRowIndex, int subTableFirstRowIndex, List<FieldValueRange> currentRanges, List<FieldValueRange> parentRanges, int fieldStartingIndex, int iteration, ref object previousValue, FieldValueRange fieldValueRange, int currentSubTableField)
        {
            if (iteration > 0 || currentSubTableField > 0)
            {
                if (previousValue != null && !previousValue.Equals(value))
                {
                    MergeCells(worksheet, currentSubTableRowIndex, subTableFirstRowIndex, fieldValueRange, currentRanges, parentRanges);
                    SetFlagValues(ref previousValue, value, fieldValueRange, fieldStartingIndex);
                }
                else if (previousValue == null && value != null)
                {
                    MergeCells(worksheet, currentSubTableRowIndex, subTableFirstRowIndex, fieldValueRange, currentRanges, parentRanges);
                    SetFlagValues(ref previousValue, value, fieldValueRange, fieldStartingIndex);
                }
                else
                {
                    fieldValueRange.EndIndex = fieldStartingIndex;
                }
            }
            else
            {
                SetFlagValues(ref previousValue, value, fieldValueRange, fieldStartingIndex);
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

        public bool IncludeSummary { get; set; }

        public List<string> Titles { get; set; }

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

        public string SubTableTitle { get; set; }

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
    public class FieldValueRange
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

    }
    public class IndexesNeeded
    {
        public int SubTableDataRowIndex { get; set; }
        public int ColDataRowIndex { get; set; }
        public int HeaderRowIndex { get; set; }
        public int MaxHeaderRows { get; set; }// this refers to the maximum number of rows used in a heading of all subtables including field titles' row and excluding subtable titles
        public int MaxSubTableHeaders { get; set; }// this refers to the maximum number of headers used in all subtables
    }
    public class VRReportTableInfo
    {
        public VRAutomatedReportTableInfo ReportTableInfo { get; set; }
        public AdvancedExcelFileGeneratorSubTableDefinition TableDefinition { get; set; }
    }
    public class DefinitionsInfo
    {
        public Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> ColumnDefinitionsDic { get; set; }
        public Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> SubTableDefinitionsDic { get; set; }
        public IOrderedEnumerable<int> SortedColumnsIndexes { get; set; }//this is the list of the user mapped column indices excluding the ones added programmatically
        public Dictionary<string, VRAutomatedReportFieldInfo> FieldInfo { get; set; }
        public Dictionary<Guid, VRAutomatedReportTableInfo> SubTablesInfo { get; set; }
    }


    public interface ITableDefinitionInfoContext
    {
        AdvancedExcelFileGeneratorSubTableDefinition GetSubTableDefinitionByColIndex(int colIndex);
        AdvancedExcelFileGeneratorTableColumnDefinition GetColumnDefinitionByColIndex(int colIndex);
        List<VRReportTableInfo> GetSubTablesInfo();
        IOrderedEnumerable<int> GetSortedColumnsIndexes();
        VRAutomatedReportTableInfo GetSubTableInfo(Guid tableId);
        VRAutomatedReportFieldInfo GetFieldInfo(string fieldName);
    }

    public class TableDefinitionInfoContext : ITableDefinitionInfoContext
    {
        Dictionary<Guid, VRAutomatedReportTableInfo> _subTablesInfo { get; set; }
        List<VRReportTableInfo> _presentSubTablesInfo { get; set; }
        Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> _subTableDefinitionsDic { get; set; }
        List<AdvancedExcelFileGeneratorSubTableDefinition> _subTableDefinitions { get; set; }
        Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> _columnDefinitionsDic { get; set; }
        List<AdvancedExcelFileGeneratorTableColumnDefinition> _columnDefinitions { get; set; }
        IOrderedEnumerable<int> _sortedColumnsIndexes { get; set; }
        Dictionary<string, VRAutomatedReportFieldInfo> _fieldInfos { get; set; }

        public TableDefinitionInfoContext(List<AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitions, List<AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitions, Dictionary<Guid, VRAutomatedReportTableInfo> subTablesInfo, Dictionary<string, VRAutomatedReportFieldInfo> fieldInfos)
        {
             _subTablesInfo = subTablesInfo;
            _subTableDefinitions = subTableDefinitions;
            _columnDefinitions = columnDefinitions;
            _fieldInfos = fieldInfos;
        }
        public  List<VRReportTableInfo> GetSubTablesInfo()
        {
            if (_presentSubTablesInfo == null)
            {
                if (_subTablesInfo != null && _subTablesInfo.Count > 0 && _subTableDefinitions != null && _subTableDefinitions.Count > 0)
                {
                    _presentSubTablesInfo = new List<VRReportTableInfo>();
                    foreach (var subTableDef in _subTableDefinitions)
                    {
                        _presentSubTablesInfo.Add(new VRReportTableInfo
                            {
                                ReportTableInfo = _subTablesInfo.GetRecord(subTableDef.SubTableId),
                                TableDefinition = subTableDef
                            });
                    }
                }
            }
            return _presentSubTablesInfo;
        }
        public AdvancedExcelFileGeneratorTableColumnDefinition GetColumnDefinitionByColIndex(int colIndex)
        {
            if (_columnDefinitionsDic == null)
            {
                _columnDefinitionsDic = new Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition>();
                if (_columnDefinitions != null && _columnDefinitions.Count > 0)
                {
                    foreach (var colDef in _columnDefinitions)
                    {
                        _columnDefinitionsDic.Add(colDef.ColumnIndex, colDef);
                    }
                }
            }
            return _columnDefinitionsDic.GetRecord(colIndex);
        }
        public AdvancedExcelFileGeneratorSubTableDefinition GetSubTableDefinitionByColIndex(int colIndex)
        {
            if (_subTableDefinitionsDic == null)
            {
                _subTableDefinitionsDic = new Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition>();
                if (_subTableDefinitions != null && _subTableDefinitions.Count > 0)
                {
                    foreach (var subTableDef in _subTableDefinitions)
                    {
                        _subTableDefinitionsDic.Add(subTableDef.ColumnIndex, subTableDef);
                    }
                }
            }
            return _subTableDefinitionsDic.GetRecord(colIndex);
        }
        public IOrderedEnumerable<int> GetSortedColumnsIndexes()
        {
            if(_sortedColumnsIndexes == null)
            {
                List<int> columnsIndexes = new List<int>();
                if (_columnDefinitions != null && _columnDefinitions.Count > 0)
                {
                    foreach (var colDef in _columnDefinitions)
                    {
                        columnsIndexes.Add(colDef.ColumnIndex);
                    }
                }
                if (_subTableDefinitions != null && _subTableDefinitions.Count > 0)
                {
                    foreach (var subTableDef in _subTableDefinitions)
                    {
                        columnsIndexes.Add(subTableDef.ColumnIndex);
                    }
                }
                _sortedColumnsIndexes = columnsIndexes.OrderBy(x => x);
            }
            return _sortedColumnsIndexes;
        }

        public VRAutomatedReportFieldInfo GetFieldInfo(string fieldName)
        {
            return _fieldInfos.GetRecord(fieldName);
        }
        public VRAutomatedReportTableInfo GetSubTableInfo(Guid tableId)
        {
            return _subTablesInfo.GetRecord(tableId);
        }
    }
}
