using System;
using Retail.RA.Entities;

namespace Retail.RA.Business
{
    public class SMS : OperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0F35BD74-81D4-4CF3-950D-98DE8CDAD7D9"); }
        }
        public override TrafficDirection GetTrafficDirection()
        {
            return TrafficDirection;
        }
        public override ServiceType GetServiceType()
        {
            return ServiceType.SMS;
        }
        public TrafficType TrafficType { get; set; }

        public TrafficDirection TrafficDirection { get; set; }

        public long NumberOfSMSs { get; set; }

        public Decimal Revenue { get; set; }
    }
}
