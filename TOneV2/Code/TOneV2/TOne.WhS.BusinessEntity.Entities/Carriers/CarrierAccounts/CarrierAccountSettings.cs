using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum ActivationStatus { Active = 0, Inactive = 1, Testing = 2 }
    public class CarrierAccountSettings
    {
        public ActivationStatus ActivationStatus { get; set; } 
    }
}
