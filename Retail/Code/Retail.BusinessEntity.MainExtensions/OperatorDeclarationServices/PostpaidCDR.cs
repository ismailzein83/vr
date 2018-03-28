using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices
{
    public class PostpaidCDR : OperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("696FCB6C-EE81-4C34-A390-6793ECC7252D"); }
        }

        public TrafficType TrafficType { get; set; }

        public TrafficDirection TrafficDirection { get; set; }

        public long SuccessfulCalls { get; set; }

        public Decimal TotalDuration { get; set; }

        public Decimal TotalChargedDuration { get; set; }
    }
}
