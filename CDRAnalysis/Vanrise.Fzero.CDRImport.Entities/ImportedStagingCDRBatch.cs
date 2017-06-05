using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;
using System.Linq;

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

        DateTime? _batchStart;
        DateTime? _batchEnd;

        public override DateTime GetBatchEnd()
        {
            if (!_batchEnd.HasValue)
            {
                if (StagingCDRs != null && StagingCDRs.Count > 0)
                    _batchEnd = this.StagingCDRs.Max(cdr => cdr.ConnectDateTime);
                else
                    _batchEnd = DateTime.Today;
            }
            return _batchEnd.Value;
        }

        public override DateTime GetBatchStart()
        {
            if (!_batchStart.HasValue)
            {
                if (StagingCDRs != null && StagingCDRs.Count > 0)
                    _batchStart = this.StagingCDRs.Min(cdr => cdr.ConnectDateTime);
                else
                    _batchStart = DateTime.Today;
            }
            return _batchStart.Value;
        }

        public override void SetBatchEnd(DateTime batchEnd)
        {
            _batchEnd = batchEnd;
        }

        public override void SetBatchStart(DateTime batchStart)
        {
            _batchStart = batchStart;
        }
    }
}
