﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class MediationRecordBatch
    {
        public MediationRecordBatch()
        {
            this.MediationRecords = new List<MediationRecord>();
        }

        public string SessionId { get; set; }

        public List<MediationRecord> MediationRecords { get; set; }
        public long LastCommittedId { get; set; }

        public bool IsTimedOut { get; set; }
    }
}
