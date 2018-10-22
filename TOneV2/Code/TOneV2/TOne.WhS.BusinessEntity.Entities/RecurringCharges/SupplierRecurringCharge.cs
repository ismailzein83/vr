﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRecurringCharge
    {
        public long ID { get; set; }
        public long RecurringChargeTypeId { get; set; }
        public int FinancialAccountId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public RecurringChargePeriod RecurringChargePeriod { get; set; }
        public int? DuePeriod { get; set; }
        public IEnumerable<SupplierRecurringCharge> GetSupplierRecurringChargeRDLCSchema()
        {
            return null;
        }
    }
}
