using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class AccountManagerDataRecordStoragePermanentFilter : DataRecordStoragePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("A6F4D0D4-3562-4151-8ED8-984CE7A83C20");
        public override RecordFilterGroup ConvertToRecordFilter(IDataRecordStoragePermanentFilterContext context)
        {
            RecordFilterGroup recordFilterGroup = null;
            ConfigManager configManager = new ConfigManager();

            bool addRecordFilter = false;
            if (this.DataType == DataTypeEnum.Traffic)
                addRecordFilter = configManager.GetTrafficCarrierAccountFiltering();
            else if (this.DataType == DataTypeEnum.Billing)
                addRecordFilter = configManager.GetBillingCarrierAccountFiltering();

            if (addRecordFilter)
            {
                AccountManagerManager accountManagerManager = new AccountManagerManager();

                var accountManagerId = accountManagerManager.GetCurrentUserAccountManagerId();
                if (accountManagerId.HasValue)
                {
                    var customerDimensionRecordFilter = new ObjectListRecordFilter() { FieldName = this.CustomerAccountManagerField, CompareOperator = ListRecordFilterOperator.In, Values = new List<object>() { accountManagerId.Value } };
                    var supplierDimensionRecordFilter = new ObjectListRecordFilter() { FieldName = this.SupplierAccountManagerField, CompareOperator = ListRecordFilterOperator.In, Values = new List<object>() { accountManagerId.Value } };

                    recordFilterGroup = new RecordFilterGroup();
                    recordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;
                    recordFilterGroup.Filters = new List<RecordFilter>();
                    recordFilterGroup.Filters.Add(customerDimensionRecordFilter);
                    recordFilterGroup.Filters.Add(supplierDimensionRecordFilter);
                }
            }
            return recordFilterGroup;
        }
        public string CustomerAccountManagerField { get; set; }
        public string SupplierAccountManagerField { get; set; }
        public DataTypeEnum DataType { get; set; }
    }
}
