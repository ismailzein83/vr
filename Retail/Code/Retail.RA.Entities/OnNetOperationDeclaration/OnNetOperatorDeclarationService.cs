using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class OnNetOperatorDeclarationServices
    {
        public OnNetOperatorDeclarationServicesCollection Services { get; set; }
    }
    public abstract class OnNetOperatorDeclarationServiceSettings
    {
        public abstract Guid ConfigId { get; }
    }
    public class OnNetOperatorDeclarationService
    {
        public Decimal Revenue { get; set; }
        public OnNetOperatorDeclarationServiceSettings Settings { get; set; }
    }
    public class OnNetOperatorDeclarationServicesCollection : List<OnNetOperatorDeclarationService>
    {
    }
}
