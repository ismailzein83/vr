using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerTypeSettings
    {
        public bool CanBeRootDealer { get; set; }

        public List<Guid> SupportedParentDealerTypeIds { get; set; }

        public List<DealerTypeActionSettings> SupportedActions { get; set; }

        public Guid InitialStatusId { get; set; }

        public Guid? CreationActionDefinitionId { get; set; }

        public DealerTypeExtendedSettings ExtendedSettings { get; set; }   
    }

    public class DealerTypeActionSettings
    {
        public Guid ActionDefinitionId { get; set; }
    }
}
