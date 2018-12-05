﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


--[common].[VRComponentType]------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('A5E581F3-5F22-401E-8184-402705CAE501','CRAN Monthly Report Template','92B03E4E-37FB-48A1-ACEE-841135C30B5E','{"$type":"Vanrise.Analytic.Business.ReportGenerationCustomCodeSettings, Vanrise.Analytic.Business","VRComponentTypeConfigId":"92b03e4e-37fb-48a1-acee-841135c30b5e","CustomCode":"string zoneDimension = \"Zone\";\n                string costNetMeasure = \"CostNet\";\n                string costDurationMeasure = \"CostDuration\";\n                string saleNetMeasure = \"SaleNet\";\n                string saleDurationMeasure = \"SaleDuration\";\n\n                var vrExcelFile = context.CreateExcelFile();\n                var sheet = vrExcelFile.CreateSheet();\n\n                var incomingQueryItem1 = context.GetDataList(\"IncomingQuery1\", \"Main\");\n                string incomingDataListIdentifier1 = string.Format(\"{0}_{1}\", \"IncomingQuery1\", \"Main\");\n                incomingQueryItem1.ThrowIfNull(\"dataList\", incomingDataListIdentifier1);\n                incomingQueryItem1.Items.ThrowIfNull(\"dataList.Items\", incomingDataListIdentifier1);\n                incomingQueryItem1.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", incomingDataListIdentifier1);\n\n                #region Incoming\n                int rowIndexState = 0;\n                var incomingTable = sheet.CreateTable(rowIndexState, 0);\n                sheet.SetColumnConfig(new VRExcelColumnConfig\n                {\n                    ColumnIndex = 0,\n                    ColumnWidth = 20,\n                });\n\n\n                var incomingHeaderRow = incomingTable.CreateHeaderRow();\n                var incomingHeaderCell = incomingHeaderRow.CreateCell();\n                incomingHeaderCell.SetValue(\"Incoming\");\n                context.SetCellStyle(incomingHeaderCell, \"White\", \"Black\", 11, true, VRExcelContainerHorizontalAlignment.Left);\n\n                rowIndexState += 2;\n\n                var incomingHeaderTable = sheet.CreateTable(rowIndexState, 0);\n                incomingHeaderTable.EnableTableBorders();\n                incomingHeaderTable.EnableMergeHeaders();\n                var incomingHeadersRow = incomingHeaderTable.CreateHeaderRow();\n                var incomingZoneHeaderCell = incomingHeadersRow.CreateCell();\n                incomingZoneHeaderCell.SetValue(zoneDimension);\n                context.SetCellStyle(incomingZoneHeaderCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var currentDate = new DateTime(incomingQueryItem1.From.Year, incomingQueryItem1.From.Month, 1);\n\n                VRExcelTableHeaderRow currentRow = incomingHeadersRow;\n                VRExcelTableDataRow currentMeasureRow = incomingHeaderTable.CreateDataRow();\n                currentMeasureRow.CreateCell();\n                int columnIndexState = 8;\n                var incomingHeaders = new NamibiaHeaders(currentDate, incomingQueryItem1.To, context, columnIndexState, rowIndexState, sheet, currentRow, currentMeasureRow);\n                var incomingHeadersTitles = incomingHeaders.SetHeaders();\n\n                #region FirstItem\n\n                rowIndexState += 3;\n\n                var firstItemTable = sheet.CreateTable(rowIndexState, 0);\n                firstItemTable.EnableTableBorders();\n                var firstItemRow = firstItemTable.CreateDataRow();\n                var firstItemCell = firstItemRow.CreateCell();\n                firstItemCell.SetValue(\"Namibia (Domestic)\");\n                context.SetCellStyle(firstItemCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                int count = 0;\n                VRExcelTableDataRow currentDataRow = firstItemRow;\n\n                columnIndexState = 8;\n\n                decimal totalAmount = 0;\n                decimal totalDuration = 0;\n                currentDate = new DateTime(incomingQueryItem1.From.Year, incomingQueryItem1.From.Month, 1);\n                var incomingQueryData1 = new NamibiaData(currentDate, incomingQueryItem1.To, incomingQueryItem1.Items, context, saleNetMeasure, saleDurationMeasure, columnIndexState, rowIndexState, sheet, currentDataRow);\n                if (incomingQueryItem1.Items != null && incomingQueryItem1.Items.Count > 0)\n                {\n                    incomingQueryData1.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    incomingQueryData1.BuildEmptyData(incomingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                incomingQueryData1.BuildTotalsTable(count, totalDuration, totalAmount);\n                #endregion\n                #region Second Item\n                rowIndexState += 4;\n\n                var secondItemTable = sheet.CreateTable(rowIndexState, 0);\n                secondItemTable.EnableTableBorders();\n                var secondItemRow = secondItemTable.CreateDataRow();\n                var secondItemCell = secondItemRow.CreateCell();\n                secondItemCell.SetValue(\"SADC\");\n                context.SetCellStyle(secondItemCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var incomingQueryItem2 = context.GetDataList(\"IncomingQuery2\", \"Main\");\n                string incomingDataListIdentifier2 = string.Format(\"{0}_{1}\", \"IncomingQuery2\", \"Main\");\n                incomingQueryItem2.ThrowIfNull(\"dataList\", incomingDataListIdentifier2);\n                incomingQueryItem2.Items.ThrowIfNull(\"dataList.Items\", incomingDataListIdentifier2);\n                incomingQueryItem2.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", incomingDataListIdentifier2);\n\n                count = 0;\n                currentDataRow = secondItemRow;\n\n                columnIndexState = 8;\n\n                totalAmount = 0;\n                totalDuration = 0;\n                currentDate = new DateTime(incomingQueryItem2.From.Year, incomingQueryItem2.From.Month, 1);\n                var incomingQueryData2 = new NamibiaData(currentDate, incomingQueryItem2.To, incomingQueryItem2.Items, context, saleNetMeasure, saleDurationMeasure, columnIndexState, rowIndexState, sheet, currentDataRow);\n                if (incomingQueryItem2.Items != null && incomingQueryItem2.Items.Count > 0)\n                {\n                    incomingQueryData2.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    incomingQueryData2.BuildEmptyData(incomingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                incomingQueryData2.BuildTotalsTable(count, totalDuration, totalAmount);\n\n                #endregion\n                #region Third Item\n\n                rowIndexState += 4;\n\n                var thirdItemTable = sheet.CreateTable(rowIndexState, 0);\n                thirdItemTable.EnableTableBorders();\n                var thirdItemRow = thirdItemTable.CreateDataRow();\n                var thirdItemCell = thirdItemRow.CreateCell();\n                thirdItemCell.SetValue(\"Rest of the World\");\n                context.SetCellStyle(thirdItemCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var incomingQueryItem3 = context.GetDataList(\"IncomingQuery3\", \"Main\");\n                string incomingDataListIdentifier3 = string.Format(\"{0}_{1}\", \"IncomingQuery3\", \"Main\");\n                incomingQueryItem3.ThrowIfNull(\"dataList\", incomingDataListIdentifier3);\n                incomingQueryItem3.Items.ThrowIfNull(\"dataList.Items\", incomingDataListIdentifier3);\n                incomingQueryItem3.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", incomingDataListIdentifier3);\n\n                count = 0;\n                currentDataRow = thirdItemRow;\n\n                columnIndexState = 8;\n\n                totalAmount = 0;\n                totalDuration = 0;\n\n                currentDate = new DateTime(incomingQueryItem3.From.Year, incomingQueryItem3.From.Month, 1);\n                var incomingQueryData3 = new NamibiaData(currentDate, incomingQueryItem3.To, incomingQueryItem3.Items, context, saleNetMeasure, saleDurationMeasure, columnIndexState, rowIndexState, sheet, currentDataRow);\n                if (incomingQueryItem3.Items != null && incomingQueryItem3.Items.Count > 0)\n                {\n                    incomingQueryData3.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    incomingQueryData3.BuildEmptyData(incomingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                incomingQueryData3.BuildTotalsTable(count, totalDuration, totalAmount);\n                #endregion\n                #endregion\n\n\n                #region Outgoing\n\n                var outgoingQueryItem1 = context.GetDataList(\"OutgoingQuery1\", \"Main\");\n                string outgoingDataListIdentifier1 = string.Format(\"{0}_{1}\", \"OutgoingQuery1\", \"Main\");\n                outgoingQueryItem1.ThrowIfNull(\"dataList\", outgoingDataListIdentifier1);\n                outgoingQueryItem1.Items.ThrowIfNull(\"dataList.Items\", outgoingDataListIdentifier1);\n                outgoingQueryItem1.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", outgoingDataListIdentifier1);\n\n\n                rowIndexState += 4;\n                var outgoingTable = sheet.CreateTable(rowIndexState, 0);\n\n                var outgoingHeaderRow = outgoingTable.CreateHeaderRow();\n                var outgoingHeaderCell = outgoingHeaderRow.CreateCell();\n                outgoingHeaderCell.SetValue(\"Outgoing\");\n                context.SetCellStyle(outgoingHeaderCell, \"White\", \"Black\", 11, true, VRExcelContainerHorizontalAlignment.Left);\n\n                rowIndexState += 2;\n                var outgoingHeaderTable = sheet.CreateTable(rowIndexState, 0);\n                outgoingHeaderTable.EnableTableBorders();\n                outgoingHeaderTable.EnableMergeHeaders();\n                var outgoingHeadersRow = outgoingHeaderTable.CreateHeaderRow();\n                var outgoingZoneHeaderCell = outgoingHeadersRow.CreateCell();\n                outgoingZoneHeaderCell.SetValue(zoneDimension);\n                context.SetCellStyle(outgoingZoneHeaderCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                currentDate = new DateTime(outgoingQueryItem1.From.Year, outgoingQueryItem1.From.Month, 1);\n\n                VRExcelTableHeaderRow currentOutgiongHeadersRow = outgoingHeadersRow;\n                VRExcelTableDataRow currentOutgoingMeasureRow = outgoingHeaderTable.CreateDataRow();\n                currentOutgoingMeasureRow.CreateCell();\n                columnIndexState = 8;\n                var outgoingHeaders = new NamibiaHeaders(currentDate, outgoingQueryItem1.To, context, columnIndexState, rowIndexState, sheet, currentOutgiongHeadersRow, currentOutgoingMeasureRow);\n                var outgoingHeadersTitles = outgoingHeaders.SetHeaders();\n\n                #region First Item\n\n                rowIndexState += 3;\n\n                var firstItemOutgoingTable = sheet.CreateTable(rowIndexState, 0);\n                firstItemOutgoingTable.EnableTableBorders();\n                var firstItemOutgoingRow = firstItemOutgoingTable.CreateDataRow();\n                var firstItemOutgoingCell = firstItemOutgoingRow.CreateCell();\n                firstItemOutgoingCell.SetValue(\"Namibia (Domestic)\");\n                context.SetCellStyle(firstItemOutgoingCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                count = 0;\n                VRExcelTableDataRow currentOutgoingDataRow = firstItemOutgoingRow;\n\n                columnIndexState = 8;\n\n                totalAmount = 0;\n                totalDuration = 0;\n                currentDate = new DateTime(outgoingQueryItem1.From.Year, outgoingQueryItem1.From.Month, 1);\n                var outgoingQueryData1 = new NamibiaData(currentDate, outgoingQueryItem1.To, outgoingQueryItem1.Items, context, costNetMeasure, costDurationMeasure, columnIndexState, rowIndexState, sheet, currentOutgoingDataRow);\n\n                if (outgoingQueryItem1.Items != null && outgoingQueryItem1.Items.Count > 0)\n                {\n                    outgoingQueryData1.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    outgoingQueryData1.BuildEmptyData(outgoingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                outgoingQueryData1.BuildTotalsTable(count, totalDuration, totalAmount);\n\n                #endregion\n\n                #region Second Item\n\n                rowIndexState += 4;\n\n                var secondItemOutgoingTable = sheet.CreateTable(rowIndexState, 0);\n                secondItemOutgoingTable.EnableTableBorders();\n                var secondItemOutgoingRow = secondItemOutgoingTable.CreateDataRow();\n                var secondItemOutgoingCell = secondItemOutgoingRow.CreateCell();\n                secondItemOutgoingCell.SetValue(\"SADC\");\n                context.SetCellStyle(secondItemOutgoingCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var outgoingQueryItem2 = context.GetDataList(\"OutgoingQuery2\", \"Main\");\n                string outgoingDataListIdentifier2 = string.Format(\"{0}_{1}\", \"OutgoingQuery2\", \"Main\");\n                outgoingQueryItem2.ThrowIfNull(\"dataList\", outgoingDataListIdentifier2);\n                outgoingQueryItem2.Items.ThrowIfNull(\"dataList.Items\", outgoingDataListIdentifier2);\n                outgoingQueryItem2.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", outgoingDataListIdentifier2);\n\n                count = 0;\n                currentOutgoingDataRow = secondItemOutgoingRow;\n\n                columnIndexState = 8;\n\n                totalAmount = 0;\n                totalDuration = 0;\n                currentDate = new DateTime(outgoingQueryItem2.From.Year, outgoingQueryItem2.From.Month, 1);\n                var outgoingQueryData2 = new NamibiaData(currentDate, outgoingQueryItem2.To, outgoingQueryItem2.Items, context, costNetMeasure, costDurationMeasure, columnIndexState, rowIndexState, sheet, currentOutgoingDataRow);\n\n                if (outgoingQueryItem2.Items != null && outgoingQueryItem2.Items.Count > 0)\n                {\n                    outgoingQueryData2.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    outgoingQueryData2.BuildEmptyData(outgoingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                outgoingQueryData2.BuildTotalsTable(count, totalDuration, totalAmount);\n                #endregion\n\n                #region Third Item\n                rowIndexState += 4;\n\n                var thirdItemOutgoingTable = sheet.CreateTable(rowIndexState, 0);\n                thirdItemOutgoingTable.EnableTableBorders();\n                var thirdItemOutgoingRow = thirdItemOutgoingTable.CreateDataRow();\n                var thirdItemOutgoingCell = thirdItemOutgoingRow.CreateCell();\n                thirdItemOutgoingCell.SetValue(\"Rest of the World\");\n                context.SetCellStyle(thirdItemOutgoingCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var outgoingQueryItem3 = context.GetDataList(\"OutgoingQuery3\", \"Main\");\n                string outgoingDataListIdentifier3 = string.Format(\"{0}_{1}\", \"OutgoingQuery3\", \"Main\");\n                outgoingQueryItem3.ThrowIfNull(\"dataList\", outgoingDataListIdentifier3);\n                outgoingQueryItem3.Items.ThrowIfNull(\"dataList.Items\", outgoingDataListIdentifier3);\n                outgoingQueryItem3.FieldInfos.ThrowIfNull(\"dataList.FieldInfos\", outgoingDataListIdentifier3);\n\n                count = 0;\n                currentOutgoingDataRow = thirdItemOutgoingRow;\n\n                columnIndexState = 8;\n\n                totalAmount = 0;\n                totalDuration = 0;\n                currentDate = new DateTime(outgoingQueryItem3.From.Year, outgoingQueryItem3.From.Month, 1);\n                var outgoingQueryData3 = new NamibiaData(currentDate, outgoingQueryItem3.To, outgoingQueryItem3.Items, context, costNetMeasure, costDurationMeasure, columnIndexState, rowIndexState, sheet, currentOutgoingDataRow);\n                if (outgoingQueryItem3.Items != null & outgoingQueryItem3.Items.Count > 0)\n                {\n                    outgoingQueryData3.SetData(out count, out totalDuration, out totalAmount);\n                }\n                else\n                {\n                    outgoingQueryData3.BuildEmptyData(outgoingHeadersTitles, out count, out totalDuration, out totalAmount);\n                }\n                outgoingQueryData3.BuildTotalsTable(count, totalDuration, totalAmount);\n\n                #endregion\n                #endregion\nreturn vrExcelFile.GenerateExcelFile();","Classes":"public class NamibiaHeaders\n    {\n        public NamibiaHeaders(DateTime startDate, DateTime endDate, IReportGenerationCustomCodeContext context, int columnIndexState, int rowIndexState, VRExcelSheet sheet, VRExcelTableHeaderRow headersRow, VRExcelTableDataRow measuresRow)\n        {\n            StartDate = startDate;\n            EndDate = endDate;\n            Context = context;\n            ColumnIndexState = columnIndexState;\n            RowIndexState = rowIndexState;\n            Sheet = sheet;\n            HeadersRow = headersRow;\n            MeasuresRow = measuresRow;\n        }\n        public DateTime StartDate { get; set; }\n        public DateTime EndDate { get; set; }\n        public IReportGenerationCustomCodeContext Context { get; set; }\n        public int ColumnIndexState { get; set; }\n        public int RowIndexState { get; set; }\n        public VRExcelSheet Sheet { get; set; }\n        public VRExcelTableHeaderRow HeadersRow { get; set; }\n        public VRExcelTableDataRow MeasuresRow { get; set; }\n        public List<string> SetHeaders()\n        {\n            string amountMeasureName = \"Amount\";\n            string durationMeasureName = \"Duration\";\n            List<string> headers = new List<string>();\n            int count = 0;\n            VRExcelTable table;\n            while (StartDate <= EndDate)\n            {\n                var dateDescription = StartDate.ToString(\"yyyy-MM\");\n                headers.Add(dateDescription);\n                if (count > 0 && count % 3 == 0)\n                {\n                    table = Sheet.CreateTable(RowIndexState, ColumnIndexState);\n                    table.EnableMergeHeaders();\n                    table.EnableTableBorders();\n                    HeadersRow = table.CreateHeaderRow();\n                    MeasuresRow = table.CreateDataRow();\n\n                    var dateCell1 = HeadersRow.CreateCell();\n                    dateCell1.SetValue(dateDescription);\n                    Context.SetCellStyle(dateCell1, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var dateCell2 = HeadersRow.CreateCell();\n                    dateCell2.SetValue(dateDescription);\n                    Context.SetCellStyle(dateCell2, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var durationCell = MeasuresRow.CreateCell();\n                    durationCell.SetValue(durationMeasureName);\n                    Context.SetCellStyle(durationCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var amountCell = MeasuresRow.CreateCell();\n                    amountCell.SetValue(amountMeasureName);\n                    Context.SetCellStyle(amountCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    ColumnIndexState += 7;\n\n                }\n                else\n                {\n                    var cell = HeadersRow.CreateCell();\n                    cell.SetValue(dateDescription);\n                    Context.SetCellStyle(cell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var cell2 = HeadersRow.CreateCell();\n                    Context.SetCellStyle(cell2, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                    cell2.SetValue(dateDescription);\n\n                    var durationCell = MeasuresRow.CreateCell();\n                    durationCell.SetValue(durationMeasureName);\n                    Context.SetCellStyle(durationCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var amountCell = MeasuresRow.CreateCell();\n                    amountCell.SetValue(amountMeasureName);\n                    Context.SetCellStyle(amountCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                }\n                count++;\n                StartDate = StartDate.AddMonths(1);\n            }\n            return headers;\n        }\n    }\n\n    public class NamibiaData\n    {\n        public NamibiaData(DateTime startDate, DateTime endDate, List<VRAutomatedReportResolvedDataItem> items, IReportGenerationCustomCodeContext context, string amountMeasure, string durationMeasure, int columnIndexState, int rowIndexState, VRExcelSheet sheet, VRExcelTableDataRow dataRow)\n        {\n            StartDate = startDate;\n            EndDate = endDate;\n            Items = items;\n            Context = context;\n            AmountMeasure = amountMeasure;\n            DurationMeasure = durationMeasure;\n            ColumnIndexState = columnIndexState;\n            RowIndexState = rowIndexState;\n            Sheet = sheet;\n            DataRow = dataRow;\n        }\n        public DateTime StartDate { get; set; }\n        public DateTime EndDate { get; set; }\n        public List<VRAutomatedReportResolvedDataItem> Items { get; set; }\n        public IReportGenerationCustomCodeContext Context { get; set; }\n        public string AmountMeasure { get; set; }\n        public string DurationMeasure { get; set; }\n        public int ColumnIndexState { get; set; }\n        public int RowIndexState { get; set; }\n        public VRExcelSheet Sheet { get; set; }\n        public VRExcelTableDataRow DataRow { get; set; }\n        public void SetData(out int count, out decimal totalDuration, out decimal totalAmount)\n        {\n            count = 0;\n            string monthDimension = \"Month\";\n            totalAmount = 0;\n            totalDuration = 0;\n            VRExcelTable outgoingDataTable;\n            for (int i = 0; i < Items.Count; i++)\n            {\n                var item = Items[i];\n                var monthItem = item.Fields.GetRecord(monthDimension);\n                var monthItemDateTimeValue = (DateTime)monthItem.Value;\n\n                DateTime toDate = monthItemDateTimeValue;\n                if (i == Items.Count - 1)\n                    toDate = EndDate;\n\n                while (StartDate <= toDate)\n                {\n                    decimal costNetValue = 0;\n                    decimal costDurationValue = 0;\n\n                    if (StartDate.Year == monthItemDateTimeValue.Year && StartDate.Month == monthItemDateTimeValue.Month)\n                    {\n                        var costNetItem = item.Fields.GetRecord(AmountMeasure);\n                        var costDurationItem = item.Fields.GetRecord(DurationMeasure);\n                        if (costNetItem.Value != null)\n                            costNetValue = (decimal)costNetItem.Value;\n                        if (costDurationItem.Value != null)\n                            costDurationValue = (decimal)costDurationItem.Value;\n                    }\n\n                    if (count > 0 && count % 3 == 0)\n                    {\n                        var newTable = Sheet.CreateTable(RowIndexState + 2, ColumnIndexState - 4);\n                        newTable.EnableTableBorders();\n                        var newRow = newTable.CreateDataRow();\n                        var newTableCell = newRow.CreateCell();\n                        newTableCell.SetValue(\"Total\");\n                        Context.SetCellStyle(newTableCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                        var totalDurationCell = newRow.CreateCell();\n                        totalDurationCell.SetValue(totalDuration);\n                        Context.SetCellStyle(totalDurationCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                        var totalAmountCell = newRow.CreateCell();\n                        totalAmountCell.SetValue(totalAmount);\n                        Context.SetCellStyle(totalAmountCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                        totalDuration = 0;\n                        totalAmount = 0;\n\n                        outgoingDataTable = Sheet.CreateTable(RowIndexState, ColumnIndexState);\n                        outgoingDataTable.EnableTableBorders();\n                        DataRow = outgoingDataTable.CreateDataRow();\n\n                        var costDurationValueCell = DataRow.CreateCell();\n                        costDurationValueCell.SetValue(costDurationValue);\n                        Context.SetCellStyle(costDurationValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                        totalDuration += costDurationValue;\n\n                        var costNetValueCell = DataRow.CreateCell();\n                        costNetValueCell.SetValue(costNetValue);\n                        Context.SetCellStyle(costNetValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                        totalAmount += costNetValue;\n                        ColumnIndexState += 7;\n\n                    }\n                    else\n                    {\n                        var costDurationValueCell = DataRow.CreateCell();\n                        costDurationValueCell.SetValue(costDurationValue);\n                        Context.SetCellStyle(costDurationValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                        totalDuration += costDurationValue;\n\n                        var costNetValueCell = DataRow.CreateCell();\n                        costNetValueCell.SetValue(costNetValue);\n                        Context.SetCellStyle(costNetValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                        totalAmount += costNetValue;\n                    }\n                    count++;\n                    StartDate = StartDate.AddMonths(1);\n                }\n            }\n        }\n\n        public void BuildEmptyData(List<string> headers, out int count, out decimal totalDuration, out decimal totalAmount)\n        {\n            decimal costNetValue = 0;\n            decimal costDurationValue = 0;\n            count = 0;\n            VRExcelTable outgoingDataTable;\n\n            totalDuration = 0;\n            totalAmount = 0;\n            foreach (var header in headers)\n            {\n                if (count > 0 && count % 3 == 0)\n                {\n                    var newTable = Sheet.CreateTable(RowIndexState + 2, ColumnIndexState - 4);\n                    newTable.EnableTableBorders();\n                    var newRow = newTable.CreateDataRow();\n                    var newTableCell = newRow.CreateCell();\n                    newTableCell.SetValue(\"Total\");\n                    Context.SetCellStyle(newTableCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var totalDurationCell = newRow.CreateCell();\n                    totalDurationCell.SetValue(totalDuration);\n                    Context.SetCellStyle(totalDurationCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    var totalAmountCell = newRow.CreateCell();\n                    totalAmountCell.SetValue(totalAmount);\n                    Context.SetCellStyle(totalAmountCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    totalDuration = 0;\n                    totalAmount = 0;\n\n                    outgoingDataTable = Sheet.CreateTable(RowIndexState, ColumnIndexState);\n                    outgoingDataTable.EnableTableBorders();\n                    DataRow = outgoingDataTable.CreateDataRow();\n\n                    var costDurationValueCell = DataRow.CreateCell();\n                    costDurationValueCell.SetValue(costDurationValue);\n                    Context.SetCellStyle(costDurationValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                    totalDuration += costDurationValue;\n\n                    var costNetValueCell = DataRow.CreateCell();\n                    costNetValueCell.SetValue(costNetValue);\n                    Context.SetCellStyle(costNetValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    totalAmount += costNetValue;\n                    ColumnIndexState += 7;\n\n                }\n                else\n                {\n                    var costDurationValueCell = DataRow.CreateCell();\n                    costDurationValueCell.SetValue(costDurationValue);\n                    Context.SetCellStyle(costDurationValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n                    totalDuration += costDurationValue;\n\n                    var costNetValueCell = DataRow.CreateCell();\n                    costNetValueCell.SetValue(costNetValue);\n                    Context.SetCellStyle(costNetValueCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                    totalAmount += costNetValue;\n                }\n                count++;\n            }\n        }\n\n        public void BuildTotalsTable(int count, decimal totalDuration, decimal totalAmount)\n        {\n            if (count > 0 && count % 3 == 0)\n            {\n                var newTable = Sheet.CreateTable(RowIndexState + 2, ColumnIndexState - 4);\n                newTable.EnableTableBorders();\n                var newRow = newTable.CreateDataRow();\n                var newTableCell = newRow.CreateCell();\n                newTableCell.SetValue(\"Total\");\n                Context.SetCellStyle(newTableCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var totalDurationCell = newRow.CreateCell();\n                totalDurationCell.SetValue(totalDuration);\n                Context.SetCellStyle(totalDurationCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                var totalAmountCell = newRow.CreateCell();\n                totalAmountCell.SetValue(totalAmount);\n                Context.SetCellStyle(totalAmountCell, \"White\", \"Black\", 8, true, VRExcelContainerHorizontalAlignment.Center);\n\n                totalDuration = 0;\n                totalAmount = 0;\n            }\n        }\n    }"}')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);