using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.MainExtensions.DataAnalysis.ProfilingAndCalculation
{
    public enum DealDAProfCalcAlertRuleFilterType { Sale = 0, Cost = 1, SaleOrCost = 2 }

    public class DealDAProfCalcAlertRuleFilter : DAProfCalcAlertRuleFilter
    {
        public override Guid ConfigId { get { return new Guid("E22E8134-2C77-4F8C-85CA-F8D26206D42C"); } }

        public DealDAProfCalcAlertRuleFilterType DealAlertRuleFilterType { get; set; }

        public override RecordFilterGroup GetRecordFilterGroup(IDAProfCalcGetRecordFilterGroupContext context)
        {
            switch (this.DealAlertRuleFilterType)
            {
                case DealDAProfCalcAlertRuleFilterType.Sale:
                    RecordFilterGroup saleDealRecordFilterGroup = new RecordFilterGroup();
                    saleDealRecordFilterGroup.Filters = new List<RecordFilter>();
                    saleDealRecordFilterGroup.Filters.Add(GetSaleDealRecordFilter());
                    return saleDealRecordFilterGroup;

                case DealDAProfCalcAlertRuleFilterType.Cost:
                    RecordFilterGroup costDealRecordFilterGroup = new RecordFilterGroup();
                    costDealRecordFilterGroup.Filters = new List<RecordFilter>();
                    costDealRecordFilterGroup.Filters.Add(GetCostDealRecordFilter());
                    return costDealRecordFilterGroup;

                case DealDAProfCalcAlertRuleFilterType.SaleOrCost:
                    RecordFilterGroup saleOrCostDealRecordFilterGroup = new RecordFilterGroup();
                    saleOrCostDealRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                    saleOrCostDealRecordFilterGroup.Filters = new List<RecordFilter>();
                    saleOrCostDealRecordFilterGroup.Filters.Add(GetSaleDealRecordFilter());
                    saleOrCostDealRecordFilterGroup.Filters.Add(GetCostDealRecordFilter());
                    return saleOrCostDealRecordFilterGroup;

                default: throw new NotSupportedException(string.Format("DealAlertRuleFilterType '{0}'", this.DealAlertRuleFilterType));
            }
        }

        public RecordFilter GetSaleDealRecordFilter()
        {
            RecordFilter saleDealRecordFilter = new NonEmptyRecordFilter();
            saleDealRecordFilter.FieldName = "SaleDealId";
            return saleDealRecordFilter;
        }

        public RecordFilter GetCostDealRecordFilter()
        {
            RecordFilter costDealRecordFilter = new NonEmptyRecordFilter();
            costDealRecordFilter.FieldName = "CostDealId";
            return costDealRecordFilter;
        }
    }
}