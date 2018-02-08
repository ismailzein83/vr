using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.RateCalculation
{
    public class MarginRateCalculationMethod : RateCalculationMethod
    {
        #region Properties

        public decimal Margin { get; set; }

        public decimal MarginPercentage { get; set; }

        public MarginRateCalculationMethodType Type { get; set; }

        public Guid CostCalculationMethodConfigId { get; set; }

        public int RPRouteOptionNumber { get; set; }

        public int SupplierId { get; set; }

        #endregion

        public override Guid ConfigId { get { return new Guid("9848AF1F-0C8A-4236-B5CC-81D593B07B85"); } }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            decimal? rate = null;

            switch (Type)
            {
                case MarginRateCalculationMethodType.Cost:
                    {
                        if (context.ZoneItem.Costs != null)
                        {
                            int? costCalculationMethodIndex = context.GetCostCalculationMethodIndex(CostCalculationMethodConfigId);
                            if (costCalculationMethodIndex.HasValue)
                                rate = context.ZoneItem.Costs.ElementAt(costCalculationMethodIndex.Value);
                        }
                        break;
                    }
                case MarginRateCalculationMethodType.RPRouteOption:
                    {
                        if (context.ZoneItem.RPRouteDetail != null && context.ZoneItem.RPRouteDetail.RouteOptionsDetails != null)
                        {
                            if (RPRouteOptionNumber <= context.ZoneItem.RPRouteDetail.RouteOptionsDetails.Count())
                            {
                                RPRouteOptionDetail rpRouteOption = context.ZoneItem.RPRouteDetail.RouteOptionsDetails.ElementAt(RPRouteOptionNumber - 1);
                                rate = rpRouteOption.ConvertedSupplierRate;
                            }
                        }
                        break;
                    }
                case MarginRateCalculationMethodType.Supplier:
                    {
                        if (context.ZoneItem.RPRouteDetail != null && context.ZoneItem.RPRouteDetail.RouteOptionsDetails != null)
                        {
                            RPRouteOptionDetail rpRouteOption = context.ZoneItem.RPRouteDetail.RouteOptionsDetails.FindRecord(x => x.SupplierId == SupplierId);
                            if (rpRouteOption != null)
                                rate = rpRouteOption.ConvertedSupplierRate;
                        }
                        break;
                    }
            }

            if (rate.HasValue)
                context.Rate = ApplyMargin(rate.Value);
        }

        #region Private Methods

        private decimal ApplyMargin(decimal rate)
        {
            decimal marginPercentageRate = (MarginPercentage != 0) ? (rate+((MarginPercentage * rate) / 100)): rate;
            decimal marginRate = marginPercentageRate + Margin;
            return marginRate;
        }

        #endregion
    }

    public enum MarginRateCalculationMethodType
    {
        Cost = 0,
        RPRouteOption = 1,
        Supplier = 2
    }
}
