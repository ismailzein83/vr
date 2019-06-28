using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
	public class UnBlockCustomerAction : VRAction
	{
		CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

		public override void Execute(IVRActionExecutionContext context)
		{
			VRBalanceAlertEventPayload eventPayload = context.EventPayload as VRBalanceAlertEventPayload;
			eventPayload.ThrowIfNull("eventPayload", "");
			WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();

			WHSFinancialAccount financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(eventPayload.EntityId));
			financialAccount.ThrowIfNull("financialAccount", eventPayload.EntityId);

			SwitchManager switchManager = new SwitchManager();
			var switches = switchManager.GetAllSwitches();

			if (financialAccount.CarrierAccountId.HasValue)
			{
				var carrierAccount = _carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
				UnBlockCustomer(carrierAccount, switches);
			}
			else
			{
				var carrierAccounts = _carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
				if (carrierAccounts != null)
				{
					foreach (var carrierAccount in carrierAccounts)
					{
						UnBlockCustomer(carrierAccount, switches);
					}
				}
			}
		}
		private void UnBlockCustomer(CarrierAccount carrierAccount, List<Switch> switches)
		{
			if (carrierAccount.CustomerSettings.RoutingStatus != BusinessEntity.Entities.RoutingStatus.Enabled)
			{
				_carrierAccountManager.UpdateAccountRoutingStatus(carrierAccount.CarrierAccountId, RoutingStatus.Enabled, false);
			}
		}
	}
}
