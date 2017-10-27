using PartnerPortal.CustomerAccess.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Common;

namespace PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIAnalyticQueryInterceptor
{
    public class RetailAccountVRRestAPIAnalyticQueryInterceptor : Vanrise.Analytic.Entities.VRRestAPIAnalyticQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("1DC33B53-B625-4E51-B427-7952E3817708"); } }
        public string AccountFieldName { get; set; }
        public bool WithSubAccounts { get; set; }

        public override void PrepareQuery(Vanrise.Analytic.Entities.IVRRestAPIAnalyticQueryInterceptorContext context)
        {
            var vrRestAPIAnalyticQueryInterceptorContext = context as Vanrise.Analytic.Business.VRRestAPIAnalyticQueryInterceptorContext; 
            if (vrRestAPIAnalyticQueryInterceptorContext == null)
                throw new Exception("vrRestAPIRecordQueryInterceptorContext is not of type VRRestAPIRecordQueryInterceptorContext.");

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
            filters.Add(new ObjectListRecordFilter
            {
                FieldName = AccountFieldName,
                CompareOperator = ListRecordFilterOperator.In,
                Values = accountIds.Select(itm => itm as object).ToList()
            });

            if (vrRestAPIAnalyticQueryInterceptorContext.Query == null)
                vrRestAPIAnalyticQueryInterceptorContext.Query = new Vanrise.Analytic.Entities.AnalyticQuery();

            if (vrRestAPIAnalyticQueryInterceptorContext.Query.FilterGroup == null)
            {
                vrRestAPIAnalyticQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = filters, LogicalOperator = RecordQueryLogicalOperator.And };
            }
            else
            {
                var existingFilterGroup = vrRestAPIAnalyticQueryInterceptorContext.Query.FilterGroup;
                filters.Add(existingFilterGroup);
                vrRestAPIAnalyticQueryInterceptorContext.Query.FilterGroup = new RecordFilterGroup() { Filters = filters, LogicalOperator = RecordQueryLogicalOperator.And };
            }
        }
    }
}
