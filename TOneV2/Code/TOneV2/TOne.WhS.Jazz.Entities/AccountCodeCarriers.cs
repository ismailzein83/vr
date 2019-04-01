using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.GenericData.Entities;


namespace TOne.WhS.Jazz.Entities
{
public class AccountCodeCarriers
    {
        public List<AccountCodeCarrierAccount> Carriers { get; set; }

    }
    public class AccountCodeCarrierAccount
    {
        public int CarrierAccountId { get; set; }
    }

    public class AccountCodeCarriersRecordFilter : RecordFilter
    {
        public List<int> CarriersIds { get; set; }

        public ListRecordFilterOperator CompareOperator { get; set; }
        public override string GetDescription(IRecordFilterGetDescriptionContext context)
        {
            List<AccountCodeCarrierAccount> carriers = CarriersIds.MapRecords(x => new AccountCodeCarrierAccount { CarrierAccountId = x }).ToList();

            return string.Format("{0} {1} ( {2} )", context.GetFieldTitle(FieldName),Utilities.GetEnumDescription(CompareOperator), context.GetFieldValueDescription(FieldName, new AccountCodeCarriers
            {
                Carriers = carriers
            }));
        }

    }
}