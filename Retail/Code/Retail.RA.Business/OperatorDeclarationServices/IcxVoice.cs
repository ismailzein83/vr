using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Business
{
    public class IcxVoice : IcxOperatorDeclarationServiceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8794E5D8-AA3C-469D-8C40-2ABEE07A55A4"); }
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
