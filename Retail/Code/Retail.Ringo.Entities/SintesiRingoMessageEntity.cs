using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class SintesiRingoMessageEntity
    {
        public string Operator { get; set; }
        public string Network { get; set; }
        public DateTime MessageDate { get; set; }
        public int NumberOfRows { get; set; }
        public int TotalTransferredCredit { get; set; }
    }
}
