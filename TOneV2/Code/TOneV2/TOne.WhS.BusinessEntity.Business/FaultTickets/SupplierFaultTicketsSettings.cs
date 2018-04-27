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
