using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerFaultTicketsSettings : GenericBEExtendedSettings
    {
        static Guid openMailTemplateId = new Guid("7EB94106-43F1-43EB-8952-8F0B585FD7E5");
        static Guid pendingMailTemplateId = new Guid("05A87955-DC2A-4E22-A879-6BEA3C31690E");
        static Guid closedMailTemplateId = new Guid("F299EB6D-B50C-4338-812F-142D4D8515CA");
        public override Guid ConfigId
        {
            get { return new Guid("3705144E-4BC8-45D8-94D9-96E9AF95353B"); }
        }
        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return new ConfigManager().GetFaultTicketsCustomerSerialNumberPattern();
                case "SerialNumberInitialSequence": return new ConfigManager().GetFaultTicketsCustomerSerialNumberInitialSequence();
                case "OpenTicketMailTemplate": return new ConfigManager().GetFaultTicketsCustomerOpenMailTemplateId();
                case "PendingTicketMailTemplate": return new ConfigManager().GetFaultTicketsCustomerPendingMailTemplateId();
                case "ClosedTicketMailTemplate": return new ConfigManager().GetFaultTicketsCustomerClosedMailTemplateId();
                case "CompanySetting":
                    var customerId = Convert.ToInt32(context.GenericBusinessEntity.FieldValues.GetRecord("CustomerId"));
                    var companySettings = new CarrierAccountManager().GetCompanySetting(customerId);
                    companySettings.ThrowIfNull("companySettings");
                    return companySettings;
                case "Customer":
                    var carrierAccountId = Convert.ToInt32(context.GenericBusinessEntity.FieldValues.GetRecord("CustomerId"));
                    return new CarrierAccountManager().GetCarrierAccount(carrierAccountId);
                case "TicketDetails":
                    DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                    var dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(context.DefinitionSettings.DataRecordTypeId);
                    var dataRecord = Activator.CreateInstance(dataRecordRuntimeType, context.GenericBusinessEntity.FieldValues);
                    return dataRecord;
                case "EmailTemplateId":
                    if (context.GenericBusinessEntity != null && context.GenericBusinessEntity.FieldValues != null)
                    {
                        StatusDefinitionManager statusDefinitionManager = new StatusDefinitionManager();
                        var statusId = (Guid)context.GenericBusinessEntity.FieldValues.GetRecord("StatusId");
                        if (statusId == openMailTemplateId)
                            return new ConfigManager().GetFaultTicketsCustomerOpenMailTemplateId();
                        else if (statusId == pendingMailTemplateId)
                            return new ConfigManager().GetFaultTicketsCustomerPendingMailTemplateId();
                        else if (statusId == closedMailTemplateId)
                            return new ConfigManager().GetFaultTicketsCustomerClosedMailTemplateId();
                        else return null;
                    }
                    else return null;
                default: return null;
            }
        }
    }
}
