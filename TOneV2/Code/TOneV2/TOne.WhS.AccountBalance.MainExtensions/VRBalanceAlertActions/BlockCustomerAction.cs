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
    public class BlockCustomerAction : VRAction
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
                if(carrierAccount.CustomerSettings.RoutingStatus != RoutingStatus.Blocked)
                {
                    BlockCustomer(carrierAccount.CarrierAccountId, carrierAccount.CustomerSettings.RoutingStatus);
                    BlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);

                }
            }
            else
            {
                var carrierAccounts = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value,true,false);
                foreach (var carrierAccount in carrierAccounts)
                {
                    if (carrierAccount.CustomerSettings.RoutingStatus != RoutingStatus.Blocked)
                    {
                        BlockCustomer(carrierAccount.CarrierAccountId, carrierAccount.CustomerSettings.RoutingStatus);
                        BlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);
                    }
                }
               
            }
        }

        private void BlockCustomer(int carrierAccountId, RoutingStatus routingStatus)
        {
            CustomerRoutingStatusState customerRoutingStatusState = new Entities.CustomerRoutingStatusState
            {
                OriginalRoutingStatus = routingStatus
            };
            _carrierAccountManager.UpdateCustomerRoutingStatus(carrierAccountId, RoutingStatus.Blocked, false);
            _carrierAccountManager.UpdateCarrierAccountExtendedSetting(carrierAccountId, customerRoutingStatusState);
        }
        private void BlockCustomerOnSwitches(List<Switch> switches, int carrierAccountId)
        {
            if (switches != null)
            {
                foreach (var switchItem in switches)
                {
                    TryBlockCustomerContext context = new TryBlockCustomerContext
                    {
                        CustomerId = carrierAccountId.ToString()
                    };
                    if (switchItem.Settings.RouteSynchronizer.TryBlockCustomer(context))
                    {
                        VRActionLogger.Current.LogObjectCustomAction(SwitchManager.SwitchLoggableEntity.Instance, "Block Customer ", false, switchItem, string.Format("Block Customer: {0}", _carrierAccountManager.GetCarrierAccountName(carrierAccountId)));
                    }
                }
            }
        }
    }
}
