﻿using System;
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
    public class BlockCustomerAction : VRAction
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
                if(carrierAccount.CustomerSettings.RoutingStatus != BusinessEntity.Entities.RoutingStatus.Blocked)
                {
                    CustomerRoutingStatusState customerRoutingStatusState = new Entities.CustomerRoutingStatusState
                    {
                        OriginalRoutingStatus = carrierAccount.CustomerSettings.RoutingStatus
                    };
                    carrierAccount.CustomerSettings.RoutingStatus = BusinessEntity.Entities.RoutingStatus.Blocked;
                    carrierAccountManager.UpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount));
                    carrierAccountManager.UpdateCarrierAccountExtendedSetting(financialAccount.CarrierAccountId.Value, customerRoutingStatusState);
                    BlockCustomerOnSwitches(switches, financialAccount.CarrierAccountId.ToString());
                }
            }
            else
            {
                var carrierAccounts = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value,true,false);
                foreach (var carrierAccount in carrierAccounts)
                {
                    if (carrierAccount.CustomerSettings.RoutingStatus != BusinessEntity.Entities.RoutingStatus.Blocked)
                    {
                        CustomerRoutingStatusState customerRoutingStatusState = new Entities.CustomerRoutingStatusState
                        {
                            OriginalRoutingStatus = carrierAccount.CustomerSettings.RoutingStatus
                        };
                        carrierAccount.CustomerSettings.RoutingStatus = BusinessEntity.Entities.RoutingStatus.Blocked;
                        carrierAccountManager.UpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount));
                        carrierAccountManager.UpdateCarrierAccountExtendedSetting(financialAccount.CarrierAccountId.Value, customerRoutingStatusState);
                        BlockCustomerOnSwitches(switches, financialAccount.CarrierAccountId.ToString());
                    }
                }
               
            }
        }
        private void BlockCustomerOnSwitches(List<Switch> switches, string customerId)
        {
            if (switches != null)
            {
                foreach (var switchItem in switches)
                {
                    TryBlockCustomerContext context = new TryBlockCustomerContext
                    {
                        CustomerId = customerId
                    };
                    if (switchItem.Settings.RouteSynchronizer.TryBlockCustomer(context))
                    {

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
    }
}
