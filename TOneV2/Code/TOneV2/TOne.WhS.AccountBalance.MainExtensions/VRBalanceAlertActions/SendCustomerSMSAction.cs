using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.AccountBalance.MainExtensions;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class SendCustomerSMSAction : VRAction
    {
        public Guid AccountSMSTemplateId { get; set; }

        public Guid ProfileSMSTemplateId { get; set; }
        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload eventPayload = context.EventPayload.CastWithValidate<VRBalanceAlertEventPayload>("eventPayload");

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();

            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(context.UserID);
            user.ThrowIfNull("user", context.UserID);
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();

            WHSFinancialAccount financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(eventPayload.EntityId));
            financialAccount.ThrowIfNull("financialAccount", eventPayload.EntityId);

            objects.Add("User", user);
            Decimal currentBalance;
            if (context.RollbackEventPayload != null)
                currentBalance = context.RollbackEventPayload.CastWithValidate<VRBalanceAlertRollbackEventPayload>("context.RollbackEventPayload").CurrentBalance;
            else
                currentBalance = eventPayload.CurrentBalance;
            objects.Add("AccountBalance", currentBalance);
            objects.Add("Threshold", eventPayload.Threshold);

            SMSManager smsManager = new SMSManager();
            if (financialAccount.CarrierAccountId.HasValue)
            {
                var carrierAccount = new CarrierAccountManager().GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                objects.Add("Customer", carrierAccount);
                smsManager.SendSMS(this.AccountSMSTemplateId, objects);
            }
            else
            {
                var carrierProfile = new CarrierProfileManager().GetCarrierProfile(financialAccount.CarrierProfileId.Value);
                objects.Add("Profile", carrierProfile);
                smsManager.SendSMS(this.ProfileSMSTemplateId, objects);
            }

        }
    }
}
