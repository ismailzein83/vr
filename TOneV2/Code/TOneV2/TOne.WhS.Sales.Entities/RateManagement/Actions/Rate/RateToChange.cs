﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RateToChange : Vanrise.Entities.IDateEffectiveSettings
    {
        public string ZoneName { get; set; }

        public Decimal NormalRate { get; set; }

        public Decimal? RecentNormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public RateChangeType ChangeType { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get
            {
                return _newRates;
            }
        }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();

        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }
    }

    public enum RateChangeType
    {
        New = 0,
        Increase = 1,
        Decrease = 2,
        Close = 3
    }
}
