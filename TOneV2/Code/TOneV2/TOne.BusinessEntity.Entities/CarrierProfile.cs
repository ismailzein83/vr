using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierProfile
    {
        public int ProfileID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string BillingEmail { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string RegistrationNumber { get; set; }
        public string[] Telephone { get; set; }
        public string[] Fax { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Website { get; set; }
        public int AccountsCount { get; set; }
    }
}
