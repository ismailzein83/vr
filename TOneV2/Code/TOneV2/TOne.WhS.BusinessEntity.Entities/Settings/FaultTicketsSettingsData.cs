using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class FaultTicketsSettingsData : Vanrise.Entities.SettingData
    {
        public CustomerFaultTicketSetting CustomerSetting { get; set; }
        public SupplierFaultTicketSetting SupplierSetting { get; set; }

    }
    public class CustomerFaultTicketSetting
    {
        public string SerialNumberPattern { get; set; }
        public long InitialSequence { get; set; }
        public Guid? OpenMailTemplateId { get; set; }
        public Guid? PendingMailTemplateId { get; set; }
        public Guid? ClosedMailTemplateId { get; set; }
    }
    public class SupplierFaultTicketSetting
    {
        public string SerialNumberPattern { get; set; }
        public long InitialSequence { get; set; }
        public Guid? OpenMailTemplateId { get; set; }
        public Guid? PendingMailTemplateId { get; set; }
        public Guid? ClosedMailTemplateId { get; set; }
    }
}
