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
            CustomerRoutingStatusState customerRoutingStatusState = _carrierAccountManager.GetExtendedSettings<CustomerRoutingStatusState>(carrierAccount);
            if (customerRoutingStatusState != null && carrierAccount.CustomerSettings.RoutingStatus == BusinessEntity.Entities.RoutingStatus.Blocked)
            {
                _carrierAccountManager.UpdateCustomerRoutingStatus(carrierAccount.CarrierAccountId, customerRoutingStatusState.OriginalRoutingStatus, false);
                UnBlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);
                _carrierAccountManager.UpdateCarrierAccountExtendedSetting<CustomerRoutingStatusState>(carrierAccount.CarrierAccountId, null);
            }
        }
        private void UnBlockCustomerOnSwitches(List<Switch> switches, int carrierAccountId)
        {
            if (switches != null)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                StringBuilder errorStringBuilder = new StringBuilder();
                CustomerRoutingStatusState routingStatusState = _carrierAccountManager.GetExtendedSettings<CustomerRoutingStatusState>(carrierAccountId);
                foreach (var switchItem in switches)
                {
                    SwitchCustomerBlockingInfo blockingInfo = null;
                    routingStatusState.BlockingInfoBySwitchId.TryGetValue(switchItem.SwitchId, out blockingInfo);

                    if (blockingInfo == null)
                        continue;

                    TryUnBlockCustomerContext context = new TryUnBlockCustomerContext
                    {
                        CustomerId = carrierAccountId.ToString(),
                        SwitchBlockingInfo = blockingInfo.SwitchBlockingInfo
                    };
                    try
                    {

                        if (switchItem.Settings.RouteSynchronizer.TryUnBlockCustomer(context))
                        {
                            VRActionLogger.Current.LogObjectCustomAction(SwitchManager.SwitchLoggableEntity.Instance, "UnBlock Customer ", false, switchItem, string.Format("UnBlock Customer: {0}", _carrierAccountManager.GetCarrierAccountName(carrierAccountId)));
                        }
                    }
                    catch (Exception e)
                    {
                        string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
                        errorStringBuilder.AppendLine(string.Format("Couldn't Unblock Customer {0}.More Details: {1}",
                            carrierAccountName, e.Message));

                    }
                }
                if (errorStringBuilder.Length > 0)
                    throw new VRBusinessException(errorStringBuilder.ToString());
            }
        }

    }
}
