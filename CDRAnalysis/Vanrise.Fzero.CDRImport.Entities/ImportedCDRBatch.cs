using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;
using System.Linq;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ImportedCDRBatch : MappedBatchItem
    {
        static ImportedCDRBatch()
        {
            CDR dummy = new CDR();//DO NOT REMOVE 
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(ImportedCDRBatch), "CDRs", "Datasource");           
        }

        public override string GenerateDescription()
        {
            return String.Format("Imported CDR Batch of {0} CDRs", CDRs.Count);
        }

        public List<CDR> CDRs { get; set; }

        public Guid Datasource { get; set; }

        public override int GetRecordCount()
        {
            return CDRs.Count;
        }

        DateTime? _batchStart;
        DateTime? _batchEnd;

        public override DateTime GetBatchEnd()
        {
            if (!_batchEnd.HasValue)
            {
                if (CDRs != null && CDRs.Count > 0)
                    _batchEnd = this.CDRs.Max(cdr => cdr.ConnectDateTime);
                else
                    _batchEnd = DateTime.Today;
            }
            return _batchEnd.Value;
        }

        public override DateTime GetBatchStart()
        {
            if (!_batchStart.HasValue)
            {
                if (CDRs != null && CDRs.Count > 0)
                    _batchStart = this.CDRs.Min(cdr => cdr.ConnectDateTime);
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
