using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FaultTicketsSettingsData : Vanrise.Entities.SettingData
    {
        public FaultTicketSetting FaultTicketSetting { get; set; }
    }   
    public class FaultTicketSetting
    {
        public string SerialNumberPattern { get; set; }
        public long InitialSequence { get; set; }
        public Guid? OpenMailTemplateId { get; set; }
        public Guid? PendingMailTemplateId { get; set; }
        public Guid? ClosedMailTemplateId { get; set; }
    }
}
