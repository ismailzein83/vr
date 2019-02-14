using System;
using System.Collections.Generic;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class DraftData
    {
        public long? ProcessDraftID { get; set; }
        public List<char> CountryLetters { get; set; }
        public DateTime? DraftEffectiveDate { get; set; }
        public int PendingChanges { get; set; }
    }
}
