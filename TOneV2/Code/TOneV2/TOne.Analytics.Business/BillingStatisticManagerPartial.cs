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
        public List<CustomerSummaryFormatted> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return FormatCustomerSummaries(_datamanager.GetCustomerSummary(fromDate, toDate, customerId, customerAMUId, supplierAMUId));
        }
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
                CostNet = customerSummary.CostNet
            };
        }

    }
}
