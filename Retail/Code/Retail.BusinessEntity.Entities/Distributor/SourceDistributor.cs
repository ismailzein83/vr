using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourceDistributor : ITargetBE
    {
        public Distributor Distributor { get; set; }

        public object SourceBEId
        {
            get { return Distributor.SourceId; }
        }

        public object TargetBEId
        {
            get { return Distributor.Id; }
        }
    }
}
