using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.CDRComparison.Business
{
    public class CDRRecordStorageSource : CDRSource
    {
        public override Guid ConfigId
        {
            get { return new Guid("D70BCB92-C0DB-45DA-9E7B-3F8059DA5873"); }
        }

        public override void ReadCDRs(IReadCDRsFromSourceContext context)
        {
           // throw new NotImplementedException();
        }

        public override CDRSample ReadSample(IReadSampleFromSourceContext context)
        {
            return null;
           // throw new NotImplementedException();
        }
        public List<Guid> DataRecordStorageIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }

    }
}
