using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ImportedCDRBatch : MappedBatchItem
    {
        static ImportedCDRBatch()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedCDRBatch), "CDRs", "Datasource");           
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", CDRs.Count);
        }

        public List<CDR> CDRs { get; set; }

        public int Datasource { get; set; }

        public override int GetRecordCount()
        {
            return CDRs.Count;
        }
    }
}
