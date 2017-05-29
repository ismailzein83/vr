using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business.EventHandler
{
    public class ReevaluateCarrierProfileStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("16F5F10C-83AF-4A9B-821C-1FCC9C4D03EE"); }
        }

        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(eventPayload.CarrierAccountId);
            var profileCarrierAccounts = carrierAccountManager.GetCarriersByProfileId(carrierAccount.CarrierProfileId, true,true);
            if(profileCarrierAccounts != null)
            {   CarrierProfileActivationStatus status =  CarrierProfileActivationStatus.InActive;
                foreach(var profileCarrierAccount in profileCarrierAccounts)
                {
                    switch(carrierAccount.CarrierAccountSettings.ActivationStatus)
                    {
                        case ActivationStatus.Active: 
                            status = CarrierProfileActivationStatus.Active;  
                            break;
                    }
                    if (status == CarrierProfileActivationStatus.Active)
                        break;
                }
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                carrierProfileManager.ReevaluateCarrierProfileActivationStatus(carrierAccount.CarrierProfileId, status);
            }
        }
    }
}
