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
        public List<CustomerSummaryFormatted> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, int? customerAMUId, int? supplierAMUId)
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
                CostNet = customerSummary.CostNet,
                SaleDurationFormatted = FormatNumber(customerSummary.SaleDuration, 2),
                SaleNetFormatted = FormatNumber(customerSummary.SaleNet, 2),
                CostDurationFormatted = FormatNumber(customerSummary.CostDuration, 2),
                CostNetFormatted = FormatNumber(customerSummary.CostNet, 2)
                //ProfitFormatted = FormatNumber((SaleNet - CostNet),2),
                
                
                //=FormatNumber(Fields!Services.Value,2) services
                //=FormatNumber(Fields!SaleNet.Value-Fields!CostNet.Value,2)
                //=IIf(IsNothing(Fields!CostNet.Value),"0.00",FormatNumber(Fields!CostNet.Value,Parameters!DigitRate.Value))


                //                SupplierID = (zoneSummaryDetailed.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneSummaryDetailed.SupplierID) : null,

                //Calls = zoneSummaryDetailed.Calls,

                //Rate = zoneSummaryDetailed.Rate,
                //RateFormatted = FormatNumber(zoneSummaryDetailed.Rate, 5),

                //DurationNet = zoneSummaryDetailed.DurationNet,
                //DurationNetFormatted = FormatNumber(zoneSummaryDetailed.DurationNet),

                //OffPeakDurationInSeconds = zoneSummaryDetailed.OffPeakDurationInSeconds,
                //OffPeakDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.OffPeakDurationInSeconds, 2),

                //OffPeakRate = zoneSummaryDetailed.OffPeakRate,
                //OffPeakRateFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate, 5),

                //OffPeakNet = zoneSummaryDetailed.OffPeakNet,
                //OffPeakNetFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate, 2),

                //WeekEndDurationInSeconds = zoneSummaryDetailed.WeekEndDurationInSeconds,
                //WeekEndDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.WeekEndDurationInSeconds, 2),

                //WeekEndRate = zoneSummaryDetailed.WeekEndRate,
                //WeekEndRateFormatted = FormatNumber(zoneSummaryDetailed.WeekEndRate, 2),

                //WeekEndNet = zoneSummaryDetailed.WeekEndNet,
                //WeekEndNetFormatted = FormatNumber(zoneSummaryDetailed.WeekEndNet, 2),

                //DurationInSeconds = zoneSummaryDetailed.DurationInSeconds,
                //DurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.DurationInSeconds),

                //Net = zoneSummaryDetailed.Net,
                //NetFormatted = FormatNumber(zoneSummaryDetailed.Net, 5),

                //TotalDurationFormatted = FormatNumber((zoneSummaryDetailed.DurationInSeconds + zoneSummaryDetailed.OffPeakDurationInSeconds + zoneSummaryDetailed.WeekEndDurationInSeconds), 2),

                //TotalAmountFormatted = FormatNumber(zoneSummaryDetailed.Net + zoneSummaryDetailed.OffPeakNet + zoneSummaryDetailed.WeekEndNet, 2),

                //CommissionValue = zoneSummaryDetailed.CommissionValue,
                //CommissionValueFormatted = FormatNumber(zoneSummaryDetailed.CommissionValue, 2),

                //ExtraChargeValue = zoneSummaryDetailed.ExtraChargeValue,
                //ExtraChargeValueFormatted = FormatNumber(zoneSummaryDetailed.ExtraChargeValue),
            };
        }

    }
}
