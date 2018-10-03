using PartnerPortal.CustomerAccess.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace CP.MultiNet.Business
{
    public class RetailAccountQueryInterceptor : GenericBEOnBeforeGetFilteredHandler
    {
        public override Guid ConfigId { get { return new Guid("3A9F0539-9810-4FE3-A859-F5AEEC3C91FC"); } }

        public string AccountFieldName { get; set; }
        public bool WithSubAccounts { get; set; }

        public override void PrepareQuery(IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext context)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);

            List<object> accountIds = new List<object>();
            accountIds.Add(accountInfo.AccountId);
            if (WithSubAccounts && context.VRConnectionId.HasValue)
            {
                RetailAccountInfoManager retailAccountInfoManager = new RetailAccountInfoManager();
                var childAccountsIds = retailAccountInfoManager.GetChildAccountIds(context.VRConnectionId.Value, accountInfo.AccountBEDefinitionId, accountInfo.AccountId, true);
                if (childAccountsIds != null)
                {
                    foreach (var childAccountId in childAccountsIds)
                    {
                        accountIds.Add(childAccountId);
                    }
                }
            }
            if(context.Query.Filters==null)
                context.Query.Filters = new List<GenericBusinessEntityFilter>();

            context.Query.Filters.Add(new GenericBusinessEntityFilter()
            {
                FieldName = AccountFieldName,
                FilterValues = accountIds
            });
        }

        public override void onBeforeAdd(IGenericBEOnBeforeAddHandlerContext context)
        {
            if (context.GenericBusinessEntityToAdd != null && context.GenericBusinessEntityToAdd.FieldValues!=null && context.GenericBusinessEntityToAdd.FieldValues.Count>0)
            {
                int userId = SecurityContext.Current.GetLoggedInUserId();
                RetailAccountUserManager manager = new RetailAccountUserManager();
                var accountInfo = manager.GetRetailAccountInfo(userId);
                accountInfo.ThrowIfNull("accountInfo", userId);
               
                object accountFieldValue;
                if (context.GenericBusinessEntityToAdd.FieldValues.TryGetValue(AccountFieldName, out accountFieldValue))
                {
                    List<object> accountIds = new List<object>();
                    accountIds.Add(accountInfo.AccountId);
                    if (WithSubAccounts && context.VRConnectionId.HasValue)
                    {
                        RetailAccountInfoManager retailAccountInfoManager = new RetailAccountInfoManager();
                        var childAccountsIds = retailAccountInfoManager.GetChildAccountIds(context.VRConnectionId.Value, accountInfo.AccountBEDefinitionId, accountInfo.AccountId, true);
                        if (childAccountsIds != null)
                        {
                            foreach (var childAccountId in childAccountsIds)
                            {
                                accountIds.Add(childAccountId);
                            }
                        }
                    }
                    if (!accountIds.Contains(accountFieldValue))
                    {
                        throw new NullReferenceException(string.Format("{0} '{1}'", AccountFieldName, accountFieldValue));
                    }
                }
                else
                {
                    context.GenericBusinessEntityToAdd.FieldValues.Add(AccountFieldName, accountInfo.AccountId);
                }
            }
        }

        public override void onBeforeUpdate(IGenericBEOnBeforeUpdateHandlerContext context)
        {
            if (context.GenericBusinessEntityToUpdate != null && context.GenericBusinessEntityToUpdate.FieldValues!=null && context.GenericBusinessEntityToUpdate.FieldValues.Count>0)
            {
                int userId = SecurityContext.Current.GetLoggedInUserId();
                RetailAccountUserManager manager = new RetailAccountUserManager();
                var accountInfo = manager.GetRetailAccountInfo(userId);
                accountInfo.ThrowIfNull("accountInfo", userId);

                object accountFieldValue;
                if (context.GenericBusinessEntityToUpdate.FieldValues.TryGetValue(AccountFieldName, out accountFieldValue))
                {
                    List<object> accountIds = new List<object>();
                    accountIds.Add(accountInfo.AccountId);
                    if (WithSubAccounts && context.VRConnectionId.HasValue)
                    {
                        RetailAccountInfoManager retailAccountInfoManager = new RetailAccountInfoManager();
                        var childAccountsIds = retailAccountInfoManager.GetChildAccountIds(context.VRConnectionId.Value, accountInfo.AccountBEDefinitionId, accountInfo.AccountId, true);
                        if (childAccountsIds != null)
                        {
                            foreach (var childAccountId in childAccountsIds)
                            {
                                accountIds.Add(childAccountId);
                            }
                        }
                    }

                    if (!accountIds.Contains(accountFieldValue))
                    {
                        throw new NullReferenceException(string.Format("{0} '{1}'", AccountFieldName, accountFieldValue));
                    }
                }
                else
                {
                    context.GenericBusinessEntityToUpdate.FieldValues.Add(AccountFieldName, accountInfo.AccountId);
                }
            }
        }
    }
}
