using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterConnect.BusinessEntity.Entities
{
    public class OperatorAccount
    {
        public string Suffix { get; set; }

        public int ProfileId { get; set; }

        public OperatorAccountSettings Settings { get; set; }
    }
}
