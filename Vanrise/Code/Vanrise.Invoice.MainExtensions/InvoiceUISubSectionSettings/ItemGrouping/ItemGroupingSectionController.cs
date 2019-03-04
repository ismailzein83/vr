﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;

namespace Vanrise.Invoice.MainExtensions
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ItemGroupingSection")]
    [JSONWithTypeAttribute]
    public class ItemGroupingSectionController:BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredGroupingInvoiceItems")]
        public object GetFilteredGroupingInvoiceItems(DataRetrievalInput<ItemGroupingSectionQuery> input)
        {
            InvoiceItemManager manager = new InvoiceItemManager();

            DataRetrievalInput<GroupingInvoiceItemQuery> itemGroupingInput = new DataRetrievalInput<GroupingInvoiceItemQuery>()
            {
                DataRetrievalResultType = input.DataRetrievalResultType,
                SortByColumnName = input.SortByColumnName,
                GetSummary = input.GetSummary,
                IsSortDescending = input.IsSortDescending,
                FromRow = input.FromRow,
                Query = new GroupingInvoiceItemQuery
                {
                    DimensionIds = input.Query.DimensionIds,
                    Filters=input.Query.Filters,
                    InvoiceId=input.Query.InvoiceId,
                    InvoiceTypeId=input.Query.InvoiceTypeId,
                    ItemGroupingId=input.Query.ItemGroupingId,
                    MeasureIds = input.Query.MeasureIds,
                    UniqueSectionID = input.Query.UniqueSectionID
                },
                ResultKey = input.ResultKey,
                ToRow = input.ToRow
            };
           
            if (input.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                var result = manager.GetFilteredGroupingInvoiceItems(itemGroupingInput) as Vanrise.Entities.BigResult<GroupingInvoiceItemDetail>;

                Vanrise.Entities.BigResult<ItemGroupingSectionResult> bigDataResult = new BigResult<ItemGroupingSectionResult>();
                bigDataResult.ResultKey = result.ResultKey;

                if (result.Data != null)
                 {
                     List<ItemGroupingSectionResult> itemGroupingSectionResults = new List<ItemGroupingSectionResult>();
                     RecordFilterManager filterManager = new RecordFilterManager();
                     Dictionary<string, ItemGroupingFieldInfo> fieldInfo = new Dictionary<string, ItemGroupingFieldInfo>();
                     ItemGroupingSectionSettings section = null;
                     if( input.Query.SectionId.HasValue)
                     {
                        
                        var invoiceType = new InvoiceTypeManager().GetInvoiceType(input.Query.InvoiceTypeId);
                        section = GetSection(invoiceType, input.Query.SectionId.Value);
                        if(section != null)
                        {
                            if (section.SubSections != null)
                            {
                                var itemGrouping = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == input.Query.ItemGroupingId);
                                foreach (var gridDimension in section.GridDimesions)
                                {
                                    for (var i = 0; i < itemGrouping.DimensionItemFields.Count; i++)
                                    {
                                        var dimension = itemGrouping.DimensionItemFields[i];
                                        if (dimension.DimensionItemFieldId == gridDimension.DimensionId)
                                        {
                                            fieldInfo.Add(dimension.FieldName, new ItemGroupingFieldInfo
                                            {
                                                DimensionIndex = i,
                                                FieldType = dimension.FieldType,
                                            });
                                            break;
                                        }
                                    }
                                }
                                foreach (var gridMeasure in section.GridMeasures)
                                {
                                    var measure = itemGrouping.AggregateItemFields.FirstOrDefault(x => x.AggregateItemFieldId == gridMeasure.MeasureId);
                                    fieldInfo.Add(measure.FieldName, new ItemGroupingFieldInfo
                                    {
                                        MeasureName = measure.FieldName,
                                        FieldType = measure.FieldType,
                                    });
                                }
                            }

                           
                           
                        }
                     }
                     foreach (var item in result.Data)
                     {
                         ItemGroupingSectionResult itemGroupingSectionResult = new ItemGroupingSectionResult
                         {
                             DimensionValues = item.DimensionValues,
                             MeasureValues = item.MeasureValues,
                             SubSectionsIds = new List<Guid>()
                         };

                         if (section != null &&  section.SubSections != null)
                         {
                             ItemGroupingFilterFieldMatchContext context = new ItemGroupingFilterFieldMatchContext(item, fieldInfo);

                             foreach (var subSection in section.SubSections)
                             {
                                 if (subSection.SubSectionFilter==null || filterManager.IsFilterGroupMatch(subSection.SubSectionFilter, context))
                                     itemGroupingSectionResult.SubSectionsIds.Add(subSection.InvoiceSubSectionId);
                             }
                         }
                         itemGroupingSectionResults.Add(itemGroupingSectionResult);
                     }
                     bigDataResult.Data = itemGroupingSectionResults;
                 }
                 bigDataResult.TotalCount = result.TotalCount;
                return GetWebResponse(input, bigDataResult);
            }
            else
            {
                var excelResult = manager.GetFilteredGroupingInvoiceItems(itemGroupingInput) as Vanrise.Entities.ExcelResult<GroupingInvoiceItemDetail>;
                return GetWebResponse(input, excelResult, GetSectionTitle(input.Query.InvoiceTypeId, input.Query.UniqueSectionID));
            }
        }
        private string GetSectionTitle(Guid invoiceTypeId, Guid uniqueSectionID)
        {
            string sectionTitle = null;
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", invoiceTypeId);
            invoiceType.Settings.SubSections.ThrowIfNull("invoiceType.Settings.SubSections", invoiceTypeId);
            foreach (var section in invoiceType.Settings.SubSections)
            {
                var subsection = section.Settings as ItemGroupingSection;
                if (subsection != null)
                {
                    if (section.InvoiceSubSectionId == uniqueSectionID)
                    {
                        sectionTitle = section.SectionTitle;
                        break;
                    }
                    foreach (var item in subsection.Settings.SubSections)
                    {
                        if (item.InvoiceSubSectionId == uniqueSectionID)
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

        private string GetSectionTitle(List<ItemGroupingSubSection> subSections, Guid uniqueSectionID)
        {
            if (subSections == null || subSections.Count == 0)
                return null;
            string sectionTitle = null;

            foreach (var subsection in subSections)
            {
                if (subsection.InvoiceSubSectionId == uniqueSectionID)
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
        private ItemGroupingSectionSettings GetSection(InvoiceType invoiceType, Guid sectionId)
        {
            ItemGroupingSectionSettings subsectionItem = null;
            foreach(var section in invoiceType.Settings.SubSections)
            {

                var subSection = section.Settings as ItemGroupingSection;
                if(subSection != null)
                {
                    if(section.InvoiceSubSectionId == sectionId)
                    {
                        subsectionItem = subSection.Settings;
                        break;
                    }
                    foreach(var item in subSection.Settings.SubSections)
                    {
                        if(item.InvoiceSubSectionId == sectionId)
                        {
                            subsectionItem = item.Settings;
                            break;
                        }else
                        {
                            subsectionItem = GetSection(item.Settings.SubSections, sectionId);
                            if (subsectionItem != null)
                                break;
                        }
                    }
                }
            }
            return subsectionItem;
        }
        private ItemGroupingSectionSettings GetSection(List<ItemGroupingSubSection> subSections, Guid sectionId)
        {
            if (subSections == null || subSections.Count == 0)
                return null;
            ItemGroupingSectionSettings section = null;

            foreach (var subsection in subSections)
            {
                if (subsection.InvoiceSubSectionId == sectionId)
                {
                    section = subsection.Settings;
                    break;
                }
                else
                {
                    section = GetSection(subsection.Settings.SubSections, sectionId);
                    if (section != null)
                        break;
                }
            }
            return section;
        }
    }
}
