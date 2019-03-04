using System;
using System.Collections.Generic;
using System.Text;
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
            var accountCodeCarriers= context.FieldValue.CastWithValidate<AccountCodeCarriers>("Account Code Carriers");
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