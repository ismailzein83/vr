using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.BusinessEntity.Business
{
public class WhsJazzAccountCodeCarriers
    {
        public List<WhsJazzAccountCodeCarrierAccount> Carriers { get; set; }

    }
    public class WhsJazzAccountCodeCarrierAccount
    {
        public Guid WhsJazzCarrierAccountId { get; set; }
    }
}