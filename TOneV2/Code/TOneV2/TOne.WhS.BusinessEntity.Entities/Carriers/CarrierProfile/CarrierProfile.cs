using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierProfile
    {
        public int CarrierProfileId { get; set; }
        public string Name { get; set; }
        public CarrierProfileSettings Settings { get; set;  }
    }
    public class CarrierProfileSettings
    {

    }
}
