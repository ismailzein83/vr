using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class DraftData
    {
        public List<char> CountryLetters { get; set; }

        public DateTime? DraftEffectiveDate { get; set; }
    }
}
