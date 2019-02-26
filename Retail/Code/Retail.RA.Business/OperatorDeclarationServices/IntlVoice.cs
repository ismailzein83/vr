using System;
using Retail.RA.Entities;

namespace Retail.RA.Business
{
    public class IntlVoice : IntlOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("696FCB6C-EE81-4C34-A390-6793ECC7252D"); }
        }
        public override TrafficDirection GetTrafficDirection()
        {
            return TrafficDirection;
        }
        public override ServiceType GetServiceType()
        {
            return ServiceType.Voice;
        }
        public TrafficDirection TrafficDirection { get; set; }

        public long DeclaredNumberOfCalls { get; set; }

        public Decimal DeclaredDuration { get; set; }
        public Decimal DeclaredRevenue { get; set; }
    }
}
