using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CarrierContactType
    {
        BillingContactPerson = 1,
        BillingEmail = 2,
        DisputeEmail = 3,
        PricingContactPerson = 4 ,
        PricingEmail = 5,
        AccountManagerContact = 6,
        AccountManagerEmail = 7,
        SupportContactPerson = 8,
        SupportEmail = 9, 
        TechnicalContactPerson = 10, 
        TechnicalEmail = 11,
        CommercialContactPerson = 12,
        CommercialEmail = 13, 
        AlertingSMSPhoneNumbers = 14
    }
    public class CarrierContact
    {
        public CarrierContactType Type { get; set; }

        public string Description { get; set; }
    }
}
