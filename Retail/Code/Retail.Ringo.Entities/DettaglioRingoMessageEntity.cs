using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class DettaglioRingoMessageEntity
    {
        public string Operator { get; set; }
        public string Network { get; set; }
        public string RecipientRequestCode { get; set; }
        public int TransferredCredit { get; set; }
    }
}
