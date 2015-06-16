using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Entities;

namespace TOne.Analytics.Business.BillingReports
{
    public class CustomerSummaryReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {
            BillingStatisticManager manager = new BillingStatisticManager();
            List<CustomerSummaryFormatted> customerSummary = manager.GetCustomerSummary(parameters.FromTime, parameters.ToTime, parameters.CustomerId, parameters.SupplierAMUId, parameters.CustomerAMUId);
            decimal servicesPerCustomer = 0;
            foreach (var row in customerSummary)
            {
                if (row.Carrier != null)                         
                    servicesPerCustomer += (decimal)row.Services;
            }
            parameters.ServicesForCustomer = servicesPerCustomer;
            List<ProfitSummary> profitSummary = manager.GetCustomerProfitSummary(customerSummary);
            List<SaleAmountSummary> saleAmount = manager.GetCustomerSaleAmountSummary(customerSummary);
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CustomerSummary", customerSummary);
            dataSources.Add("SaleAmount", saleAmount);
            dataSources.Add("ProfitSummary", profitSummary);            
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(TOne.Entities.ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Customer Sales and Profits", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = "[USD] United States Dollars", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = "", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "2", IsVisible = true });
            list.Add("ShowProfit", new RdlcParameter { Value = parameters.IsService.ToString(), IsVisible = true });
            list.Add("ServicesPerCustomer", new RdlcParameter { Value = parameters.ServicesForCustomer.ToString(), IsVisible = true });

            return list;
        }
    }
}
