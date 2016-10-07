﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItemsInput
    {
        public ZoneItemFilter Filter { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public int CurrencyId { get; set; }
    }

    public class ZoneItemFilter
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public char ZoneLetter { get; set; }
        public int RoutingDatabaseId { get; set; }
        public Guid PolicyConfigId { get; set; }
        public int NumberOfOptions { get; set; }
        public List<CostCalculationMethod> CostCalculationMethods { get; set; }
        public Guid? CostCalculationMethodConfigId { get; set; }
        public RateCalculationMethod RateCalculationMethod { get; set; }
        public IEnumerable<int> CountryIds { get; set; }
        public string ZoneNameFilter { get; set; }
        public Vanrise.Entities.TextFilterType? ZoneNameFilterType { get; set; }
    }
}
