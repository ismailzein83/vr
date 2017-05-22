﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DIDQuery
    {
        public string Number { get; set; }

        public List<DIDNumberType> DIDNumberTypes { get; set; }

        public List<long> AccountIds { get; set; }
    }
}
