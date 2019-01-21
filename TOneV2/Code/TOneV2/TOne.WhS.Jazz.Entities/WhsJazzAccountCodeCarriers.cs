using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
public class WhsJazzAccountCodeCarriers
    {
        public List<WhsJazzAccountCodeCarrierAccount> Carriers { get; set; }

    }
    public class WhsJazzAccountCodeCarrierAccount
    {
        public int CarrierAccountId { get; set; }
    }
}