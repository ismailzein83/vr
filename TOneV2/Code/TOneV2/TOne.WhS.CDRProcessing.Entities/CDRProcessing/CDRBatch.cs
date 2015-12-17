using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRBatch : MappedBatchItem
    {
        static CDRBatch()
        {
            CDR cdr = new CDR();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRBatch), "CDRs", "DataSourceID");
        }
        public List<CDR> CDRs { get; set; }
        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} CDRs", CDRs.Count());
        }

        public override int GetRecordCount()
        {
            return CDRs.Count();
        }
    }
}
