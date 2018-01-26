using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerFaultTicketSettings
    {
        public List<CustomerFaultTicketDescriptionSetting> DescriptionSettings { get; set; }
    }
    public class  CustomerFaultTicketDescriptionSetting
    {
        public string CodeNumber { get; set; }
        public Guid ReasonId { get; set; }
        public Guid? InternationalReleaseCodeId { get; set; }
    }
}
