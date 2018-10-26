using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Deal.MainExtensions.ReprocessFilterDefinition
{
    public class DealReprocessFilterDefinition : Vanrise.Reprocess.Entities.ReprocessFilterDefinition
    {
        public override Guid ConfigId { get { return new Guid("DD1A3CC7-16BA-41FC-8645-B88241A6B61B"); } }

        public override RecordFilterGroup GetFilterGroup(IReprocessFilterGetFilterGroupContext context)
        {
            if (!context.TargetDataRecordTypeId.HasValue)
                throw new NullReferenceException("context.TargetDataRecordTypeId");

            Guid recordTypeId = context.TargetDataRecordTypeId.Value;

            DealReprocessFilter dealReprocessFilter = context.ReprocessFilter.CastWithValidate<DealReprocessFilter>("context.ReprocessFilter");

            Dictionary<string, string> mappings = MappingFields != null ? MappingFields.GetRecord(recordTypeId) : null;
            RecordFilter cdrTypeRecordFilter = new ObjectListRecordFilter() { FieldName = mappings.GetRecord("Type"), CompareOperator = ListRecordFilterOperator.In, Values = new List<object>() { 1, 4 } };

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup() { Filters = new List<RecordFilter>() { cdrTypeRecordFilter }, LogicalOperator = RecordQueryLogicalOperator.And };

            if ((dealReprocessFilter.CustomerIds == null || dealReprocessFilter.CustomerIds.Count == 0) && (dealReprocessFilter.SupplierIds == null || dealReprocessFilter.SupplierIds.Count == 0))
                return recordFilterGroup;

            RecordFilter customerRecordFilter = null;
            if (dealReprocessFilter.CustomerIds != null && dealReprocessFilter.CustomerIds.Count > 0)
                customerRecordFilter = new ObjectListRecordFilter() { FieldName = mappings.GetRecord("Customer"), CompareOperator = ListRecordFilterOperator.In, Values = dealReprocessFilter.CustomerIds.Select(itm => (object)itm).ToList() };

            RecordFilter supplierRecordFilter = null;
            if (dealReprocessFilter.SupplierIds != null && dealReprocessFilter.SupplierIds.Count > 0)
                supplierRecordFilter = new ObjectListRecordFilter() { FieldName = mappings.GetRecord("Supplier"), CompareOperator = ListRecordFilterOperator.In, Values = dealReprocessFilter.SupplierIds.Select(itm => (object)itm).ToList() };

            if (customerRecordFilter == null)//supplierRecordFilter is not null
            {
                recordFilterGroup.Filters.Add(supplierRecordFilter);
                return recordFilterGroup;
            }

            if (supplierRecordFilter == null)//customerRecordFilter is not null
            {
                recordFilterGroup.Filters.Add(customerRecordFilter);
                return recordFilterGroup;
            }

            //supplierRecordFilter and customerRecordFilter are not null
            RecordFilterGroup carrierAccountFilterGroup = new RecordFilterGroup() { Filters = new List<RecordFilter>() { customerRecordFilter, supplierRecordFilter }, LogicalOperator = RecordQueryLogicalOperator.Or };
            recordFilterGroup.Filters.Add(carrierAccountFilterGroup);

            return recordFilterGroup;
        }
    }
}