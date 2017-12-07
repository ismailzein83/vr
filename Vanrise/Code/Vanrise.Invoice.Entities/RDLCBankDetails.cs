using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class RDLCBankDetails
    {
        public RDLCBankDetails() { }

        public string Bank { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string AccountCode { get; set; }
        public string AccountHolder { get; set; }
        public string IBAN { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string SwiftCode { get; set; }
        public string SortCode { get; set; }
        public string ChannelName { get; set; }
        public string CorrespondentBank { get; set; }
        public string CorrespondentBankSwiftCode { get; set; }
        public IEnumerable<RDLCBankDetails> GetBankDetailsRDLCSchema()
        {
            return null;
        }
    }
}
