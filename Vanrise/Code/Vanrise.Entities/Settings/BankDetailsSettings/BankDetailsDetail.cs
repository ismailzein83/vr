﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class BankDetailsDetail
    {
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
        public string ACH { get; set; }
        public string ABARoutingNumber { get; set; }
        public string MoreInfo { get; set; }
        public string SecondaryAccounts { get; set; }
    }

    public class SecondaryAccount
    {
        public string AccountNumber { get; set; }
        public int CurrencyId { get; set; }
    }
}
