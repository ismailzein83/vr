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
                var gridColumns = GetInvoiceSubSectionGridColumn(invoiceType, input.Query.UniqueSectionID);
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
            public List<InvoiceSubSectionGridColumn> GetInvoiceSubSectionGridColumn(InvoiceType invoiceType, Guid uniqueSectionID)
            {
                List<InvoiceSubSectionGridColumn> gridColumns = null;
                foreach (var subsection in invoiceType.Settings.SubSections)
                {
                    var invoiceItemSubSection = subsection.Settings as InvoiceItemSubSection;
                    if (invoiceItemSubSection != null)
                    {
                        if (subsection.InvoiceSubSectionId == uniqueSectionID)
                        {
                            gridColumns = invoiceItemSubSection.GridColumns;
                            break;
                        }
                        else
                        {
                            gridColumns = GetInvoiceSubSectionGridColumn(invoiceItemSubSection.SubSections, uniqueSectionID);
                            if (gridColumns != null)
                                break;
                        }
                    }


                }
                return gridColumns;
            }
            public List<InvoiceSubSectionGridColumn> GetInvoiceSubSectionGridColumn(List<InvoiceItemSubSectionOfSubSuction> subSections, Guid uniqueSectionID)
            {
                if (subSections == null || subSections.Count == 0)
                    return null;
                List<InvoiceSubSectionGridColumn> gridColumns = null;

                foreach (var subsection in subSections)
                {
                    if (subsection.UniqueSectionID == uniqueSectionID)
                    {
                        gridColumns = subsection.Settings.GridColumns;
                        break;
                    }
                    else
                    {
                        gridColumns = GetInvoiceSubSectionGridColumn(subsection.Settings.SubSections, uniqueSectionID);
                        if (gridColumns != null)
                            break;
                    }
                }
                return gridColumns;
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
