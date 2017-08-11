using System;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ChargeableEntityManager
    {
        public ChargeableEntitySettings GetChargeableEntitySettings(Guid chargeableEntityId)
        {
            GenericLKUPManager genericLKUPManager = new GenericLKUPManager();
            GenericLKUPItem genericLKUPItem = genericLKUPManager.GetGenericLKUPItem(chargeableEntityId);

            genericLKUPItem.ThrowIfNull("genericLKUPItem", chargeableEntityId);
            genericLKUPItem.Settings.ThrowIfNull("genericLKUPItem.Settings", chargeableEntityId);
            genericLKUPItem.Settings.ExtendedSettings.ThrowIfNull("genericLKUPItem.Settings.ExtendedSettings", chargeableEntityId);

            return genericLKUPItem.Settings.ExtendedSettings as ChargeableEntitySettings;
        }
    }
}
