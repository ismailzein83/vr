using PartnerPortal.CustomerAccess.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor
{
    public class RetailAccountVRRestAPIRecordQueryInterceptor : Vanrise.GenericData.Entities.VRRestAPIRecordQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("B3A94A20-92ED-47BF-86D6-1034B720BE73"); } }
        public string AccountFieldName { get; set; }
        public bool WithSubAccounts { get; set; }

        public override void PrepareQuery(Vanrise.GenericData.Entities.IVRRestAPIRecordQueryInterceptorContext context)
        {
            var vrRestAPIRecordQueryInterceptorContext = context as Vanrise.GenericData.Business.VRRestAPIRecordQueryInterceptorContext;
            vrRestAPIRecordQueryInterceptorContext.ThrowIfNull("vrRestAPIRecordQueryInterceptorContext");

            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);

            List<long> accountIds = new List<long>();
            accountIds.Add(accountInfo.AccountId);

            if (WithSubAccounts)
            {
                RetailAccountInfoManager retailAccountInfoManager = new RetailAccountInfoManager();
                var childAccountsIds = retailAccountInfoManager.GetChildAccountIds(context.VRConnectionId, accountInfo.AccountBEDefinitionId, accountInfo.AccountId, true);
                if (childAccountsIds != null)
                {
                    foreach (var childAccountId in childAccountsIds)
                    {
                        accountIds.Add(childAccountId);
                    }
                }
            }

            var filters = new List<RecordFilter>();
            filters.Add(new NumberListRecordFilter
            {
                FieldName = AccountFieldName,
                CompareOperator = ListRecordFilterOperator.In,
                Values = accountIds.Select(itm => (Decimal)itm).ToList()
            });

            if (vrRestAPIRecordQueryInterceptorContext.Query == null)
                vrRestAPIRecordQueryInterceptorContext.Query = new DataRecordQuery();

            if (vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup == null)
            {
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = filters, LogicalOperator = RecordQueryLogicalOperator.And };
            }
            else
            {
                var existingFilterGroup = vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup;
                filters.Add(existingFilterGroup);
                vrRestAPIRecordQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = filters, LogicalOperator = RecordQueryLogicalOperator.And };
            }
        }
    }
}
