using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceItemManager
    {
        #region Public Methods
        public IDataRetrievalResult<InvoiceItemDetail> GetFilteredInvoiceItems(DataRetrievalInput<InvoiceItemQuery> input)
        {

            return BigDataManager.Instance.RetrieveData(input, new InvoiceItemRequestHandler());
        }

        public static string ExecuteItemSetNameParts(List<InvoiceItemConcatenatedPart> itemSetNameParts, dynamic invoiceItemDetails, string currentItemSetName)
        {
            if (itemSetNameParts != null && itemSetNameParts.Count > 0)
            {
                StringBuilder itemSetName = new StringBuilder();
                InvoiceItemConcatenatedPartContext context = new InvoiceItemConcatenatedPartContext
                {
                    InvoiceItemDetails = invoiceItemDetails,
                    CurrentItemSetName = currentItemSetName
                };
                foreach (var part in itemSetNameParts)
                {
                    itemSetName.Append(part.Settings.GetPartText(context));
                }
                return itemSetName.ToString();
            }
            return null;
        }
        public IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(long invoiceId, List<string> itemSetNames, CompareOperator compareOperator)
        {
            IEnumerable<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            if (itemSetNames != null)
            {
                var invoiceTypeId = new InvoiceManager().GetInvoiceTypeId(invoiceId);
                invoiceItems = GetInvoiceItemsByItemSetNames(invoiceTypeId, new List<long> { invoiceId }, itemSetNames, compareOperator);
            }
            return invoiceItems;
        }

        public string GetSubsectionTitle(Guid invoiceTypeId, Guid uniqueSectionID)
        {
            string sectionTitle = null;
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", invoiceTypeId);
            invoiceType.Settings.SubSections.ThrowIfNull("invoiceType.Settings.SubSections", invoiceTypeId);
            foreach (var section in invoiceType.Settings.SubSections)
            {
                var subsection = section.Settings as InvoiceItemSubSection;
                if (subsection != null)
                {
                    if (section.InvoiceSubSectionId == uniqueSectionID)
                    {
                        sectionTitle = section.SectionTitle;
                        break;
                    }
                    foreach (var item in subsection.SubSections)
                    {
                        if (item.UniqueSectionID == uniqueSectionID)
                        {
                            sectionTitle = item.SectionTitle;
                            break;
                        }
                        else
                        {
                            sectionTitle = GetSectionTitle(item.Settings.SubSections, uniqueSectionID);
                            if (sectionTitle != null)
                                break;
                        }
                    }
                    if (sectionTitle != null)
                        break;
                }
            }
            return sectionTitle;
        }

        private string GetSectionTitle(List<InvoiceItemSubSectionOfSubSuction> subSections, Guid uniqueSectionID)
        {
            if (subSections == null || subSections.Count == 0)
                return null;
            string sectionTitle = null;

            foreach (var subsection in subSections)
            {
                if (subsection.UniqueSectionID == uniqueSectionID)
                {
                    sectionTitle = subsection.SectionTitle;
                    break;
                }
                else
                {
                    sectionTitle = GetSectionTitle(subsection.Settings.SubSections, uniqueSectionID);
                    if (sectionTitle != null)
                        break;
                }
            }
            return sectionTitle;
        }

        public IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(Guid invoiceTypeId, List<long> invoiceIds, List<string> itemSetNames, CompareOperator compareOperator)
        {
            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            if (itemSetNames != null)
            {
                var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
                Dictionary<string, List<string>> itemSetNamesByStorageConnectionString = GetItemSetNamesByStorageConnectionString(invoiceTypeId, itemSetNames);
                foreach (var item in itemSetNamesByStorageConnectionString)
                {
                    IInvoiceItemDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
                    dataManager.StorageConnectionStringKey = item.Key;
                    var result = dataManager.GetInvoiceItemsByItemSetNames(invoiceIds, item.Value, compareOperator);
                    invoiceItems.AddRange(result);
                }
                var remainingInvoiceItemSets = itemSetNames.FindAllRecords(x => !itemSetNamesByStorageConnectionString.Values.Any(y => y.Contains(x)));
                if (remainingInvoiceItemSets != null)
                {
                    IInvoiceItemDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
                    var result = dataManager.GetInvoiceItemsByItemSetNames(invoiceIds, remainingInvoiceItemSets, compareOperator);
                    invoiceItems.AddRange(result);

                }
                FillAdditionalInvoiceItems(invoiceItems, invoiceType);
            }
            return invoiceItems;
        }

        public IDataRetrievalResult<GroupingInvoiceItemDetail> GetFilteredGroupingInvoiceItems(DataRetrievalInput<GroupingInvoiceItemQuery> input)
        {
            if (input.SortByColumnName != null && input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""].Value", measureProperty[0], measureProperty[1]);
            }
            return BigDataManager.Instance.RetrieveData(input, new GroupingInvoiceItemRequestHandler());
        }

        public Dictionary<string, List<string>> GetItemSetNamesByStorageConnectionString(Guid invoiceTypeId, IEnumerable<string> itemSetNames)
        {
            Dictionary<string, List<string>> itemSetNamesByStorageConnectionString = new Dictionary<string, List<string>>();
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            foreach (var itemSetName in itemSetNames)
            {
                string storageConnectionString = invoiceTypeManager.GetItemSetNameStorageInfo(invoiceTypeId, itemSetName);
                if (storageConnectionString != null)
                {
                    var storageItemSetNames = itemSetNamesByStorageConnectionString.GetOrCreateItem(storageConnectionString, () =>
                    {
                        return new List<string>();
                    });
                    storageItemSetNames.Add(itemSetName);
                }
            }
            return itemSetNamesByStorageConnectionString;
        }

        #endregion

        #region Private Classes


        private class InvoiceItemRequestHandler : BigDataRequestHandler<InvoiceItemQuery, Entities.InvoiceItemDetail, Entities.InvoiceItemDetail>
        {
            public InvoiceItemRequestHandler()
            {
            }
            public override InvoiceItemDetail EntityDetailMapper(Entities.InvoiceItemDetail entity)
            {
                return entity;
            }
            public override IEnumerable<Entities.InvoiceItemDetail> RetrieveAllData(DataRetrievalInput<InvoiceItemQuery> input)
            {

                InvoiceTypeManager manager = new InvoiceTypeManager();
                var invoiceType = manager.GetInvoiceType(input.Query.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", input.Query.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", input.Query.InvoiceTypeId);
                invoiceType.Settings.SubSections.ThrowIfNull("invoiceType.Settings.SubSections", input.Query.InvoiceTypeId);
                List<InvoiceSubSectionGridColumn> gridColumns = null;
                foreach (var subsection in invoiceType.Settings.SubSections)
                {
                    gridColumns = subsection.Settings.GetSubsectionGridColumns(invoiceType, input.Query.UniqueSectionID);
                    if (gridColumns != null)
                        break;
                }
                var itemSetNameStorageConnectionString = manager.GetItemSetNameStorageInfo(input.Query.InvoiceTypeId, input.Query.ItemSetName);
                IInvoiceItemDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceItemDataManager>();
                _dataManager.StorageConnectionStringKey = itemSetNameStorageConnectionString;
                if (input.Query.ItemSetNameParts != null && input.Query.ItemSetNameParts.Count > 0)
                {
                    input.Query.ItemSetName = InvoiceItemManager.ExecuteItemSetNameParts(input.Query.ItemSetNameParts, input.Query.InvoiceItemDetails, input.Query.ItemSetName);
                    input.Query.CompareOperator = CompareOperator.Equal;
                }

                var results = _dataManager.GetFilteredInvoiceItems(input.Query.InvoiceId, input.Query.ItemSetName, input.Query.CompareOperator);

                InvoiceItemManager.FillAdditionalInvoiceItems(results, invoiceType);

                List<Entities.InvoiceItemDetail> detailedResults = new List<InvoiceItemDetail>();
                foreach (var item in results)
                {
                    InvoiceItemDetail invoiceItemDetail = new Entities.InvoiceItemDetail();
                    invoiceItemDetail.Items = new List<InvoiceItemDetailObject>();
                    invoiceItemDetail.Entity = item;
                    if (gridColumns != null)
                    {
                        foreach (var column in gridColumns)
                        {
                            var fieldValue = item.Details.GetType().GetProperty(column.FieldName).GetValue(item.Details, null);
                            // Vanrise.Common.Utilities.GetPropValue(column.FieldName, item.Details) 
                            //Vanrise.Common.Utilities.GetPropValueReader(column.FieldName).GetPropertyValue(item.Details);
                            invoiceItemDetail.Items.Add(new InvoiceItemDetailObject
                            {
                                Description = column.FieldType.GetDescription(fieldValue),
                                Value = fieldValue
                            });
                        }
                    }
                    detailedResults.Add(invoiceItemDetail);

                }

                return detailedResults;
            }
            protected override ResultProcessingHandler<InvoiceItemDetail> GetResultProcessingHandler(DataRetrievalInput<InvoiceItemQuery> input, BigResult<InvoiceItemDetail> bigResult)
            {
                return new ResultProcessingHandler<InvoiceItemDetail>()
                {
                    ExportExcelHandler = new InvoiceItemExcelExportHandler(input.Query)
                };
            }
        }

        private class GroupingInvoiceItemRequestHandler : BigDataRequestHandler<GroupingInvoiceItemQuery, Entities.GroupingInvoiceItemDetail, Entities.GroupingInvoiceItemDetail>
        {
            public GroupingInvoiceItemRequestHandler()
            {
            }
            public override GroupingInvoiceItemDetail EntityDetailMapper(Entities.GroupingInvoiceItemDetail entity)
            {
                return entity;
            }
            public override IEnumerable<Entities.GroupingInvoiceItemDetail> RetrieveAllData(DataRetrievalInput<GroupingInvoiceItemQuery> input)
            {
                return new InvoiceItemGroupingManager().GetFilteredGroupingInvoiceItems(input.Query);
            }
            protected override ResultProcessingHandler<GroupingInvoiceItemDetail> GetResultProcessingHandler(DataRetrievalInput<GroupingInvoiceItemQuery> input, BigResult<GroupingInvoiceItemDetail> bigResult)
            {
                return new ResultProcessingHandler<GroupingInvoiceItemDetail>
                {
                    ExportExcelHandler = new GroupingInvoiceItemExcelExportHandler(input.Query)
                };
            }

        }
        private class InvoiceItemExcelExportHandler : ExcelExportHandler<InvoiceItemDetail>
        {
            InvoiceItemQuery _query;
            public InvoiceItemExcelExportHandler(InvoiceItemQuery query)
            {
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<InvoiceItemDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet() { Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() } };
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                InvoiceItemRequestHandler invoiceItemRequestHandler = new InvoiceItemRequestHandler();
                var invoiceType = invoiceTypeManager.GetInvoiceType(_query.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", _query.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", _query.InvoiceTypeId);
                invoiceType.Settings.SubSections.ThrowIfNull("invoiceType.Settings.SubSections", _query.InvoiceTypeId);
                List<InvoiceSubSectionGridColumn> gridColumns = null;
                foreach(var subsection in invoiceType.Settings.SubSections)
                {
                    gridColumns = subsection.Settings.GetSubsectionGridColumns(invoiceType, _query.UniqueSectionID);
                    if (gridColumns != null)
                        break;
                }
                if (gridColumns != null && gridColumns.Count > 0)
                {
                    foreach (var column in gridColumns)
                    {
                        var headerCell = new ExportExcelHeaderCell { Title = column.Header };
                        if (column.FieldType != null)
                        {
                            column.FieldType.SetExcelCellType(new DataRecordFieldTypeSetExcelCellTypeContext { HeaderCell = headerCell });
                        }
                        sheet.Header.Cells.Add(headerCell);
                    }
                }
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    sheet.Rows = new List<ExportExcelRow>();
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow();
                        if (record.Items != null && record.Items.Count > 0)
                        {
                            int index = 0;
                            row.Cells = new List<ExportExcelCell>();
                            foreach (var item in record.Items)
                            {
                                var gridColumn = gridColumns.ElementAtOrDefault(index);
                                row.Cells.Add(new ExportExcelCell()
                                {
                                    Value = gridColumn != null && gridColumn.FieldType != null && gridColumn.FieldType.RenderDescriptionByDefault() ? item.Description : item.Value
                                });
                                index++;
                            }
                        }
                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }
        private class GroupingInvoiceItemExcelExportHandler : ExcelExportHandler<GroupingInvoiceItemDetail>
        {
            GroupingInvoiceItemQuery _query;
            public GroupingInvoiceItemExcelExportHandler(GroupingInvoiceItemQuery query)
            {
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<GroupingInvoiceItemDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet() { Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() } };
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(_query.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", _query.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", _query.InvoiceTypeId);
                invoiceType.Settings.SubSections.ThrowIfNull("invoiceType.Settings.SubSections", _query.InvoiceTypeId);
                invoiceType.Settings.ItemGroupings.ThrowIfNull("invoiceType.Settings.ItemGroupings", _query.InvoiceTypeId);
                var groupingItem = invoiceType.Settings.ItemGroupings.FindRecord(x => x.ItemGroupingId == _query.ItemGroupingId);
                groupingItem.ThrowIfNull("groupingItem", _query.ItemGroupingId);
                groupingItem.DimensionItemFields.ThrowIfNull("groupingItem.DimensionItemFields", _query.ItemGroupingId);
                groupingItem.AggregateItemFields.ThrowIfNull("groupingItem.AggregateItemFields", _query.ItemGroupingId);
                List<InvoiceSubSectionGridColumn> gridColumns = null;
                foreach (var subsection in invoiceType.Settings.SubSections)
                {
                    gridColumns = subsection.Settings.GetSubsectionGridColumns(invoiceType, _query.UniqueSectionID);
                    if (gridColumns != null)
                        break;
                }
                if (gridColumns != null && gridColumns.Count > 0)
                {
                    int columnIndex = 0;
                    if (_query.DimensionIds != null && _query.DimensionIds.Count > 0)
                    {
                        for (int i = 0; i < _query.DimensionIds.Count; i++)
                        {
                            var dimensionId = _query.DimensionIds[i];
                            var gridColumn = gridColumns[columnIndex];
                            var headerCell = new ExportExcelHeaderCell() { Title = gridColumn.Header };
                            var dimensionItem = groupingItem.DimensionItemFields.FindRecord(x => x.DimensionItemFieldId == dimensionId);
                            dimensionItem.ThrowIfNull("dimenionItem", dimensionId);
                            dimensionItem.FieldType.ThrowIfNull("dimenionItem.FieldType", dimensionId);
                            dimensionItem.FieldType.SetExcelCellType(new DataRecordFieldTypeSetExcelCellTypeContext
                            {
                                HeaderCell = headerCell
                            });
                            sheet.Header.Cells.Add(headerCell);
                            columnIndex++;
                        }
                    }
                    if (_query.MeasureIds != null && _query.MeasureIds.Count > 0)
                    {
                        for (int i = 0; i < _query.MeasureIds.Count; i++)
                        {
                            var measureId = _query.MeasureIds[i];
                            var gridColumn = gridColumns[columnIndex];
                            var headerCell = new ExportExcelHeaderCell() { Title = gridColumn.Header };
                            var measureItem = groupingItem.AggregateItemFields.FindRecord(x => x.AggregateItemFieldId == measureId);
                            measureItem.ThrowIfNull("measureItem", measureId);
                            measureItem.FieldType.ThrowIfNull("measureItem.FieldType", measureId);
                            measureItem.FieldType.SetExcelCellType(new DataRecordFieldTypeSetExcelCellTypeContext
                            {
                                HeaderCell = headerCell
                            });
                            sheet.Header.Cells.Add(headerCell);
                            columnIndex++;
                        }
                    }
                }
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    sheet.Rows = new List<ExportExcelRow>();
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        if (record.DimensionValues != null && record.DimensionValues.Length > 0)
                        {
                            for (int i = 0; i < record.DimensionValues.Length; i++)
                            {
                                var dimValue = record.DimensionValues[i];
                                var dimId = _query.DimensionIds.ElementAtOrDefault(i);
                                var dimensionItem = groupingItem.DimensionItemFields.FindRecord(x => x.DimensionItemFieldId == dimId);
                                dimensionItem.ThrowIfNull("dimenionItem", dimId);
                                dimensionItem.FieldType.ThrowIfNull("dimenionItem.FieldType", dimId);
                                row.Cells.Add(new ExportExcelCell { Value = dimensionItem.FieldType.RenderDescriptionByDefault() ? dimValue.Name : dimValue.Value });
                            }
                        }
                        if (record.MeasureValues != null && record.MeasureValues.Count > 0)
                        {
                            int index = 0;
                            foreach (var measureValue in record.MeasureValues)
                            {
                                if (measureValue.Value != null)
                                {
                                    var measureId = _query.MeasureIds.ElementAtOrDefault(index);
                                    var measureItem = groupingItem.AggregateItemFields.FindRecord(x => x.AggregateItemFieldId == measureId);
                                    measureItem.ThrowIfNull("measureItem", measureId);
                                    measureItem.FieldType.ThrowIfNull("measureItem.FieldType", measureId);
                                    row.Cells.Add(new ExportExcelCell { Value = measureItem.FieldType.RenderDescriptionByDefault() ? measureItem.FieldType.GetDescription(measureValue.Value.Value) : measureValue.Value.Value });
                                }
                                index++;
                            }
                        }
                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class DataRecordFieldTypeSetExcelCellTypeContext : IDataRecordFieldTypeSetExcelCellTypeContext
        {
            public ExportExcelHeaderCell HeaderCell { get; set; }
        }

        #endregion

        public static void FillAdditionalInvoiceItems(IEnumerable<InvoiceItem> invoiceItems, InvoiceType invoiceType)
        {
            if (invoiceItems != null)
            {
                InvoiceItemAdditionalFieldsContext context = new InvoiceItemAdditionalFieldsContext
                {
                    InvoiceType = invoiceType
                };
                foreach (var invoiceItem in invoiceItems)
                {
                    var invoiceItemWithAdditionalFields = invoiceItem.Details as IInvoiceItemAdditionalFields;
                    if (invoiceItemWithAdditionalFields != null)
                        invoiceItemWithAdditionalFields.FillAdditionalFields(context);
                }
            }
        }
    }
}
