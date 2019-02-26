using Retail.RA.Business;
using System;
using System.Collections.Generic;


namespace Retail.RA.Entities
{
    public class IntlOperatorDeclarationService
    {
        public Decimal Revenue { get; set; }

        public IntlOperatorDeclarationServiceSettings Settings { get; set; }
    }

    public abstract class IntlOperatorDeclarationServiceSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract TrafficDirection GetTrafficDirection();
        public abstract ServiceType GetServiceType();

    }

    public class IntlOperatorDeclarationServices
    {
        public IntlOperatorDeclarationServicesCollection Services { get; set; }
    }
    public class IntlOperatorDeclarationServicesCollection : List<IntlOperatorDeclarationService>
    {
    }
    public struct IntlOperationDeclarationTrafficItem
    {
        public ServiceType ServiceType { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
    }
}
