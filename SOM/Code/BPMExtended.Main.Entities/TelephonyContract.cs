﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephonyContract
    {
        public string TelephonyContractId { get; set; }

        public string PhoneNumber { get; set; }

        public string Status { get; set; }

        public DateTime ContractTime { get; set; }
    }
}
