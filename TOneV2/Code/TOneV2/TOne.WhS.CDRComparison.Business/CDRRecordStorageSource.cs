using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
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
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            if (this.DataRecordStorageIds != null && this.DataRecordStorageIds.Count > 0)
            {
                List<CDR> cdrs = new List<CDR>();
                foreach (var dataRecordStorageId in DataRecordStorageIds)
                {
                    dataRecordStorageManager.GetDataRecords(dataRecordStorageId, FromDate, ToDate, FilterGroup, () => { return false; }, (cdr) =>
                    {

                        cdrs.Add(new CDR
                        {
                            CDPN = cdr.CDPN,
                            IsPartnerCDR = false,
                            OriginalCDPN = cdr.OrigCDPN,
                            OriginalCGPN = cdr.OrigCGPN,
                            DurationInSec = cdr.DurationInSeconds,
                            CGPN = cdr.CGPN,
                            Time = cdr.AttemptDateTime
                        });
                        if (cdrs.Count >= 100000)
                        {
                            context.OnCDRsReceived(cdrs);
                            cdrs = new List<CDR>();
                        }
                    });
                }
                if (cdrs.Count > 0)
                {
                    context.OnCDRsReceived(cdrs);
                    cdrs = new List<CDR>();
                }
            }
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
