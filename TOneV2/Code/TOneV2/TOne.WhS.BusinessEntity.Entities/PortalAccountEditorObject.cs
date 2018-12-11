using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PortalAccountEditorObject
    {
        public CarrierProfilePortalAccount Entity { get; set; }
        public int CarrierProfileId { get; set; }
        public List<int> GroupIds { get; set; }

    }
}
