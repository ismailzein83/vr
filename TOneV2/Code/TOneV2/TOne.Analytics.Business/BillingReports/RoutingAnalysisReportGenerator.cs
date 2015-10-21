using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.Entities;

namespace TOne.Analytics.Business.BillingReports
{
    public class RoutingAnalysisReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {
            AccountManagerManager am = new AccountManagerManager();
            List<string> suppliersIds = am.GetMyAssignedSupplierIds();
            List<string> customersIds = am.GetMyAssignedCustomerIds();


            BillingStatisticManager manager = new BillingStatisticManager();
            List<RoutingAnalysisFormatted> routingAnalysis = manager.GetRoutingAnalysis(parameters.FromTime, parameters.ToTime, parameters.CustomersId, parameters.SuppliersId, parameters.Top, customersIds, suppliersIds, parameters.CurrencyId);


            decimal TotalDuration = 0;
            double TotalSale = 0;
            double TotalCost = 0;
            double TotalProfit = 0;

            
            foreach (  var row in routingAnalysis)
            {
              

                TotalDuration += row.Duration != null ? row.Duration : 0;
                TotalCost += row.CostNet!=null ? (double)row.CostNet : 0;
                TotalSale += row.SaleNet!= null ? (double)row.SaleNet: 0;
                TotalProfit += (double)((double)row.SaleNet - (double)row.CostNet);
            }

            parameters.TotalDuration = TotalDuration;
            parameters.TotalSale = TotalSale;
            parameters.TotalCost = TotalSale;
            parameters.TotalProfit = TotalProfit;

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("RoutingAnalysis", routingAnalysis);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(TOne.Entities.ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Routing Analysis Report", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "2", IsVisible = true });

            list.Add("TotalDuration", new RdlcParameter { Value = parameters.TotalDuration.ToString(), IsVisible = true });
            list.Add("TotalSale", new RdlcParameter { Value = parameters.TotalSale.ToString(), IsVisible = true });
            list.Add("TotalCost", new RdlcParameter { Value = parameters.TotalCost.ToString(), IsVisible = true });
            list.Add("TotalProfit", new RdlcParameter { Value = parameters.TotalProfit.ToString(), IsVisible = true });


            list.Add("PageBreak", new RdlcParameter { Value = parameters.PageBreak.ToString(), IsVisible = true });

            return list;
        }
    }
}
