using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Entities;
namespace TOne.WhS.AccountBalance.MainExtensions
{
    public class SendCustomerEmailAction : VRAction
    {
        public Guid AccountMailTemplateId { get; set; }

        public Guid ProfileMailTemplateId { get; set; }

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
          
            VRMailManager mailManager = new VRMailManager();
            if(financialAccount.CarrierAccountId.HasValue)
            {
                var carrierAccount = new CarrierAccountManager().GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                objects.Add("Customer", carrierAccount);
                mailManager.SendMail(this.AccountMailTemplateId, objects);
            }else
            {
                var carrierProfile = new CarrierProfileManager().GetCarrierProfile(financialAccount.CarrierProfileId.Value);
                objects.Add("Profile", carrierProfile);
                mailManager.SendMail(this.ProfileMailTemplateId, objects);
            }
        }
        
    }
}
