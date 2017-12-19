﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public enum EnterpriseType { Enterprise = 0, Residential = 1 }
    public class TelesEnterpriseInfo
    {
        public string TelesEnterpriseId { get; set; }
        public string Name { get; set; }
        public EnterpriseType EnterpriseType { get; set; }
    }
}
