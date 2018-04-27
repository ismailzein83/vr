using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerFaultTicketsSettings : GenericBEExtendedSettings
    {
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

                case "TicketDetails":
                    DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                    var dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(context.DefinitionSettings.DataRecordTypeId);
                    var dataRecord = Activator.CreateInstance(dataRecordRuntimeType, context.GenericBusinessEntity.FieldValues);
                    return dataRecord;
                default: return null;
            }
        }
    }
}
