﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItem
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }

        #region Rate Properties
        public long? CurrentRateId { get; set; }
        public Decimal? CurrentRate { get; set; }
        public DateTime? CurrentRateBED { get; set; }
        public DateTime? CurrentRateEED { get; set; }
        public DateTime? CurrentRateNewEED { get; set; }
        public bool? IsCurrentRateEditable { get; set; }
        public Dictionary<int, OtherRate> CurrentOtherRates { get; set; }
        public Decimal? NewRate
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.NormalRate;
                return null;
            }
        }
        public IEnumerable<DraftRateToChange> NewRates { get; set; }
        public DateTime? NewRateBED
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.BED;
                return null;
            }
        }
        public DateTime? NewRateEED
        {
            get
            {
                DraftRateToChange newNormalRate = NewRates.FindRecord(x => x.RateTypeId == null);
                if (newNormalRate != null)
                    return newNormalRate.EED;
                return null;
            }
        }
        public IEnumerable<DraftRateToClose> ClosedRates { get; set; }
        public decimal? CalculatedRate { get; set; }
        #endregion

        #region Routing Product Properties
        public int? CurrentRoutingProductId { get; set; }
        public string CurrentRoutingProductName { get; set; }
        public DateTime? CurrentRoutingProductBED { get; set; }
        public DateTime? CurrentRoutingProductEED { get; set; }
        public bool? IsCurrentRoutingProductEditable { get; set; }
        public int? NewRoutingProductId { get; set; }
        public DateTime? NewRoutingProductBED { get; set; }
        public DateTime? NewRoutingProductEED { get; set; }
        public DateTime? RoutingProductChangeEED { get; set; }
        public int EffectiveRoutingProductId { get; set; }
        public string EffectiveRoutingProductName { get; set; }
        #endregion

        #region Route Option Properties
        public IEnumerable<RPRouteOptionDetail> RouteOptions { get; set; }
        public List<decimal?> Costs { get; set; }
        #endregion
    }

    public class OtherRate
    {
        public int RateTypeId { get; set; }
        public decimal Rate { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
