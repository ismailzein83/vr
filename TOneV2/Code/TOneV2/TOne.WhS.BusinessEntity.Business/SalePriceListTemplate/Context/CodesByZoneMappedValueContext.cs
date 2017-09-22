﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodesByZoneMappedValueContext : ICodesByZoneMappedValueContext
	{
        public SalePLZoneNotification ZoneNotification { get; set; }

        public char Delimiter { get; set; }
        public bool HasCodeRange { get; set; }
        public char RangeSeparator { get; set; }
        public bool IsCommaDecimalSeparator { get; set; }
        public object Value { get; set; }
        public int CustomerId { get; set; }
	}
}
