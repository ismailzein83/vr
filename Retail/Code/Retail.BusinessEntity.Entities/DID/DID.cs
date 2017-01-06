using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DID
    {
        public int DIDId { get; set; }

        public string Number { get; set; }

        public DIDSettings Settings { get; set; }
    }
}
