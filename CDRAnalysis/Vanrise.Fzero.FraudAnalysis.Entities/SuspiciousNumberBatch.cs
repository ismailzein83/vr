using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SuspiciousNumberBatch: PersistentQueueItem
    {
        static SuspiciousNumberBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(SuspiciousNumberBatch), "suspiciousNumbers");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(SuspiciousNumber), "Number", "SuspectionLevel", "CriteriaValues");
        }

        public override string GenerateDescription()
        {
            return String.Format("Suspicious Number Batch of {0}", suspiciousNumbers.Count);
        }

        public List<SuspiciousNumber> suspiciousNumbers;
    }
}
