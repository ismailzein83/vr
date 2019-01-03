using Retail.RA.Business;
using System;
using System.Collections.Generic;


namespace Retail.RA.Entities
{
    public class OperatorDeclarationService
    {
        public Decimal Revenue { get; set; }

        public OperatorDeclarationServiceSettings Settings { get; set; }
    }

    public abstract class OperatorDeclarationServiceSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract TrafficDirection GetTrafficDirection();
        public abstract ServiceType GetServiceType();

    }

    public class OperatorDeclarationServices
    {
        public OperatorDeclarationServicesCollection Services { get; set; }
    }
    public class OperatorDeclarationServicesCollection : List<OperatorDeclarationService>
    {
    }
    public struct OperationDeclarationTrafficItem
    {
        public ServiceType ServiceType { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
    }
}
