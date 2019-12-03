﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class UpdateContractAddressRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public CustomerType CustomerType { get; set; }
        public ContractInfoDetails ContractInfo { get; set; }
        public string  City { get; set; }
        public string CountryId { get; set; }
        public string StateProvince{ get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string Building { get; set; }
        public string LocationType { get; set; }
        public string Floor { get; set; }
        public DirectoryInquiry Action { get; set; }
        public PaymentData PaymentData { get; set; }
        public string ServiceId { get; set; }
        public string AddressSequence { get; set; }

        public string Career { get; set; }
        public string Language { get; set; }
        public string HomePhone { get; set; }
        public string FaxNumber { get; set; }
        public string MobilePhone { get; set; }
        public string GivenName { get; set; }
        public string Mailbox { get; set; }


    }
}
