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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR), "MSISDN", "IMSI", "ConnectDateTime", "Destination", "DurationInSeconds",
                "DisconnectDateTime","Call_Class", "IsOnNet", "Call_Type", "Sub_Type", "IMEI", "BTS_Id", "Cell_Id",
                "SwitchRecordId", "Up_Volume", "Down_Volume", "Cell_Latitude", "Cell_Longitude", "In_Trunk", "Out_Trunk",
                "Service_Type", "Service_VAS_Name");
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", cdrs.Count);
        }

        public List<CDR> cdrs;

    }
}
