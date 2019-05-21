﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerAnalyticPermanentFilter : AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("4A3AD674-9ADB-40C6-BEFD-A1813F08F333");
        public override RecordFilterGroup ConvertToRecordFilter(IAnalyticTablePermanentFilterContext context)
        {
            RecordFilterGroup recordFilterGroup = null;

            AccountManagerManager accountManagerManager = new AccountManagerManager();

            var accountManagerId = accountManagerManager.GetCurrentUserAccountManagerId();
            if (accountManagerId.HasValue)
            {
                var customerDimensionRecordFilter = new ObjectListRecordFilter() { FieldName = this.CustomerAccountManagerDimension, CompareOperator = ListRecordFilterOperator.In, Values = new List<object>() { accountManagerId.Value } };
                var supplierDimensionRecordFilter = new ObjectListRecordFilter() { FieldName = this.SupplierAccountManagerDimension, CompareOperator = ListRecordFilterOperator.In, Values = new List<object>() { accountManagerId.Value } };

                recordFilterGroup = new RecordFilterGroup();
                recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                recordFilterGroup.Filters = new List<RecordFilter>();
                recordFilterGroup.Filters.Add(customerDimensionRecordFilter);
                recordFilterGroup.Filters.Add(supplierDimensionRecordFilter);
            }
            return recordFilterGroup;
        }
        public string CustomerAccountManagerDimension { get; set; }
        public string SupplierAccountManagerDimension { get; set; }
    }
}
