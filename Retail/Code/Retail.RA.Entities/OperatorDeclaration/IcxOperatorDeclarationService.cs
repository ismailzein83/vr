using Retail.RA.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class IcxOperatorDeclarationService
    {
        public Decimal Revenue { get; set; }

        public IcxOperatorDeclarationServiceSettings Settings { get; set; }
    }

    public abstract class IcxOperatorDeclarationServiceSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract TrafficDirection GetTrafficDirection();
        public abstract ServiceType GetServiceType();

    }

    public class IcxOperatorDeclarationServices
    {
        public IcxOperatorDeclarationServicesCollection Services { get; set; }
    }
    public class IcxOperatorDeclarationServicesCollection : List<IcxOperatorDeclarationService>
    {
    }
    public struct IcxOperationDeclarationTrafficItem
    {
        public ServiceType ServiceType { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
    }
}
