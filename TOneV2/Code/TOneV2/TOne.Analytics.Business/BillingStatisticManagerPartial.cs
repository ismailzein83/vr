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

        public List<CustomerRoutingFormatted> GetCustomerRouting()
        {
            return new List<CustomerRoutingFormatted>();
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
        #endregion 
        

     
    }
}
