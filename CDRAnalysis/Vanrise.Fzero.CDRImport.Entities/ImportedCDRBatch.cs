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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedCDRBatch), "cdrs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR), "Source_Type", "Source_Name", "Source_File", 
                "Record_Type", "Call_Type", "IMEI","IMEI14", "Entity");
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", cdrs.Count);
        }

        public List<CDR> cdrs;

    }
}
