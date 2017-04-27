﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueItem
    {
        public long ItemId { get; set; }

        public long ExecutionFlowTriggerItemId { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public byte[] Content { get; set; }
    }
}