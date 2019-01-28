using System;
using System.Collections.Generic;
using System.Text;


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
}