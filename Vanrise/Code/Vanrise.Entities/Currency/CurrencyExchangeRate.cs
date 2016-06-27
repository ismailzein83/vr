﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Entities
{
    public class CurrencyExchangeRate 
    {

        public long CurrencyExchangeRateId { get; set; }
        public int CurrencyId { get; set; }

        public Decimal Rate { get; set; }

        public DateTime ExchangeDate { get; set; }

        public string SourceId { get; set; }

    }

    public class ExchangeRateWithEED
    {
        public int CurrencyId { get; set; }

        public Decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
