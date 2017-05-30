using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Entities
{
    public class SwitchMappingDetail
    {
        public int CarrierAccountId { get; set; }
        public string CarrierAccountName { get; set; }

        public CarrierAccountType CarrierAccountType { get; set; }

        public string EndPointsDescription { get; set; }

        public string RoutesDescription { get; set; }

    }
}
