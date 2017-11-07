﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceSettingDataManager : BaseSQLDataManager, IInvoiceSettingDataManager
    {
        #region ctor
        public InvoiceSettingDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<InvoiceSetting> GetInvoiceSettings()
        {
            return GetItemsSP("VR_Invoice.sp_InvoiceSetting_GetAll", InvoiceSettingMapper);
        }
        public bool AreInvoiceSettingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_Invoice.InvoiceSetting", ref updateHandle);
        }
        public bool InsertInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            string serializedObj = null;
            if (invoiceSetting.Details != null)
                serializedObj = Vanrise.Common.Serializer.Serialize(invoiceSetting.Details);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceSetting_Insert", invoiceSetting.InvoiceSettingId, invoiceSetting.Name, invoiceSetting.InvoiceTypeId,invoiceSetting.IsDefault, serializedObj);
            return (affectedRows > -1);
        }
        public bool UpdateInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            string serializedObj = null;
            if (invoiceSetting.Details != null)
                serializedObj = Vanrise.Common.Serializer.Serialize(invoiceSetting.Details);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceSetting_Update", invoiceSetting.InvoiceSettingId, invoiceSetting.Name, invoiceSetting.InvoiceTypeId, invoiceSetting.IsDefault, serializedObj);
            return (affectedRows > -1 );
        }
        public bool SetInvoiceSettingDefault(Guid invoiceSettingId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceSetting_SetDefault", invoiceSettingId);
            return (affectedRows > -1);
        }

        public bool DeleteInvoiceSetting(Guid invoiceSettingId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceSetting_Delete", invoiceSettingId);
            return affectedRows > 0;
        }

        #endregion
        
        #region Mappers
        public InvoiceSetting InvoiceSettingMapper(System.Data.IDataReader reader)
        {
            InvoiceSetting invoiceSetting = new InvoiceSetting
            {
                InvoiceSettingId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                InvoiceTypeId = GetReaderValue<Guid>(reader, "InvoiceTypeId"),
                Details = Vanrise.Common.Serializer.Deserialize<InvoiceSettingDetails>(reader["Details"] as string),
                IsDefault = GetReaderValue<Boolean>(reader, "IsDefault"),
            };
            return invoiceSetting;
        }
        #endregion
    }
}
