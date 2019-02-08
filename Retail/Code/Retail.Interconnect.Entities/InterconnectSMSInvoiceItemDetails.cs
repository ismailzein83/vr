using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class InterconnectSMSInvoiceItemDetails
    {
        public decimal Amount { get; set; }
        public long DestinationMobileNetworkId { get; set; }
        public string DestinationMobileNetworkIdDescription { get; set; }
        public long OriginationMobileNetworkId { get; set; }
        public string OriginationMobileNetworkIdDescription { get; set; }
        public long OperatorId { get; set; }
        public string OperatorIdDescription { get; set; }
        public decimal Rate { get; set; }
        public string RateDescription { get; set; }
        public int RateTypeId { get; set; }
        public string RateTypeIdDescription { get; set; }
        public int BillingType { get; set; }
        public string BillingTypeDescription { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public int NumberOfSMS { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OriginalAmount { get; set; }
        public IEnumerable<InterconnectSMSInvoiceItemDetails> GetCustomerInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
