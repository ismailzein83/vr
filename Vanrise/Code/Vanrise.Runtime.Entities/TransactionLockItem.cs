using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class TransactionLockItem
    {
        public Guid LockItemUniqueId { get; set; }

        public string TransactionUniqueName { get; set; }

        public int ProcessId { get; set; }
    }
}
