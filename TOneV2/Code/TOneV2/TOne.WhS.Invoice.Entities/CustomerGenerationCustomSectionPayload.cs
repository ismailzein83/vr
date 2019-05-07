﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class BaseGenerationCustomSectionPayload
    {
        public int? TimeZoneId { get; set; }
        public decimal? Commission { get; set; }
		public decimal? Adjustment { get; set; }
        public CommissionType CommissionType { get; set; }
    }
    public class CustomerGenerationCustomSectionPayload: BaseGenerationCustomSectionPayload
    {
        
    }
}