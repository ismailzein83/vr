using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Business
{
    public class IcxSMS : IcxOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("AC8D489B-8E67-4F5D-A40F-E50E0B70653C"); }
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
