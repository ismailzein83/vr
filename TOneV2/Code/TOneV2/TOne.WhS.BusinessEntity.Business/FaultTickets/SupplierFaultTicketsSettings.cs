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
    public class SupplierFaultTicketsSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FCFA0193-9463-4435-9D96-54FD2B6D050B"); }    
        }
        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return new ConfigManager().GetFaultTicketsSupplierSerialNumberPattern();
                case "SerialNumberInitialSequence": return new ConfigManager().GetFaultTicketsSupplierSerialNumberInitialSequence();
                case "OpenTicketMailTemplate": return new ConfigManager().GetFaultTicketsSupplierOpenMailTemplateId();
                case "PendingTicketMailTemplate": return new ConfigManager().GetFaultTicketsSupplierPendingMailTemplateId();
                case "ClosedTicketMailTemplate": return new ConfigManager().GetFaultTicketsSupplierClosedMailTemplateId();
                case "CompanySetting":
                    var supplierId = Convert.ToInt32(context.GenericBusinessEntity.FieldValues.GetRecord("SupplierId"));
                    var companySettings = new CarrierAccountManager().GetCompanySetting(supplierId);
                    companySettings.ThrowIfNull("companySettings");
                    return companySettings;
                case "Supplier":
                    var carrierAccountId = Convert.ToInt32(context.GenericBusinessEntity.FieldValues.GetRecord("SupplierId"));
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
                        var statusId = context.GenericBusinessEntity.FieldValues.GetRecord("StatusId");
                        switch (statusId.ToString().ToUpper())
                        {
                            case "7EB94106-43F1-43EB-8952-8F0B585FD7E5": return new ConfigManager().GetFaultTicketsSupplierOpenMailTemplateId();
                            case "05A87955-DC2A-4E22-A879-6BEA3C31690E": return new ConfigManager().GetFaultTicketsSupplierPendingMailTemplateId();
                            case "F299EB6D-B50C-4338-812F-142D4D8515CA": return new ConfigManager().GetFaultTicketsSupplierClosedMailTemplateId();
                            default: return null;
                        }
                    }
                    else return null;
                default: return null;
            }
        }
    }
}
