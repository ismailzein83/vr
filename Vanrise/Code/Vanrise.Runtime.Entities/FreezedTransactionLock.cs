﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class FreezedTransactionLock
    {
        public long FreezedTransactionLockId { get; set; }

        public List<Guid> TransactionLockItemIds { get; set; }
    }
}
