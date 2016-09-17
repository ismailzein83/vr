using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourcePOS : ITargetBE
    {
        public PointOfSale PointOfSale { get; set; }

        public object SourceBEId
        {
            get { return PointOfSale.SourceId; }
        }

        public object TargetBEId
        {
            get { return PointOfSale.Id; }
        }
    }
}
