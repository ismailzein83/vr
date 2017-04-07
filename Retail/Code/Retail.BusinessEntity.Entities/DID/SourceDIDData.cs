using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourceDIDData : ITargetBE
    {
        public DID DID { get; set; }
        public long? AccountId { get; set; }
        public DateTime? BED { get; set; }
        public object SourceBEId
        {
            get { return this.DID.SourceId; }
        }

        public object TargetBEId
        {
            get { return this.DID.DIDId; }
        }
    }
}
