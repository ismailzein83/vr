using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Analytics.Business
{
    public partial class BillingStatisticManager
    {
        private readonly IBillingStatisticDataManager _datamanager;
        private readonly CarrierAccountManager _cmanager;

        public BillingStatisticManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            _cmanager = new CarrierAccountManager();
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, List<string> supplierIds, List<string> customerIds, int currencyId)
        {
            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, customerId, supplierId, currencyId));
        }


        #region Private Methods
        private ZoneProfitFormatted FormatZoneProfit(ZoneProfit zoneProfit)
        {
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return new ZoneProfitFormatted
            {
                CostZone = supplierZoneManager.GetSupplierZoneName(zoneProfit.SupplierZoneID), //zoneProfit.CostZone,
                SaleZone = saleZoneManager.GetSaleZoneName(zoneProfit.SaleZoneID),//zoneProfit.SaleZone,
                SupplierID = (zoneProfit.SupplierID != null) ? _cmanager.GetCarrierAccountName(zoneProfit.SupplierID) : null,
                CustomerID = (zoneProfit.CustomerId != null) ? _cmanager.GetCarrierAccountName(zoneProfit.CustomerId) : null,
                Calls = zoneProfit.Calls,

                DurationNet = zoneProfit.DurationNet,
                DurationNetFormated = FormatNumber(zoneProfit.DurationNet),

                SaleDuration = zoneProfit.SaleDuration,
                SaleDurationFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleDuration.HasValue) ? FormatNumber(zoneProfit.SaleDuration) : "0.00",

                SaleNet = zoneProfit.SaleNet,
                SaleNetFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleNet.HasValue) ? FormatNumberDigitRate(zoneProfit.SaleNet) : "0.00",

                CostDuration = zoneProfit.CostDuration,
                CostDurationFormated = FormatNumberDigitRate(zoneProfit.CostDuration),

                CostNet = zoneProfit.CostNet,
                CostNetFormated = (zoneProfit.CostNet.HasValue) ? FormatNumberDigitRate(zoneProfit.CostNet) : "0.00",

                Profit = zoneProfit.SaleNet == 0 ? "" : FormatNumber((!zoneProfit.SaleNet.HasValue) ? 0 : zoneProfit.SaleNet - zoneProfit.CostNet),
                ProfitSum = (!zoneProfit.SaleNet.HasValue || zoneProfit.SaleNet == 0) ? 0 : zoneProfit.SaleNet - zoneProfit.CostNet,
                ProfitPercentage = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleNet.HasValue) ? FormatNumber(((1 - zoneProfit.CostNet / zoneProfit.SaleNet)) * 100) : "-100%",
            };
        }

        private List<ZoneProfitFormatted> FormatZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormatted> models = new List<ZoneProfitFormatted>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(FormatZoneProfit(z));
                }
            return models;
        }
        private string FormatNumberDigitRate(Decimal? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }
        private string FormatNumberDigitRate(Double? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        private string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

        private string FormatNumber(int? number)
        {
            return String.Format("{0:#,###0}", number);
        }

        private string FormatNumberPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00%}", number);
        }
        private string FormatNumber(Double? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }
        #endregion

    }
}
