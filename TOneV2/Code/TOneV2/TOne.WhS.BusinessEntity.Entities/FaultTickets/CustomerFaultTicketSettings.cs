using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerFaultTicketSettingsInput
    {
        public List<CustomerFaultTicketDescriptionSetting> DescriptionSettings { get; set; }
        public Guid ReasonBEDefinitionId { get; set; }
        public Guid ReleaseCodeBEDefinitionId { get; set; }
    }
    public class CustomerFaultTicketSettingsDetails
    {
        public List<CustomerFaultTicketDescriptionSettingDetails> DescriptionSettings { get; set; }
    }
    public class CustomerFaultTicketDescriptionSettingDetails
    {
        public string CodeNumber { get; set; }
        public Guid ReasonId { get; set; }
        public string ReasonDescription { get; set; }
        public long? InternationalReleaseCodeId { get; set; }
        public string InternationalReleaseCodeDescription { get; set; }
    }
    //public class CustomerFaultTicketSettings
    //{
    //    public List<CustomerFaultTicketDescriptionSetting> DescriptionSettings { get; set; }
    //}
    public class  CustomerFaultTicketDescriptionSetting
    {
        public string CodeNumber { get; set; }
        public Guid ReasonId { get; set; }
        public long? InternationalReleaseCodeId { get; set; }
    }
}
