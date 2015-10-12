using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.LCR.Entities;
using TOne.Web.Online.Models;

namespace TOne.Web.Online.ModelMappers
{
    public class SaleZoneRateModelMapper
    {
        public static List<SaleZoneRateModel> MapSaleZoneRateModels(IEnumerable<SaleRate> saleRates)
        {
            List<SaleZoneRateModel> models = new List<SaleZoneRateModel>();
            if (saleRates != null && saleRates.ToList().Count > 0)
                foreach (var saleRate in saleRates)
                {
                    models.Add(MapSaleZoneRateModel(saleRate));
                }
            return models;
        }
        private static SaleZoneRateModel MapSaleZoneRateModel(SaleRate saleRate)
        {
            return new SaleZoneRateModel()
            {
                CurrencyId = saleRate.Currency.CurrencyID,
                FlaggedSericeId = saleRate.OurServicesFlag.FlaggedServiceID,
                ServiceColor = saleRate.OurServicesFlag.ServiceColor,
                Symbol = saleRate.OurServicesFlag.Symbol,
                SaleRate = saleRate.Rate,
                ZoneId = saleRate.ZoneId,
                ZoneName = saleRate.ZoneName,
                PriceListId = saleRate.PriceListId,
                SupplierRates = MapSupplierRatesModel(saleRate.SupplierZoneRates)
            };
        }
        private static List<SupplierZoneRateModel> MapSupplierRatesModel(List<SupplierZoneRate> supplierZoneRates)
        {
            List<SupplierZoneRateModel> supplierZoneRateModels = new List<SupplierZoneRateModel>();
            foreach (var supplierZoneRate in supplierZoneRates)
                supplierZoneRateModels.Add(MapSupplierRateModel(supplierZoneRate));
            return supplierZoneRateModels;
        }
        private static SupplierZoneRateModel MapSupplierRateModel(SupplierZoneRate supplierZoneRate)
        {
            return new SupplierZoneRateModel()
            {
                FlaggedServiceId = supplierZoneRate.SupplierServicesFlag.FlaggedServiceID,
                ServiceColor = supplierZoneRate.SupplierServicesFlag.ServiceColor,
                SupplierName = supplierZoneRate.SupplierName,
                SupplierRate = supplierZoneRate.Rate,
                Symbol = supplierZoneRate.SupplierServicesFlag.Symbol,
                ZoneName = supplierZoneRate.ZoneName
            };
        }
    }
}