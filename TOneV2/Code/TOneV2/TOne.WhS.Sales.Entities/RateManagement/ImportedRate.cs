﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RateManagement
{
    public enum RateChangeType { NotChanged = 0, New = 1, Increase = 2, Decrease = 3 }
    
    public class NewRate
    {
        public long ZoneId { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public RateChangeType ChangeType { get; set; }
        

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();
        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }
    }


    public class ExistingRate
    {
        public BusinessEntity.Entities.SaleRate RateEntity { get; set; }

        public ChangedRate ChangedRate { get; set; }

        public DateTime? EED
        {
            get { return ChangedRate != null ? ChangedRate.EED : RateEntity.EndEffectiveDate; }
        }
    }

    public class ChangedRate
    {
        public long RateId { get; set; }

        public DateTime EED { get; set; }
    }
}
