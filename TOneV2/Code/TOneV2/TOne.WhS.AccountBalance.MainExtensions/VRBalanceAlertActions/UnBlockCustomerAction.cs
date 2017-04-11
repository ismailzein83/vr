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

namespace TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions
{
    public class UnBlockCustomerAction : VRAction
    {
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
                    carrierAccount.CustomerSettings.RoutingStatus = customerRoutingStatusState.OriginalRoutingStatus;
                    customerRoutingStatusState = null;
                    carrierAccountManager.UpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount));
                    carrierAccountManager.UpdateCarrierAccountExtendedSetting(financialAccount.CarrierAccountId.Value, customerRoutingStatusState);
                    UnBlockCustomerOnSwitches(switches, financialAccount.CarrierAccountId.ToString());
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
                        carrierAccount.CustomerSettings.RoutingStatus = customerRoutingStatusState.OriginalRoutingStatus;
                        customerRoutingStatusState = null;
                        carrierAccountManager.UpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount));
                        carrierAccountManager.UpdateCarrierAccountExtendedSetting(financialAccount.CarrierAccountId.Value, customerRoutingStatusState);
                        UnBlockCustomerOnSwitches(switches, financialAccount.CarrierAccountId.ToString());
                    }
                }
            }
        }
        private CarrierAccountToEdit ConvertCarrierAccountToEdit(CarrierAccount carrierAccount)
        {
            return new CarrierAccountToEdit
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                SourceId = carrierAccount.SourceId,
                SupplierSettings = carrierAccount.SupplierSettings,
                CarrierAccountSettings = carrierAccount.CarrierAccountSettings,
                CustomerSettings = carrierAccount.CustomerSettings,
                NameSuffix = carrierAccount.NameSuffix,
                CreatedTime = carrierAccount.CreatedTime,

            };
        }
        private void UnBlockCustomerOnSwitches(List<Switch> switches, string customerId)
        {
            if (switches != null)
            {
                foreach (var switchItem in switches)
                {
                    TryUnBlockCustomerContext context = new TryUnBlockCustomerContext
                    {
                        CustomerId = customerId
                    };
                    if (switchItem.Settings.RouteSynchronizer.TryUnBlockCustomer(context))
                    {

                    }
                }
            }
        }

    }
}
