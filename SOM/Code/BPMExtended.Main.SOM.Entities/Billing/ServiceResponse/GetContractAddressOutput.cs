using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class GetContractAddressOutput
    {
        public Address Address { get; set; }
        public bool InDirectory { get; set; }

        public string Career { get; set; }
        public string Language { get; set; }
        public string HomePhone { get; set; }
        public string FaxNumber { get; set; }
        public string MobilePhone { get; set; }
        public string GivenName { get; set; }
        public string CompanyName { get; set; }
        public string Branch { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessType { get; set; }
        public string MiddleName { get; set; }
        public string WholeSale { get; set; }

    }
}
