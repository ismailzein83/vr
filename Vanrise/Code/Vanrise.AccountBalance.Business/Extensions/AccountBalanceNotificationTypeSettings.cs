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

            var accountBalanceNotificationTypeIsMatchedContext = new AccountBalanceNotificationTypeIsMatchedContext()
            {
                VRNotification = context.VRNotification,
                AccountBalanceNotificationExtendedQuery = accountBalanceNotificationQuery.AccountBalanceNotificationExtendedQuery
            };
            return AccountBalanceNotificationTypeExtendedSettings.IsVRNotificationMatched(accountBalanceNotificationTypeIsMatchedContext);
        }

        public override VRNotificationDetailEventPayload GetNotificationDetailEventPayload(IVRNotificationTypeGetNotificationEventPayloadContext context)
        {
            VRBalanceAlertEventPayload eventPayload = new VRNotificationManager().GetVRNotificationEventPayload<VRBalanceAlertEventPayload>(context.VRNotification);

            AccountBalanceAlertRuleGetEntityNameContext accountBalanceAlertRuleGetEntityNameContext = new AccountBalanceAlertRuleGetEntityNameContext()
            {
                RuleTypeSettings = new VRAlertRuleTypeManager().GetVRAlertRuleTypeSettings<VRBalanceAlertRuleTypeSettings>(eventPayload.AlertRuleTypeId),
                EntityId = eventPayload.EntityId
            };

            return new AccountBalanceNotificationDetailEventPayload()
            {
                BusinessEntityDescription = new AccountBalanceAlertRuleBehavior().GetEntityName(accountBalanceAlertRuleGetEntityNameContext),
                CurrentBalance = eventPayload.CurrentBalance,
                Threshold = eventPayload.Threshold,
                Currency = new Vanrise.Common.Business.CurrencyManager().GetCurrencySymbol(eventPayload.CurrencyId)
            };
        }
    }

    public class AccountBalanceNotificationDetailEventPayload : VRNotificationDetailEventPayload
    {
        public string BusinessEntityDescription { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal Threshold { get; set; }

        public String Currency { get; set; }
    }
}
