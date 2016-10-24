using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ImportedStagingCDRBatch : MappedBatchItem
    {
        static ImportedStagingCDRBatch()
        {
            StagingCDR dummy = new StagingCDR();//DO NOT REMOVE 
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedStagingCDRBatch), "StagingCDRs", "Datasource");           
        }

        public override string GenerateDescription()
        {
            return String.Format("Staging Imported CDR Batch of {0} Staging CDRs", StagingCDRs.Count);
        }

        public List<StagingCDR> StagingCDRs { get; set; }

        public Guid Datasource { get; set; }

        public override int GetRecordCount()
        {
            return StagingCDRs.Count;
        }
    }
}
