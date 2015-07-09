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

        public List<CustomerRoutingFormatted> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId,  int? supplierAMUId, int? customerAMUId)
        {

           List<CustomerRouting> list = _datamanager.GetCustomerRouting(fromDate, toDate, customerId, supplierId, supplierAMUId, customerAMUId);
           var result = list.GroupBy(s => new { s.CustomerID, s.SupplierID, s.SaleZone, s.CostZone })
           .Select(s =>
               new
               {
                   Customer = _bemanager.GetCarrirAccountName(s.Key.CustomerID),
                   Destination = _bemanager.GetZoneName(s.Key.SaleZone),
                   Sale_Duration = s.Sum(k => k.SaleDuration),
                   Sale_Rate = s.Sum(k => k.SaleRate * (double)k.SaleDuration) / (double)s.Sum(k => k.SaleDuration), 
                   Supplier = _bemanager.GetCarrirAccountName(s.Key.SupplierID) ,
                   CostZone = s.Key.CostZone != null ? _bemanager.GetZoneName(s.Key.CostZone) : null,
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
                       SaleDuration = string.Format("{0:#,##0.00}", s.Sum(k => k.Sale_Duration)),
                       SaleRate = string.Format("{0:0.0000}", (s.Sum(k => k.Sale_Rate * (double) k.Sale_Duration) / (double)s.Sum(k => k.Sale_Duration))),
                       Supplier = s.Key.Supplier,
                       CostDuration = string.Format("{0:#,##0.00}", s.Sum(k => k.Cost_Duration)),
                       CostRate = string.Format("{0:0.0000}", (s.Sum(k => k.Cost_Rate * (double)k.Cost_Duration) / (double)s.Sum(k => k.Cost_Duration))),
                       Profit = FormatNumber( s.Sum(k => k.Profit)),
                       ProfitPerc = string.Format("{0:#,##0.00}%", s.Average(k => k.Profit_Perc))
                   }).OrderBy(s => s.Customer + s.Destination + s.Supplier).ToList();

           return resultGrouped ;
        }

        public List<RoutingAnalysisFormatted> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, int? customerAMUId, int? supplierAMUId)
        {

            return FormatRoutingsAnalyics(_datamanager.GetRoutingAnalysis(fromDate, toDate, customerId, supplierId, top, supplierAMUId, customerAMUId));
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
                SaleDurationFormatted = FormatNumber(customerSummary.SaleDuration, 2),
                SaleNetFormatted = FormatNumber(customerSummary.SaleNet, 2),
                CostDurationFormatted = FormatNumber(customerSummary.CostDuration, 2),
                CostNetFormatted = FormatNumber(customerSummary.CostNet, 2),
                ProfitFormatted = FormatNumber((customerSummary.SaleNet - customerSummary.CostNet), 2),
                ProfitPercentageFormatted = FormatNumber((customerSummary.SaleNet - customerSummary.CostNet) / customerSummary.SaleNet, 2)
            };
        }

        private RoutingAnalysisFormatted FormatRoutingAnalysis(RoutingAnalysis obj)
        {
            return new RoutingAnalysisFormatted
            {
                SaleZoneID = obj.SaleZoneID,
                SaleZone = (obj.SaleZoneID!=null)?_bemanager.GetZoneName(obj.SaleZoneID):null,
               
                SupplierID = obj.SupplierID,
                Supplier = (obj.SupplierID != null) ? _bemanager.GetCarrirAccountName(obj.SupplierID) : null,

                SaleNet = obj.SaleNet,
                SaleNetFormatted = FormatNumber(obj.SaleNet, 2),

                CostNet = obj.CostNet,
                CostNetFormatted = FormatNumber(obj.CostNet, 2),

                Duration = obj.Duration ,
                DurationFormatted = FormatNumber(obj.Duration),

                ACD = obj.ACD ,
                ACDFormatted = FormatNumber(obj.ACD,4),


                ASR = obj.ASR,
                ASRFormatted = FormatNumber(obj.ASR, 4),

                Profit = (double)((double)obj.SaleNet - (double)obj.CostNet),
                ProfitFormatted = FormatNumber((double)obj.SaleNet - (double)obj.CostNet),
                AVGCost = ((double)obj.Duration == 0 || (double)obj.CostNet == 0) ? 0 : (double)((double)obj.CostNet / (double)obj.Duration),
                AVGCostFormatted = ((double)obj.Duration == 0 || (double)obj.CostNet == 0) ? "0" : FormatNumber((double)((double)obj.CostNet / (double)obj.Duration), 5),
                AVGSale = ((double)obj.Duration == 0 || (double)obj.SaleNet == 0) ? 0 : (double)((double)obj.SaleNet / (double)obj.Duration),
                AVGSaleFormatted = ((double)obj.Duration == 0 || (double)obj.SaleNet == 0) ? "0" : FormatNumber((double)((double)obj.SaleNet / (double)obj.Duration), 5),
            };
        }
        #endregion 
        

     
    }
}
