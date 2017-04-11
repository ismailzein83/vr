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

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class UnBlockCustomerAction : VRAction
    {
        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload eventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            eventPayload.ThrowIfNull("eventPayload", "");
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();

            FinancialAccount financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(eventPayload.EntityId));
            financialAccount.ThrowIfNull("financialAccount", eventPayload.EntityId);

            SwitchManager switchManager = new SwitchManager();
            var switches = switchManager.GetAllSwitches();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            if (financialAccount.CarrierAccountId.HasValue)
            {
                var carrierAccount = carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                CustomerRoutingStatusState customerRoutingStatusState = carrierAccountManager.GetExtendedSettings<CustomerRoutingStatusState>(carrierAccount);
                if (customerRoutingStatusState != null && carrierAccount.CustomerSettings.RoutingStatus == BusinessEntity.Entities.RoutingStatus.Blocked)
                {
                    UnBlockCustomer(carrierAccount.CarrierAccountId, customerRoutingStatusState.OriginalRoutingStatus);
                    UnBlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);
                }
            }
            else
            {
                var carrierAccounts = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                foreach (var carrierAccount in carrierAccounts)
                {
                    CustomerRoutingStatusState customerRoutingStatusState = carrierAccountManager.GetExtendedSettings<CustomerRoutingStatusState>(carrierAccount);
                    if (customerRoutingStatusState != null && carrierAccount.CustomerSettings.RoutingStatus == BusinessEntity.Entities.RoutingStatus.Blocked)
                    {
                        UnBlockCustomer(carrierAccount.CarrierAccountId, customerRoutingStatusState.OriginalRoutingStatus);
                        UnBlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);
                    }
                }
            }
        }
        private void UnBlockCustomer(int carrierAccountId, RoutingStatus routingStatus)
        {
            _carrierAccountManager.UpdateCustomerRoutingStatus(carrierAccountId, routingStatus,false);
            _carrierAccountManager.UpdateCarrierAccountExtendedSetting<CustomerRoutingStatusState>(carrierAccountId, null);
        }
        private void UnBlockCustomerOnSwitches(List<Switch> switches, int carrierAccountId)
        {
            if (switches != null)
            {
                foreach (var switchItem in switches)
                {
                    TryUnBlockCustomerContext context = new TryUnBlockCustomerContext
                    {
                        CustomerId = carrierAccountId.ToString()
                    };
                    if (switchItem.Settings.RouteSynchronizer.TryUnBlockCustomer(context))
                    {
                        VRActionLogger.Current.LogObjectCustomAction(SwitchManager.SwitchLoggableEntity.Instance, "UnBlock Customer ", false, switchItem, string.Format("UnBlock Customer: {0}", _carrierAccountManager.GetCarrierAccountName(carrierAccountId)));
                    }
                }
            }
        }

    }
}
