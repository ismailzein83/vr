using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Jazz.Business
{
public class AccountCodeCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("A474BA09-750B-4CD7-B842-0BAA550FE108"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var accountCodeCarriers= context.FieldValue.CastWithValidate<AccountCodeCarriers>("context.FieldValue");
            List<string> carrierAccountsNames=null;
            if (accountCodeCarriers!=null && accountCodeCarriers.Carriers!=null && accountCodeCarriers.Carriers.Count > 0)
            {
                carrierAccountsNames = new List<string>();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                foreach(var carrierAccount in accountCodeCarriers.Carriers)
                {
                    carrierAccountsNames.Add(carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId));
                }
            }
            return string.Join(",", carrierAccountsNames);
        }
        public override bool IsMatched(IFieldCustomObjectTypeSettingsContext context)
        {
            var accountCodeCarriers = context.FieldValue.CastWithValidate<AccountCodeCarriers>("context.FieldValue");
            var accountCodeCarriersFilter = context.RecordFilter.CastWithValidate<AccountCodeCarriersRecordFilter>("context.RecordFilter");

            if (accountCodeCarriersFilter.CarriersIds == null || accountCodeCarriersFilter.CarriersIds.Count == 0)
                return true;

            if (accountCodeCarriers != null && accountCodeCarriers.Carriers != null && accountCodeCarriers.Carriers.Count > 0)
            {
                foreach (var carrierAccount in accountCodeCarriers.Carriers)
                {
                    if (accountCodeCarriersFilter.CarriersIds.Contains(carrierAccount.CarrierAccountId))
                        return true;
                }
            }
            return false;
        }
        public override string SelectorUIControl { get { return "whs-jazz-accountcode-customobject-filter"; } }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(AccountCodeCarriers);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as AccountCodeCarriers;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Account Code Custom Object";
        }
    }
 
}