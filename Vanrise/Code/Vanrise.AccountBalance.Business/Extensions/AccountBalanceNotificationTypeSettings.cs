using System;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("0FC411D1-90FD-417C-BFDF-EC0C35B1A666"); } }

        public override string SearchRuntimeEditor { get { return "vr-accountbalance-notification-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-accountbalance-notification-bodyeditor"; } }

        public string AccountColumnHeader { get; set; }

        public bool ShowAccountTypeColumn { get; set; }

        public AccountBalanceNotificationTypeExtendedSettings AccountBalanceNotificationTypeExtendedSettings { get; set; }


        public override bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context)
        {
            AccountBalanceNotificationTypeExtendedSettings.ThrowIfNull("AccountBalanceNotificationTypeExtendedSettings");

            if (context.ExtendedQuery == null)
                return true;

            var accountBalanceNotificationQuery = context.ExtendedQuery as AccountBalanceNotificationQuery;
            if (accountBalanceNotificationQuery == null)
                return false;

            if (context.VRNotification == null || context.VRNotification.EventPayload == null)
                return false;

            var vrBalanceAlertEventPayload = context.VRNotification.EventPayload as VRBalanceAlertEventPayload;
            if (vrBalanceAlertEventPayload == null)
                return false;

            var accountBalanceAlertRuleTypeSettings = new VRAlertRuleTypeManager().GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(vrBalanceAlertEventPayload.AlertRuleTypeId);

            var accountBalanceNotificationTypeIsMatchedContext = new AccountBalanceNotificationTypeIsMatchedContext()
            {
                EventPayload = vrBalanceAlertEventPayload,
                AccountTypeId = accountBalanceAlertRuleTypeSettings.AccountTypeId,
                AccountBalanceNotificationExtendedQuery = accountBalanceNotificationQuery.AccountBalanceNotificationExtendedQuery
            };

            return AccountBalanceNotificationTypeExtendedSettings.IsVRNotificationMatched(accountBalanceNotificationTypeIsMatchedContext);
        }

        public override VRNotificationDetailEventPayload GetNotificationDetailEventPayload(IVRNotificationTypeGetNotificationEventPayloadContext context)
        {
            var vrBalanceAlertEventPayload = new VRNotificationManager().GetVRNotificationEventPayload<VRBalanceAlertEventPayload>(context.VRNotification);
            VRBalanceAlertRollbackEventPayload rollbackEventPayload =   null;
            if(context.VRNotification.RollbackEventPayload != null)
                rollbackEventPayload = context.VRNotification.RollbackEventPayload.CastWithValidate<VRBalanceAlertRollbackEventPayload>("context.VRNotification.RollbackEventPayload");
            var accountBalanceAlertRuleTypeSettings = new VRAlertRuleTypeManager().GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(vrBalanceAlertEventPayload.AlertRuleTypeId);

            AccountBalanceAlertRuleGetEntityNameContext accountBalanceAlertRuleGetEntityNameContext = new AccountBalanceAlertRuleGetEntityNameContext()
            {
                RuleTypeSettings = accountBalanceAlertRuleTypeSettings,
                EntityId = vrBalanceAlertEventPayload.EntityId
            };

            string accountType = new AccountTypeManager().GetAccountTypeName(accountBalanceAlertRuleTypeSettings.AccountTypeId);

            return new AccountBalanceNotificationDetailEventPayload()
            {
                AccountType = accountType,
                BusinessEntityDescription = new AccountBalanceAlertRuleBehavior().GetEntityName(accountBalanceAlertRuleGetEntityNameContext),
                CurrentBalance = vrBalanceAlertEventPayload.CurrentBalance,
                RollbackBalance = rollbackEventPayload != null ? rollbackEventPayload.CurrentBalance : (Decimal?)null,
                Threshold = vrBalanceAlertEventPayload.Threshold,
                Currency = new Vanrise.Common.Business.CurrencyManager().GetCurrencySymbol(vrBalanceAlertEventPayload.CurrencyId)
            };
        }
    }

    public class AccountBalanceNotificationDetailEventPayload : VRNotificationDetailEventPayload
    {
        public string AccountType { get; set; }

        public string BusinessEntityDescription { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal? RollbackBalance { get; set; }

        public Decimal Threshold { get; set; }

        public String Currency { get; set; }
    }
}
