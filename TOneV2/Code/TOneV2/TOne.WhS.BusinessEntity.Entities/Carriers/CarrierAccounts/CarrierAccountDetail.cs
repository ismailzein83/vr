﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class CarrierAccountDetail
    {
       public string CarrierProfileName { get; set; }
       public string CarrierAccountName { get; set; }
       public string AccountTypeDescription{ get; set; }
       public string SellingNumberPlanName { get; set; }
       public string ActivationStatusDescription { get; set; }
       public CarrierAccount Entity { get; set; }
    }
}
