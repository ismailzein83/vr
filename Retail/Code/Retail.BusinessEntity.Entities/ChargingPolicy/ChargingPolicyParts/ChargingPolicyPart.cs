﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyPart
    {
        public int PartTypeId { get; set; }

        public abstract string PartTypeName
        {
            get;
        }
    }
}
