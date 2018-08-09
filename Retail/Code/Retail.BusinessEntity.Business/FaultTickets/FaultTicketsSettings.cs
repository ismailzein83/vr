using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class FaultTicketsSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B24015FB-D7C9-4051-9FA6-9521EBC3EF9D"); }
        }
        //public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        //{
        //    if (context.InfoType == null)
        //        return null;

        //    switch (context.InfoType)
        //    {
        //        case "SerialNumberPattern": return new ConfigManager().GetFaultTicketsSerialNumberPattern();
        //        case "SerialNumberInitialSequence": return new ConfigManager().GetFaultTicketsSerialNumberInitialSequence();
        //        case "OpenTicketMailTemplate": return new ConfigManager().GetFaultTicketsOpenMailTemplateId();
        //        case "PendingTicketMailTemplate": return new ConfigManager().GetFaultTicketsPendingMailTemplateId();
        //        case "ClosedTicketMailTemplate": return new ConfigManager().GetFaultTicketsClosedMailTemplateId();
                
        //    }
        //}
    }
}
