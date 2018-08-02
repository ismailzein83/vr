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

        private void EvaluateStartingRows(bool includeHeaders, bool includeTitle, Dictionary<Guid, VRAutomatedReportTableInfo> subTablesInfo, List<AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitions, ref int subTableDataRowIndex, ref int colDataRowIndex, ref int headerRowIndex, out int maxHeaderRows, out int maxSubTableDimensions)
        {
            Dictionary<Guid, VRAutomatedReportTableInfo> presentSubTablesInfo = new Dictionary<Guid, VRAutomatedReportTableInfo>();
            if(subTablesInfo!=null && subTablesInfo.Count > 0 && subTableDefinitions != null && subTableDefinitions.Count > 0)
            {
                foreach (var subTableDef in subTableDefinitions)
                {
                    presentSubTablesInfo.Add(subTableDef.SubTableId, subTablesInfo.GetRecord(subTableDef.SubTableId));
                }
            }
            if (includeHeaders)
            {
                maxHeaderRows = presentSubTablesInfo.Max(x => x.Value.FieldsOrder.Count());
                maxSubTableDimensions = maxHeaderRows;
                headerRowIndex += maxHeaderRows - 1;
                colDataRowIndex = headerRowIndex + 1;
            }
            else
            {
                maxSubTableDimensions = 0;
                maxHeaderRows = 0;
            }
            if (presentSubTablesInfo.Count > 0)
            {
                int maximumDimensionCount = maxHeaderRows;
                var subTableInfoWithMostDimensions = presentSubTablesInfo.FirstOrDefault(x => x.Value.FieldsOrder.Count() == maximumDimensionCount);
                var subTableDefWithMostDimensions = subTableDefinitions.FindRecord(x => x.SubTableId == subTableInfoWithMostDimensions.Key);
                bool doAllTablesHaveSameDimensionCount = presentSubTablesInfo.All(x => x.Value.FieldsOrder.Count == maximumDimensionCount);
                if (subTableDefWithMostDimensions != null && subTableDefWithMostDimensions.SubTableTitle != null)
                {
                    if((presentSubTablesInfo.Count > 1 && !doAllTablesHaveSameDimensionCount )|| presentSubTablesInfo.Count==1)
                    {
                        subTableDataRowIndex++;
                        maxHeaderRows++;
                        headerRowIndex++;
                        colDataRowIndex++;
                    }
                }
            }
            if (includeTitle)
            {
                headerRowIndex++;
                colDataRowIndex++;
                subTableDataRowIndex++;
            }
        }
        private void BuildColumnsAndSubTablesDefinitionsDic(List<AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitions, List<AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitions,out Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitionsDic,out Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitionsDic,out List<int> columnsIndexes)
        {
            columnsIndexes = new List<int>();
            columnDefinitionsDic = new Dictionary<int,AdvancedExcelFileGeneratorTableColumnDefinition>();
            subTableDefinitionsDic = new Dictionary<int,AdvancedExcelFileGeneratorSubTableDefinition>();
            if (columnDefinitions != null && columnDefinitions.Count > 0)
            {
                foreach (var colDef in columnDefinitions)
                {
                    columnsIndexes.Add(colDef.ColumnIndex);
                    columnDefinitionsDic.Add(colDef.ColumnIndex, colDef);
                }
            }
            if (subTableDefinitions != null && subTableDefinitions.Count > 0)
            {
                foreach (var subTableDef in subTableDefinitions)
                {
                    columnsIndexes.Add(subTableDef.ColumnIndex);
                    subTableDefinitionsDic.Add(subTableDef.ColumnIndex, subTableDef);
                }
            }
        }
        private void MergeCells(Worksheet worksheet, int subTableFirstRowIndex, int subTableDataRowIndex, int startIndex, int endIndex, List<FieldValueRange> currentRanges, List<FieldValueRange> parentRanges)
        {
            if (startIndex != endIndex)
            {
                if (subTableFirstRowIndex == subTableDataRowIndex && parentRanges.Count == 0)
                {
                    worksheet.Cells.Merge(subTableFirstRowIndex, startIndex, 1, endIndex - startIndex + 1);
                    currentRanges.Add(new FieldValueRange()
                    {
                        StartIndex = startIndex,
                        EndIndex = endIndex
                    });
                }
                else if (parentRanges.Count > 0)
                {
                    if (!parentRanges.Any(x => x.StartIndex <= endIndex && x.EndIndex >= startIndex))
                    {
                        return;
                    }

                    if (parentRanges.Any(x => x.StartIndex <= startIndex && x.EndIndex >= endIndex))
                    {
                        worksheet.Cells.Merge(subTableFirstRowIndex, startIndex, 1, endIndex - startIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = startIndex,
                            EndIndex = endIndex
                        });
                    }

                    var parentLeftSide = parentRanges.FindRecord(x => x.StartIndex >= startIndex && x.EndIndex >= endIndex && endIndex > x.StartIndex);
                    if (parentLeftSide != null)
                    {
                        worksheet.Cells.Merge(subTableFirstRowIndex, parentLeftSide.StartIndex, 1, endIndex - parentLeftSide.StartIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = parentLeftSide.StartIndex,
                            EndIndex = endIndex
                        });
                    }


                    var parentRightSide = parentRanges.FindRecord(x => x.StartIndex <= startIndex && x.EndIndex <= endIndex && startIndex < x.EndIndex);
                    if (parentRightSide != null)
                    {
                        worksheet.Cells.Merge(subTableFirstRowIndex, startIndex, 1, parentRightSide.EndIndex - startIndex + 1);
                        currentRanges.Add(new FieldValueRange()
                        {
                            StartIndex = startIndex,
                            EndIndex = parentRightSide.EndIndex
                        });
                    }

                    var parents = parentRanges.FindAllRecords(x => x.StartIndex > startIndex && x.EndIndex < endIndex);
                    if (parents != null)
                    {
                        foreach (var parentItem in parents)
                        {
                            worksheet.Cells.Merge(subTableFirstRowIndex, parentItem.StartIndex, 1, parentItem.EndIndex - parentItem.StartIndex + 1);
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


        private void SetFlagValues(ref object previousValue, object newValue, ref int startIndex, int newStartIndex, ref int endIndex, int newEndIndex)
        {
            previousValue = newValue;
            startIndex = newStartIndex;
            endIndex = newEndIndex;
        }

        private void CheckAndSetNumericalOrDateTimeValues(Worksheet worksheet, VRAutomatedReportResolvedDataFieldValue fieldValue, int subTableFirstRowIndex, int adjustedSubTableFirstRowIndex, List<FieldValueRange> currentRanges, List<FieldValueRange> parentRanges, int fieldStartingIndex, int iteration, ref object previousValue, ref int startIndex, ref int endIndex)
        {
            if (iteration > 0)
            {
                if (previousValue == null)
                {
                    MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                    SetFlagValues(ref previousValue, fieldValue.Value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                }
                else
                {
                    if (!previousValue.Equals(fieldValue.Value))
                    {
                        MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                        SetFlagValues(ref previousValue, fieldValue.Value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                    }
                    else
                    {
                        endIndex = fieldStartingIndex;
                    }
                }
            }
            else
            {
                SetFlagValues(ref previousValue, fieldValue.Value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
            }
        }

        private void CheckAndSetStringValues(Worksheet worksheet, string value, int subTableFirstRowIndex, int adjustedSubTableFirstRowIndex, List<FieldValueRange> currentRanges, List<FieldValueRange> parentRanges, int fieldStartingIndex, int iteration, ref object previousValue, ref int startIndex, ref int endIndex)
        {
            if (iteration > 0)
            {
                if (previousValue == null)
                {
                    if (value != null)
                    {
                        MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                        SetFlagValues(ref previousValue, value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                    }
                    else
                    {
                        endIndex = fieldStartingIndex;
                    }
                }
                else
                {
                    if (!previousValue.ToString().Equals(value))
                    {
                        MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                        SetFlagValues(ref previousValue, value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                    }
                    else
                    {
                        endIndex = fieldStartingIndex;
                    }
                }
            }
            else
            {
                SetFlagValues(ref previousValue, value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
            }
        }
        //private void SetBordersForTitleCell(Worksheet worksheet, int subTableTitleRowIndex, int subTableTitleColIndex, int subTableValuesCount)
        //{
        //    int count = 0;
        //    while (count < subTableValuesCount)
        //    {
        //        Cell cell = worksheet.Cells.GetCell(subTableTitleRowIndex, subTableTitleColIndex+count);
        //        Style style = cell.GetStyle();
        //        if (count == 0)
        //        {
        //            style.SetBorder(BorderType.LeftBorder, CellBorderType.Medium, Color.Black);
        //            style.SetBorder(BorderType.TopBorder, CellBorderType.Medium, Color.Black);
        //        }
        //        else if (count == subTableValuesCount-1)
        //        {
        //            style.SetBorder(BorderType.LeftBorder, CellBorderType.Medium, Color.Black);
        //            style.SetBorder(BorderType.TopBorder, CellBorderType.Medium, Color.Black);
        //        }
        //        else
        //        {
        //            style.SetBorder(BorderType.TopBorder, CellBorderType.Medium, Color.Black);
        //        }
        //        cell.SetStyle(style);
        //        count++;
        //    }
   
        //}
        private void BuildSubTableTitle(Worksheet worksheet,  string subTableTitle, int subTableTitleRowIndex, int subTableTitleColIndex, int subTableValuesCount)
        {
            if (subTableTitle != null)
            {
                SetStyleAndValue(worksheet, subTableTitleRowIndex - 1, subTableTitleColIndex, subTableTitle, 14, true, TextAlignmentType.Center, false);
                worksheet.Cells.Merge(subTableTitleRowIndex - 1, subTableTitleColIndex, 1, subTableValuesCount);
                //SetBordersForTitleCell(worksheet, subTableTitleRowIndex - 1, subTableTitleColIndex, subTableValuesCount);
            }
        }

        private void BuildTableHeaders(Worksheet worksheet, int headerRowIndex, ref int subTableDataRowIndex, IOrderedEnumerable<int> sortedColumnsIndexes, Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitionsDic, Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitionsDic, Dictionary<string, VRAutomatedReportFieldInfo> fieldInfos, Dictionary<Guid, VRAutomatedReportTableInfo> subTablesInfo, out int subTableValuesCount, int maxHeaderRows, int maxSubTableDimensions)
        {
            int columnIndexState = sortedColumnsIndexes.First();
            List<int> subTableRows = new List<int>();
            subTableValuesCount = 0;
            foreach (var col in sortedColumnsIndexes)
            {
                if (col > columnIndexState)
                    columnIndexState = col;
                var column = columnDefinitionsDic.GetRecord(col);
                if (column != null)
                {
                    if (column.FieldTitle != null)
                    {
                        SetStyleAndValue(worksheet, headerRowIndex, columnIndexState, column.FieldTitle, 14, true, TextAlignmentType.Center, true);
                    }
                    else
                    {
                        var fieldInfo = fieldInfos.GetRecord(column.FieldName);
                        if (fieldInfo != null)
                        {
                            SetStyleAndValue(worksheet, headerRowIndex, columnIndexState, fieldInfo.FieldTitle, 14, true, TextAlignmentType.Center, true);
                        }
                    }
                }
                else
                {
                    var subTableDef = subTableDefinitionsDic.GetRecord(col);
                    if (subTableDef != null)
                    {
                        var subTableInfo = subTablesInfo.GetRecord(subTableDef.SubTableId);
                        int subTableFirstRowIndex = subTableDataRowIndex;
                        int subTableTitleRowIndex = subTableFirstRowIndex;
                        int subTableTitleColIndex = columnIndexState;

                        int adjustedSubTableFirstRowIndex = 0;
                        if (subTableInfo != null && subTableInfo.FieldsInfo!=null && subTableInfo.FieldsInfo.Count>0 && subTableInfo.FieldsOrder != null && subTableInfo.FieldsOrder.Count>0)
                        {
                            subTableRows.Add(subTableInfo.FieldsOrder.Count);
                            if (maxSubTableDimensions > subTableInfo.FieldsOrder.Count)
                            {
                                subTableFirstRowIndex += (maxSubTableDimensions - subTableInfo.FieldsOrder.Count);
                                subTableTitleRowIndex = subTableFirstRowIndex;
                            }
                            adjustedSubTableFirstRowIndex = subTableFirstRowIndex;
                            int valuesCount = 0;
                            List<FieldValueRange> parentRanges = new List<FieldValueRange>();

                            foreach(var field in subTableInfo.FieldsOrder)
                            {
                                var fieldStartingIndex = columnIndexState;
                                var fieldValues = subTableInfo.FieldsInfo[field];
                                List<FieldValueRange> currentRanges = new List<FieldValueRange>();
                                if (fieldValues != null && fieldValues.FieldValues != null && fieldValues.FieldValues.Count > 0 && fieldValues.FieldType != null)
                                {
                                    valuesCount = fieldValues.FieldValues.Count;
                                    subTableValuesCount = valuesCount;
                                    object previousValue = null;
                                    int startIndex = -1;
                                    int endIndex = -1;
                                    for (int iteration = 0; iteration < fieldValues.FieldValues.Count; iteration++)
                                    {
                                        var fieldValue = fieldValues.FieldValues[iteration];
                                        if (fieldValue != null)
                                        {
                                            if (fieldValues.FieldType.RenderDescriptionByDefault())
                                            {
                                                SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, fieldValue.Description, 14, true, TextAlignmentType.Center, true);
                                                CheckAndSetStringValues(worksheet, fieldValue.Description, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, currentRanges, parentRanges, fieldStartingIndex, iteration, ref previousValue, ref startIndex, ref endIndex);
                                                fieldStartingIndex++;
                                            }
                                            else
                                            {
                                                if (fieldValue.Value is DateTime)
                                                {
                                                    var date = Convert.ToDateTime(fieldValue.Value);
                                                    SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, date.ToString(generalSettingsManager.GetDateTimeFormat()), 14, true, TextAlignmentType.Center, true);
                                                    CheckAndSetNumericalOrDateTimeValues(worksheet, fieldValue, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, currentRanges, parentRanges, fieldStartingIndex, iteration, ref previousValue, ref startIndex, ref endIndex);
                                                    fieldStartingIndex++;
                                                }
                                                else if (fieldValue.Value is int || fieldValue.Value is double || fieldValue.Value is decimal || fieldValue.Value is long)
                                                {
                                                    SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, fieldValue.Value, 14, true, TextAlignmentType.Center, true);
                                                    CheckAndSetNumericalOrDateTimeValues(worksheet, fieldValue, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, currentRanges, parentRanges, fieldStartingIndex, iteration, ref previousValue, ref startIndex, ref endIndex);
                                                    fieldStartingIndex++;
                                                }
                                                else if (fieldValue.Value is string)
                                                {
                                                    SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, fieldValue.Value, 14, true, TextAlignmentType.Center, true);
                                                    CheckAndSetStringValues(worksheet, fieldValue.Value.ToString(), subTableFirstRowIndex, adjustedSubTableFirstRowIndex, currentRanges, parentRanges, fieldStartingIndex, iteration, ref previousValue, ref startIndex, ref endIndex);
                                                    fieldStartingIndex++;
                                                }
                                                else
                                                {
                                                    SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, fieldValue.Value, 14, true, TextAlignmentType.Center, true);
                                                    if (iteration > 0)
                                                    {
                                                        if (previousValue != fieldValue.Value)
                                                        {
                                                            MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                                                            SetFlagValues(ref previousValue, fieldValue.Value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                                                        }
                                                        else
                                                        {
                                                            endIndex = fieldStartingIndex;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        SetFlagValues(ref previousValue, fieldValue.Value, ref startIndex, fieldStartingIndex, ref endIndex, fieldStartingIndex);
                                                    }
                                                    fieldStartingIndex++;
                                                }
                                            }
                                        }
                                    }
                                    MergeCells(worksheet, subTableFirstRowIndex, adjustedSubTableFirstRowIndex, startIndex, endIndex, currentRanges, parentRanges);
                                    parentRanges = currentRanges;
                                }
                                subTableFirstRowIndex++;
                            }
                            columnIndexState += valuesCount - 1;
                        }
                       BuildSubTableTitle(worksheet, subTableDef.SubTableTitle, subTableTitleRowIndex, subTableTitleColIndex, subTableValuesCount);
                    }
                }
                columnIndexState++;
            }
            if (subTableRows!=null && subTableRows.Count>0)
            {
                subTableDataRowIndex += subTableRows.Max();
            }
        }

        private void BuildTableSummaryData(Worksheet worksheet, VRAutomatedReportResolvedDataList dataList, int summaryRowIndex, IOrderedEnumerable<int> sortedColumnsIndexes, Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitionsDic, Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitionsDic)
        {
            var fieldInfos = dataList.FieldInfos;
            var subTablesInfo = dataList.SubTablesInfo;
            var summaryRecord = dataList.SummaryDataItem;
            int columnIndexState = sortedColumnsIndexes.First();

            foreach (var col in sortedColumnsIndexes)
            {
                if (col > columnIndexState)
                    columnIndexState = col;
                var column = columnDefinitionsDic.GetRecord(col);
                if (column != null)
                {
                    var summaryField = (summaryRecord != null && summaryRecord.Fields != null) ? summaryRecord.Fields.GetRecord(column.FieldName) : null;
                    var fieldInfo = fieldInfos.GetRecord(column.FieldName);
                    if (summaryField != null && fieldInfo != null && fieldInfo.FieldType!=null)
                    {
                        if (fieldInfo.FieldType.RenderDescriptionByDefault())
                        {
                            SetStyleAndValue(worksheet, summaryRowIndex, columnIndexState, summaryField.Description, 14, true, TextAlignmentType.Left, true);
                        }
                        else
                        {
                            if (summaryField.Value is DateTime)
                            {
                                var summaryDate = Convert.ToDateTime(summaryField.Value);
                                SetStyleAndValue(worksheet, summaryRowIndex, columnIndexState, summaryDate.ToString(generalSettingsManager.GetDateTimeFormat()), 14, true, TextAlignmentType.Right, true);
                            }
                            else if (summaryField.Value is int || summaryField.Value is double || summaryField.Value is decimal || summaryField.Value is long)
                            {
                                SetStyleAndValue(worksheet, summaryRowIndex, columnIndexState, summaryField.Value, 14, true, TextAlignmentType.Right, true);
                            }
                            else
                            {
                                SetStyleAndValue(worksheet, summaryRowIndex, columnIndexState, summaryField.Value, 14, true, TextAlignmentType.Left, true);
                            }
                        }
                    }
                }
                else
                {
                    var subTableDef = subTableDefinitionsDic.GetRecord(col);
                    if (subTableDef != null)
                    {
                        var subTableInfo = subTablesInfo.GetRecord(subTableDef.SubTableId);
                        VRAutomatedReportResolvedDataItemSubTable summarySubTableFields = (summaryRecord != null && summaryRecord.SubTables != null) ? summaryRecord.SubTables.GetRecord(subTableDef.SubTableId) : null;
                        if (summarySubTableFields != null && summarySubTableFields.Fields != null && summarySubTableFields.Fields.Count > 0 && subTableDef.SubTableFields != null && subTableDef.SubTableFields.Count > 0)
                        {
                            int valuesCount = 0;
                            foreach (var measureDef in subTableDef.SubTableFields)
                            {
                                var fieldStartingIndex = columnIndexState;
                                var summaryMeasure = (summarySubTableFields != null && summarySubTableFields.Fields != null) ? summarySubTableFields.Fields.GetRecord(measureDef.FieldName) : null;
                                if (summaryMeasure != null && summaryMeasure.FieldValues != null && summaryMeasure.FieldValues.Count > 0)
                                {
                                    valuesCount = summaryMeasure.FieldValues.Count;
                                    for (int o = 0; o < summaryMeasure.FieldValues.Count; o++)
                                    {
                                        var summaryMeasureValue = (summaryMeasure != null && summaryMeasure.FieldValues != null) ? summaryMeasure.FieldValues[o] : null;
                                        if (summaryMeasureValue.Value is DateTime)
                                        {
                                            var date = Convert.ToDateTime(summaryMeasureValue.Value);
                                            SetStyleAndValue(worksheet, summaryRowIndex, fieldStartingIndex, date.ToString(generalSettingsManager.GetDateTimeFormat()), 14, true, TextAlignmentType.Right, true);
                                            fieldStartingIndex++;
                                            continue;
                                        }
                                        else if (summaryMeasureValue.Value is int || summaryMeasureValue.Value is double || summaryMeasureValue.Value is decimal || summaryMeasureValue.Value is long)
                                        {
                                            SetStyleAndValue(worksheet, summaryRowIndex, fieldStartingIndex, summaryMeasureValue.Value, 14, true, TextAlignmentType.Right, true);
                                            fieldStartingIndex++;
                                            continue;
                                        }
                                        else
                                        {
                                            SetStyleAndValue(worksheet, summaryRowIndex, fieldStartingIndex, summaryMeasureValue.Value, 14, true, TextAlignmentType.Left, true);
                                            fieldStartingIndex++;
                                        }
                                    }
                                }
                            }
                            columnIndexState += valuesCount - 1;
                        }
                    }
                }
                columnIndexState++;
            }
        }

        private void BuildTableData(Worksheet worksheet, List<int> allColumnIndices, VRAutomatedReportResolvedDataList dataList, int subTableDataRowIndex, int colDataRowIndex, IOrderedEnumerable<int> sortedColumnsIndexes, Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitionsDic, Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitionsDic, int subTableValuesCount, out int summaryRowIndex)
        {
            var items = dataList.Items;
            var fieldInfos = dataList.FieldInfos;
            summaryRowIndex = colDataRowIndex + items.Count;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int columnIndexState = sortedColumnsIndexes.First();
                foreach (var col in sortedColumnsIndexes)
                {
                    if (col > columnIndexState)
                        columnIndexState = col;

                    var column = columnDefinitionsDic.GetRecord(col);
                    if (column != null)
                    {
                        allColumnIndices.Add(columnIndexState);
                        var field = item.Fields.GetRecord(column.FieldName);
                        var fieldInfo = fieldInfos.GetRecord(column.FieldName);
                        if (field!=null && fieldInfo != null && fieldInfo.FieldType!=null)
                        {
                            if (fieldInfo.FieldType.RenderDescriptionByDefault())
                            {
                                SetStyleAndValue(worksheet, colDataRowIndex, columnIndexState, field.Description, 12, false, TextAlignmentType.Left, true);
                            }
                            else
                            {
                                if (field.Value is DateTime)
                                {
                                    var date = Convert.ToDateTime(field.Value);
                                    SetStyleAndValue(worksheet, colDataRowIndex, columnIndexState, date.ToString(generalSettingsManager.GetDateTimeFormat()), 12, false, TextAlignmentType.Right, true);
                                }
                                else if (field.Value is int || field.Value is double || field.Value is decimal || field.Value is long)
                                {
                                    SetStyleAndValue(worksheet, colDataRowIndex, columnIndexState, field.Value, 12, false, TextAlignmentType.Right, true);
                                }
                                else
                                {
                                    SetStyleAndValue(worksheet, colDataRowIndex, columnIndexState, field.Value, 12, false, TextAlignmentType.Left, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        var subTableDef = subTableDefinitionsDic.GetRecord(col);
                        if (subTableDef != null)
                        {
                            int subTableFirstRowIndex = subTableDataRowIndex+i;
                            item.SubTables.ThrowIfNull("item.SubTables");
                            var subTableItem = item.SubTables.GetRecord(subTableDef.SubTableId);
                            if (subTableItem != null && subTableItem.Fields != null && subTableItem.Fields.Count > 0 && subTableDef.SubTableFields != null && subTableDef.SubTableFields.Count > 0)
                            {
                                int valuesCount = 0;
                                foreach (var measureDef in subTableDef.SubTableFields)
                                {
                                    var fieldStartingIndex = columnIndexState;
                                    var measure = subTableItem.Fields.GetRecord(measureDef.FieldName);
                                    if (measure != null && measure.FieldValues != null && measure.FieldValues.Count > 0)
                                    {
                                        valuesCount = measure.FieldValues.Count;
                                        for(int o=0; o<measure.FieldValues.Count; o++)
                                        {
                                            var measureValue = measure.FieldValues[o];
                                            allColumnIndices.Add(fieldStartingIndex);
                                            if (measureValue.Value is DateTime)
                                            {
                                                var date = Convert.ToDateTime(measureValue.Value);
                                                SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, date.ToString(generalSettingsManager.GetDateTimeFormat()), 12, false, TextAlignmentType.Right, true);
                                                fieldStartingIndex++;
                                                continue;
                                            }
                                            else if (measureValue.Value is int || measureValue.Value is double || measureValue.Value is decimal || measureValue.Value is long)
                                            {
                                                SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, measureValue.Value, 12, false, TextAlignmentType.Right, true);
                                                fieldStartingIndex++;
                                                continue;
                                            }
                                            else
                                            {
                                                SetStyleAndValue(worksheet, subTableFirstRowIndex, fieldStartingIndex, measureValue.Value, 12, false, TextAlignmentType.Left, true);
                                                fieldStartingIndex++;
                                            }
                                        }
                                        subTableFirstRowIndex++;
                                    }
                                }
                                columnIndexState += valuesCount - 1;
                            }
                            else
                            {
                                int valuesCount = 0;
                                foreach (var measureDef in subTableDef.SubTableFields)
                                {
                                    var fieldStartingIndex = columnIndexState;
                                    valuesCount = subTableValuesCount;
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
                    columnIndexState++;
                }
                colDataRowIndex++;
            }
        }
        private void BuildTableExcelTableDefinition(AdvancedExcelFileGeneratorTableDefinition tableDef, Workbook tableDefinitionsWorkbook, VRAutomatedReportResolvedDataList dataList)
        {
            Worksheet worksheet = tableDefinitionsWorkbook.Worksheets[tableDef.SheetIndex];
            int titleRowIndex = tableDef.RowIndex;
            int headerRowIndex = tableDef.RowIndex;
            int colDataRowIndex = tableDef.RowIndex;
            int subTableDataRowIndex = tableDef.RowIndex;
            int maxHeaderRows = 0;
            int maxSubTableDimensions = 0;
            EvaluateStartingRows(tableDef.IncludeHeaders, tableDef.IncludeTitle, dataList.SubTablesInfo, tableDef.SubTableDefinitions, ref subTableDataRowIndex, ref colDataRowIndex, ref headerRowIndex, out maxHeaderRows, out maxSubTableDimensions);
            List<int> allColumnIndices = new List<int>();
            List<int> columnsIndexes;
            Dictionary<int, AdvancedExcelFileGeneratorTableColumnDefinition> columnDefinitionsDic;
            Dictionary<int, AdvancedExcelFileGeneratorSubTableDefinition> subTableDefinitionsDic;
            BuildColumnsAndSubTablesDefinitionsDic(tableDef.ColumnDefinitions, tableDef.SubTableDefinitions, out columnDefinitionsDic, out subTableDefinitionsDic, out columnsIndexes);
            
            IOrderedEnumerable<int> sortedColumnsIndexes = columnsIndexes.OrderBy(x => x);
            int subTableValuesCount=0;
            int summaryRowIndex;
            if(tableDef.IncludeHeaders)
            {
                BuildTableHeaders(worksheet, headerRowIndex, ref subTableDataRowIndex, sortedColumnsIndexes, columnDefinitionsDic, subTableDefinitionsDic, dataList.FieldInfos, dataList.SubTablesInfo, out subTableValuesCount, maxHeaderRows, maxSubTableDimensions);
            }

            if (dataList.Items.Count > 0)
            {
                BuildTableData(worksheet, allColumnIndices, dataList, subTableDataRowIndex, colDataRowIndex, sortedColumnsIndexes, columnDefinitionsDic, subTableDefinitionsDic, subTableValuesCount, out summaryRowIndex);
                if(tableDef.IncludeSummary && dataList.SummaryDataItem!=null)
                {
                    BuildTableSummaryData(worksheet, dataList, summaryRowIndex, sortedColumnsIndexes, columnDefinitionsDic, subTableDefinitionsDic);
                }
            }
            else
            {
                if (tableDef.IncludeHeaders)
                {
                   foreach (var col in tableDef.ColumnDefinitions)
                   {
                        SetStyleAndValue(worksheet, headerRowIndex, col.ColumnIndex, col.FieldTitle, 14, true, TextAlignmentType.Left, true);
                   }
                }
            }
            if (tableDef.IncludeTitle && tableDef.Title != null && allColumnIndices.Count>0)
            {
                var orderedIndices = allColumnIndices.Distinct().OrderBy(x => x);
                int difference = 1;
                var titleColumnIndex = orderedIndices.First();
                if (orderedIndices.Count() > 1)
                {
                    difference = orderedIndices.Last() - titleColumnIndex + 1;
                }
                worksheet.Cells.Merge(titleRowIndex, titleColumnIndex, 1, difference);
                SetStyleAndValue(worksheet, titleRowIndex, titleColumnIndex, tableDef.Title, 16, true, TextAlignmentType.Center, false);
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
                                if (subTable.SubTableFields != null && subTable.SubTableFields.Count>0)
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

        private void SetStyleAndValue(Worksheet worksheet, int rowIndex, int columnIndex, object value, int fontSize, bool isBold, TextAlignmentType alignment, bool setBorders)
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
            if (setBorders)
            {
                SetBorder(style, true, true, true, true);
            }
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

        public bool IncludeSummary { get; set; }

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

}
