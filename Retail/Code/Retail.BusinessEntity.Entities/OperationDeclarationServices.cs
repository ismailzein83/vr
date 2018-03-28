using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
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
        public List<OperatorDeclarationService> Services { get; set; }
    }
}
