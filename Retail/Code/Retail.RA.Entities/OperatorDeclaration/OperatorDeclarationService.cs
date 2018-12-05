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
    }

    public class OperatorDeclarationServices
    {
        public OperatorDeclarationServicesCollection Services { get; set; }
    }
    public class OperatorDeclarationServicesCollection : List<OperatorDeclarationService>
    {
    }
}
