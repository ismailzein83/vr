using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ImportedCDRBatch : MappedBatchItem
    {
        static ImportedCDRBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedCDRBatch), "CDRs");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR), "MSISDN", "IMSI", "ConnectDateTime", "Destination", "DurationInSeconds",
                "DisconnectDateTime","CallClass", "IsOnNet", "CallType", "SubType", "IMEI", "BTSId", "CellId",
                "SwitchRecordId", "UpVolume", "DownVolume", "CellLatitude", "CellLongitude", "InTrunk", "OutTrunk",
                "ServiceType", "ServiceVASName");
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", CDRs.Count);
        }

        public List<CDR> CDRs { get; set; }

        public override int GetRecordCount()
        {
            return CDRs.Count;
        }
    }
}
