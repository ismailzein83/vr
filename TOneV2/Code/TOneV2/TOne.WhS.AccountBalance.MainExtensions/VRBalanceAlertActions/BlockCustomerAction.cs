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
            if (financialAccount.CarrierAccountId.HasValue)
            {
                var carrierAccount = _carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                BlockCustomer(carrierAccount, switches);
            }
            else
            {
                var carrierAccounts = _carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                foreach (var carrierAccount in carrierAccounts)
                {
                    BlockCustomer(carrierAccount, switches);
                }

            }
        }

        private void BlockCustomer(CarrierAccount carrierAccount, List<Switch> switches)
        {
            if (carrierAccount.CustomerSettings.RoutingStatus != RoutingStatus.Blocked)
            {
                CustomerRoutingStatusState customerRoutingStatusState = new Entities.CustomerRoutingStatusState
                {
                    OriginalRoutingStatus = carrierAccount.CustomerSettings.RoutingStatus,
                };
                _carrierAccountManager.UpdateCustomerRoutingStatus(carrierAccount.CarrierAccountId, RoutingStatus.Blocked, false);
                Dictionary<int, SwitchCustomerBlockingInfo> blockingInfoBySwitchId = BlockCustomerOnSwitches(switches, carrierAccount.CarrierAccountId);

                customerRoutingStatusState.BlockingInfoBySwitchId = blockingInfoBySwitchId;

                _carrierAccountManager.UpdateCarrierAccountExtendedSetting(carrierAccount.CarrierAccountId, customerRoutingStatusState);
            }
        }
        private Dictionary<int, SwitchCustomerBlockingInfo> BlockCustomerOnSwitches(List<Switch> switches, int carrierAccountId)
        {
            Dictionary<int, SwitchCustomerBlockingInfo> blockingInfo = new Dictionary<int, SwitchCustomerBlockingInfo>();
            if (switches != null)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                StringBuilder errorStringBuilder = new StringBuilder();
                foreach (var switchItem in switches)
                {
                    TryBlockCustomerContext context = new TryBlockCustomerContext
                    {
                        CustomerId = carrierAccountId.ToString()
                    };
                    try
                    {
                        if (switchItem.Settings.RouteSynchronizer.TryBlockCustomer(context))
                        {
                            blockingInfo.Add(switchItem.SwitchId,
                                new SwitchCustomerBlockingInfo() { SwitchBlockingInfo = context.SwitchBlockingInfo });

                            VRActionLogger.Current.LogObjectCustomAction(SwitchManager.SwitchLoggableEntity.Instance,
                                "Block Customer ", false, switchItem,
                                string.Format("Block Customer: {0}",
                                    _carrierAccountManager.GetCarrierAccountName(carrierAccountId)));
                        }
                    }
                    catch (Exception e)
                    {
                        string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
                        errorStringBuilder.AppendLine(string.Format("Couldn't Block Customer {0}.More Details: {1}",
                            carrierAccountName, e.Message));

                    }
                }
                if (errorStringBuilder.Length > 0)
                    throw new VRBusinessException(errorStringBuilder.ToString());
            }
            return blockingInfo;
        }
    }
}
