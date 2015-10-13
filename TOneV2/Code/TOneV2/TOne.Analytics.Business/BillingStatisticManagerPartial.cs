using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Business
{
    public partial class BillingStatisticManager
    {

        public List<CustomerRoutingFormatted> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, List<string> customerIds, List<string> supplierIds, string currencyId)
        {

            List<CustomerRouting> list = _datamanager.GetCustomerRouting(fromDate, toDate, customerId, supplierId, customerIds, supplierIds, currencyId);
           var result = list.GroupBy(s => new { s.CustomerID, s.SupplierID, s.SaleZone, s.CostZone })
           .Select(s =>
               new
               {
                   Customer = _bemanager.GetCarrirAccountName(s.Key.CustomerID),
                   Destination = _bemanager.GetZoneName(s.Key.SaleZone),
                   Sale_Duration = s.Sum(k => k.SaleDuration),
                   Sale_Rate = s.Sum(k => k.SaleRate * (double)k.SaleDuration) / (double)s.Sum(k => k.SaleDuration), 
                   Supplier = _bemanager.GetCarrirAccountName(s.Key.SupplierID) ,
                   CostZone = _bemanager.GetZoneName(s.Key.CostZone),
                   Cost_Duration = s.Sum(k => k.CostDuration),
                   Cost_Rate = s.Sum(k => k.CostRate * (double)k.CostDuration) / (double)s.Sum(k => k.CostDuration),
                   Profit = ((double)s.Sum(k => k.SaleDuration) * s.Average(k => k.SaleRate)) != 0 ? (((double)s.Sum(k => k.SaleDuration) * s.Average(k => k.SaleRate)) - ((double)s.Sum(k => k.CostDuration) * s.Average(k => k.CostRate))) : 0,
                   Profit_Perc = ((double)s.Sum(k => k.SaleDuration) * s.Average(k => k.SaleRate)) != 0 ? (((double)s.Sum(k => k.SaleDuration) * s.Average(k => k.SaleRate)) - ((double)s.Sum(k => k.CostDuration) * s.Average(k => k.CostRate))) * 100 / ((double)s.Sum(k => k.SaleDuration) * s.Average(k => k.SaleRate)) : 0

               }
           ).OrderBy(s => s.Customer + s.Destination + s.Supplier + s.CostZone).ToList();

           var resultGrouped = result
               .GroupBy(s => new { s.Customer, s.Supplier, s.Destination })
               .Select(s =>
                   new CustomerRoutingFormatted
                   {
                       Customer = s.Key.Customer,
                       Destination = s.Key.Destination,
                       SaleDuration = FormatNumber(s.Sum(k => k.Sale_Duration)),
                       SaleRate = FormatNumberDigitRate(s.Sum(k => k.Sale_Rate * (double)k.Sale_Duration) / (double)s.Sum(k => k.Sale_Duration)),
                       Supplier = s.Key.Supplier,
                       CostDuration = FormatNumber(s.Sum(k => k.Cost_Duration)),
                       CostRate = FormatNumberDigitRate(s.Sum(k => k.Cost_Rate * (double)k.Cost_Duration) / (double)s.Sum(k => k.Cost_Duration)),
                       Profit = FormatNumber( s.Sum(k => k.Profit)),
                       ProfitPerc = FormatNumberPercentage(s.Average(k => k.Profit_Perc))
                   }).OrderBy(s => s.Customer + s.Destination + s.Supplier).ToList();

           return resultGrouped ;

        }

        public List<RoutingAnalysisFormatted> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, List<string> customerIds, List<string> supplierIds, string currencyId)
        {

            return FormatRoutingsAnalyics(_datamanager.GetRoutingAnalysis(fromDate, toDate, customerId, supplierId, top, customerIds, supplierIds, currencyId));
        }

        #region PrivateMethode
        private List<CustomerSummaryFormatted> FormatCustomerSummaries(List<CustomerSummary> zoneSummariesDetailed)
        {
            List<CustomerSummaryFormatted> models = new List<CustomerSummaryFormatted>();
            if (zoneSummariesDetailed != null)
                foreach (var z in zoneSummariesDetailed)
                {
                    models.Add(FormatCustomerSummary(z));
                }
            return models;
        }

        private List<RoutingAnalysisFormatted> FormatRoutingsAnalyics(List<RoutingAnalysis> routingAnalytics)
        {
            List<RoutingAnalysisFormatted> models = new List<RoutingAnalysisFormatted>();
            if (routingAnalytics != null)
                foreach (var z in routingAnalytics)
                {
                    models.Add(FormatRoutingAnalysis(z));
                }
            return models;
        }

        
        private CustomerSummaryFormatted FormatCustomerSummary(CustomerSummary customerSummary)
        {
            return new CustomerSummaryFormatted
            {
                Carrier = customerSummary.Carrier,
                SaleDuration = customerSummary.SaleDuration,
                SaleNet = customerSummary.SaleNet,
                CostDuration = customerSummary.CostDuration,
                CostNet = customerSummary.CostNet,
                SaleDurationFormatted = FormatNumber(customerSummary.SaleDuration),
                SaleNetFormatted = FormatNumber(customerSummary.SaleNet),
                CostDurationFormatted = FormatNumber(customerSummary.CostDuration),
                CostNetFormatted = FormatNumber(customerSummary.CostNet),
                ProfitFormatted = FormatNumber((customerSummary.SaleNet - customerSummary.CostNet)),
                ProfitPercentageFormatted = FormatNumber((customerSummary.SaleNet - customerSummary.CostNet) / customerSummary.SaleNet)
            };
        }

        private RoutingAnalysisFormatted FormatRoutingAnalysis(RoutingAnalysis obj)
        {
            return new RoutingAnalysisFormatted
            {
                SaleZoneID = obj.SaleZoneID,
                SaleZone = _bemanager.GetZoneName(obj.SaleZoneID),
               
                SupplierID = obj.SupplierID,
                Supplier = (obj.SupplierID != null) ? _bemanager.GetCarrirAccountName(obj.SupplierID) : null,

                SaleNet = obj.SaleNet,
                SaleNetFormatted = FormatNumber(obj.SaleNet),

                CostNet = obj.CostNet,
                CostNetFormatted = FormatNumber(obj.CostNet),

                Duration = obj.Duration ,
                DurationFormatted = FormatNumber(obj.Duration),

                ACD = obj.ACD ,
                ACDFormatted = FormatNumberDigitRate(obj.ACD),


                ASR = obj.ASR,
                ASRFormatted = FormatNumberDigitRate(obj.ASR),

                Profit = (double)((double)obj.SaleNet - (double)obj.CostNet),
                ProfitFormatted = FormatNumber((double)obj.SaleNet - (double)obj.CostNet),
                AVGCost = ((double)obj.Duration == 0 || (double)obj.CostNet == 0) ? 0 : (double)((double)obj.CostNet / (double)obj.Duration),
                AVGCostFormatted = ((double)obj.Duration == 0 || (double)obj.CostNet == 0) ? "0" : FormatNumberDigitRate((double)((double)obj.CostNet / (double)obj.Duration)),
                AVGSale = ((double)obj.Duration == 0 || (double)obj.SaleNet == 0) ? 0 : (double)((double)obj.SaleNet / (double)obj.Duration),
                AVGSaleFormatted = ((double)obj.Duration == 0 || (double)obj.SaleNet == 0) ? "0" : FormatNumberDigitRate((double)((double)obj.SaleNet / (double)obj.Duration)),
            };
        }
        #endregion 
        

     
    }
}
