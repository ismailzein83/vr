using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class PartnerInvoiceSettingDataManager:BaseSQLDataManager,IPartnerInvoiceSettingDataManager
    {
    
        #region ctor
        public PartnerInvoiceSettingDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<PartnerInvoiceSetting> GetPartnerInvoiceSettings()
        {
            return GetItemsSP("VR_Invoice.sp_PartnerInvoiceSetting_GetAll", PartnerInvoiceSettingMapper);
        }
        public bool ArePartnerInvoiceSettingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_Invoice.PartnerInvoiceSetting", ref updateHandle);
        }
        public bool InsertPartnerInvoiceSetting(Guid invoicePartnerSettingId, Guid invoiceSettingId, string partnerId, PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            string serializedPartnerInvoiceSettingDetails = null;
            if (partnerInvoiceSettingDetails != null)
                serializedPartnerInvoiceSettingDetails = Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_PartnerInvoiceSetting_Insert", invoicePartnerSettingId, partnerId, invoiceSettingId, serializedPartnerInvoiceSettingDetails);
            return (affectedRows > -1);
        }
        public bool UpdatePartnerInvoiceSetting(Guid partnerInvoiceSettingId, PartnerInvoiceSettingDetails partnerInvoiceSettingDetails)
        {
            string serializedObj = null;
            if (partnerInvoiceSettingDetails != null)
                serializedObj = Vanrise.Common.Serializer.Serialize(partnerInvoiceSettingDetails);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_PartnerInvoiceSetting_Update", partnerInvoiceSettingId, serializedObj);
            return (affectedRows > -1 );
        }
        public bool DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_PartnerInvoiceSetting_Delete", partnerInvoiceSettingId);
            return (affectedRows > 0);
        }
        public bool InsertOrUpdateInvoiceSetting(Guid partnerInvoiceSettingId,string partnerId, Guid invoiceSettingId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_PartnerInvoiceSetting_InsertOrUpdate", partnerInvoiceSettingId, partnerId, invoiceSettingId);
            return (affectedRows > -1);
        }

        #endregion
        
        #region Mappers
        public PartnerInvoiceSetting PartnerInvoiceSettingMapper(IDataReader reader)
        {
            PartnerInvoiceSetting partnerInvoiceSetting = new PartnerInvoiceSetting
            {
                PartnerInvoiceSettingId = GetReaderValue<Guid>(reader, "ID"),
                PartnerId = GetReaderValue<string>(reader, "PartnerId"),
                InvoiceSettingID = GetReaderValue<Guid>(reader, "InvoiceSettingID"),
                Details = Vanrise.Common.Serializer.Deserialize<PartnerInvoiceSettingDetails>(reader["Details"] as string)
            };
            return partnerInvoiceSetting;
        }
        #endregion
    }
}
