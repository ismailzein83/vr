﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class AccountEnterpriseDIDQuery
    {
        public List<long> AccountIds { get; set; }
        public string DIDNumber { get; set; }
    }
}
