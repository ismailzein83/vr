using System;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices
{
    public class Voice : OperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("696FCB6C-EE81-4C34-A390-6793ECC7252D"); }
        }

        public TrafficType TrafficType { get; set; }

        public TrafficDirection TrafficDirection { get; set; }

        public long NumberOfCalls { get; set; }

        public Decimal Duration { get; set; }
        public Decimal Amount { get; set; }

    }
}
