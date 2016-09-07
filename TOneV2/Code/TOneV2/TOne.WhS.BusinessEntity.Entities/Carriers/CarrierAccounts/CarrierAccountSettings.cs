using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum ActivationStatus
    {
        [Description("Active")]
        Active = 0,

        [Description("Inactive")]
        Inactive = 1,

        [Description("Testing")]
        Testing = 2
    }
    public class CarrierAccountSettings
    {
        public ActivationStatus ActivationStatus { get; set; }

        public int CurrencyId { get; set; }

        public string Mask { get; set; }

        public int NominalCapacity { get; set; }
        public CarrierAccountPriceListSettings PriceListSettings { get; set; }
    }
    public class CarrierAccountPriceListSettings
    {
        public string Email { get; set; }
        public string FileMask { get; set; }
        public string SubjectCode { get; set; }
        public List<int> BPBusinessRuleSetIds { get; set; }
    }
}
