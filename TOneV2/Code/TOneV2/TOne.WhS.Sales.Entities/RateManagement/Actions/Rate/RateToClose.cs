﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RateToClose
    {
        public string ZoneN0ame { get; set; }

        public DateTime CloseEffectiveDate { get; set; }

        List<ExistingRate> _changedExistingRates = new List<ExistingRate>();

        public List<ExistingRate> ChangedExistingRates
        {
            get
            {
                return _changedExistingRates;
            }
        }
    }
}
