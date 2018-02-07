using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierFaultTicketSettingsInput
    {
        public List<SupplierFaultTicketDescriptionSetting> DescriptionSettings { get; set; }
        public Guid ReasonBEDefinitionId { get; set; }
        public Guid ReleaseCodeBEDefinitionId { get; set; }
    }
    public class SupplierFaultTicketSettingsDetails
    {
        public List<SupplierFaultTicketDescriptionSettingDetails> DescriptionSettings { get; set; }
    }
    public class SupplierFaultTicketDescriptionSettingDetails
    {
        public string CodeNumber { get; set; }
        public Guid ReasonId { get; set; }
        public string ReasonDescription { get; set; }
        public Guid? InternationalReleaseCodeId { get; set; }
        public string InternationalReleaseCodeDescription { get; set; }
    }
    //public class CustomerFaultTicketSettings
    //{
    //    public List<CustomerFaultTicketDescriptionSetting> DescriptionSettings { get; set; }
    //}
    public class SupplierFaultTicketDescriptionSetting
    {
        public string CodeNumber { get; set; }
        public Guid ReasonId { get; set; }
        public Guid? InternationalReleaseCodeId { get; set; }
    }
}
