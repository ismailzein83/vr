using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ImportedCDRBatch : PersistentQueueItem
    {
        static ImportedCDRBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedCDRBatch), "file");
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", file.Length);
        }

        public byte[] file;

    }
}
