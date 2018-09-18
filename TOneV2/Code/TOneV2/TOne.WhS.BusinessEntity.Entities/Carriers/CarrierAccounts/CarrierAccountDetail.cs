using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CarrierAccountDetail
    {
        public string CarrierProfileName { get; set; }
        public string CarrierAccountName { get; set; }
        public string AccountTypeDescription { get; set; }
        public string SellingNumberPlanName { get; set; }
        public string SellingProductName { get; set; }
        public string ActivationStatusDescription { get; set; }
        public CarrierAccount Entity { get; set; }
        public List<int> Services { get; set; }
        public string ServicesNames { get; set; }
        public string InvoiceTypeDescription { get; set; }
        public string InvoiceSettingName { get; set; }
        public string CompanySettingName { get; set; }
    }
}
