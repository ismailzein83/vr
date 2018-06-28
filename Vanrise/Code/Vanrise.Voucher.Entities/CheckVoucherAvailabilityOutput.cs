﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public class CheckVoucherAvailabilityOutput
    {
        public bool IsAvailable { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
    }

}
