﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DIDDetail 
    {
        public string AccountName { get; set; }

        public string Description { get; set; }
        public DID Entity { get; set; }
    }
}
