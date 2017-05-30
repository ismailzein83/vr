using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace NP.IVSwitch.Entities
{
    public class SwitchMapping
    {
        public CarrierAccount CarrierAccount { get; set; }

        public List<int> EndPoints { get; set; }

        public List<int> Routes { get; set; }
    }
}
