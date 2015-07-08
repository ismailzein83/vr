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
                       SaleRate = string.Format("{0:0.00000}", (s.Sum(k => k.Sale_Rate * (double) k.Sale_Duration) / (double)s.Sum(k => k.Sale_Duration))),
                       Supplier = s.Key.Supplier,
                       CostDuration = string.Format("{0:#,##0.00}", s.Sum(k => k.Cost_Duration)),
                       CostRate = string.Format("{0:0.00000}", (s.Sum(k => k.Cost_Rate * (double)k.Cost_Duration) / (double)s.Sum(k => k.Cost_Duration))),
                       Profit = FormatNumber( s.Sum(k => k.Profit)),
                       ProfitPerc = string.Format("{0:#,##0.00}%", s.Average(k => k.Profit_Perc))
                   }).OrderBy(s => s.Customer + s.Destination + s.Supplier).ToList();

           return resultGrouped ;
        }

        public List<RoutingAnalysisFormatted> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, int? customerAMUId, int? supplierAMUId)
        {

            return FormatRoutingsAnalyics(_datamanager.GetRoutingAnalysis(fromDate, toDate, customerId, supplierId, top, supplierAMUId, customerAMUId));
        }

        public VariationReportResult GetInOutReportsData(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy)
        {
            List<VariationReportsData> customersList = new List<VariationReportsData>();
            List<VariationReportsData> onlyCustomersList = new List<VariationReportsData>();
            List<VariationReportsData> suppliersList = new List<VariationReportsData>();
            List<VariationReportsData> customersAndSuppliersList = new List<VariationReportsData>();
            List<TimeRange> timeRanges;



         //   switch (variationReportOptions)
        //    {
              //  case VariationReportOptions.InOutBoundMinutes:
                    var customersResult = GetVariationReportsData(selectedDate, periodCount, timePeriod, VariationReportOptions.InBoundMinutes, fromRow, toRow, entityType, entityID, groupingBy);
                   timeRanges = customersResult.TimeRange;
                    foreach (var customer in customersResult.VariationReportsData)
                    {
                        customersList.Add(customer);
                    
                    }
                    var suppliersResult = GetVariationReportsData(selectedDate, periodCount, timePeriod, VariationReportOptions.OutBoundMinutes, fromRow, toRow, entityType, entityID, groupingBy);
                    foreach (var supplier in suppliersResult.VariationReportsData)
                    {
                        suppliersList.Add(supplier);
                    
                    }
                    
                    foreach (var customer in customersList)
                    { 
                        var matchedSupplier = suppliersList.FirstOrDefault(s=>s.ID==customer.ID);
                        if (matchedSupplier!=null)
                            {                            
                               //create 3 item in reslut list : IN, Out ,Total
                                var name = customer.Name;
                              customer.Name = string.Format("{0}/IN",name);  
                              customersAndSuppliersList.Add(customer); 
                              matchedSupplier.Name = string.Format("{0}/OUT",name);
                              customersAndSuppliersList.Add(matchedSupplier);
                            
                            //Total:
                            var total = new VariationReportsData();
                            total.Name = string.Format("{0}/TOTAL", name);  
                            total.PeriodTypeValueAverage = customer.PeriodTypeValueAverage - matchedSupplier.PeriodTypeValueAverage  ;
                            total.PeriodTypeValuePercentage =  customer.PeriodTypeValuePercentage - matchedSupplier.PeriodTypeValuePercentage ;
                            total.PreviousPeriodTypeValuePercentage = matchedSupplier.PreviousPeriodTypeValuePercentage - customer.PreviousPeriodTypeValuePercentage;
                           // List<float> totalValues = new List<float>();
                            List<decimal> totalValues = new List<decimal>();
                            for(int i=0; i<customer.Values.Count;i++)
                            // totalValues.Add( float.Parse(matchedSupplier.Values[i].ToString()) - float.Parse(customer.Values[i].ToString() ) );
                            totalValues.Add(customer.Values[i] - matchedSupplier.Values[i] );
                            total.Values = totalValues;
                            customersAndSuppliersList.Add(total);

                            //remove out element from out list
                            suppliersList.Remove(matchedSupplier);

                            }
                            else {//add item to in list
                                   onlyCustomersList.Add(customer); }
                       }
                     customersAndSuppliersList =  customersAndSuppliersList.OrderBy(item=>item.Name).ToList();

                        //add items in in list
                        //add items remaining in out list
                    foreach(var item in onlyCustomersList)
                          item.Name = string.Format("{0}/IN",item.Name);
                    foreach (var item in suppliersList)
                        item.Name = string.Format("{0}/OUT", item.Name);

                    customersAndSuppliersList.AddRange(onlyCustomersList.OrderBy(customer => customer.Name).ToList());

                    customersAndSuppliersList.AddRange(suppliersList.OrderBy(supplier=>supplier.Name).ToList());
                   
                 // return //  VariationReportResult inOutData = 
               //     break;

                 

                    


                //case VariationReportOptions.InOutBoundAmount:
                //    resultInList.Add(GetVariationReportsData(selectedDate, periodCount, timePeriod, VariationReportOptions.InBoundAmount, fromRow, toRow, entityType, entityID, groupingBy));
                //    resultOutList.Add(GetVariationReportsData(selectedDate, periodCount, timePeriod, VariationReportOptions.OutBoundAmount, fromRow, toRow, entityType, entityID, groupingBy));
                //    break;
          //  }
            return new VariationReportResult() { VariationReportsData = customersAndSuppliersList, TimeRange = timeRanges };

           
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
                ACDFormatted = FormatNumber(obj.ACD,5),


                ASR = obj.ASR,
                ASRFormatted = FormatNumber(obj.ASR, 5),

                Profit = FormatNumber((double)((double)obj.SaleNet- (double)obj.CostNet) , 2),

                AVGCost = FormatNumber((double)((double)obj.CostNet / (double)obj.Duration), 5),

                AVGSale = FormatNumber((double)((double)obj.SaleNet / (double)obj.Duration), 5)
                

                
            
            };
        }
        #endregion 
        

     
    }
}
