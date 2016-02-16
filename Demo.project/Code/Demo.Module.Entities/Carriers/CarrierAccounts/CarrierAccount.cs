﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum CarrierAccountType { Exchange = 1, Supplier = 2, Customer = 3}

    public class CarrierAccount
    {
        public int CarrierAccountId { get; set; }
        public string NameSuffix { get; set; }
        public int CarrierProfileId { get; set; }
        public CarrierAccountSettings CarrierAccountSettings { get; set; } 
        public CarrierAccountSupplierSettings SupplierSettings { get; set; }
        public CarrierAccountCustomerSettings CustomerSettings { get; set; }
    }
}
