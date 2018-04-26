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
    public class InvoiceTypeDataManager : BaseSQLDataManager, IInvoiceTypeDataManager
    {

        #region ctor
        public InvoiceTypeDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<InvoiceType> GetInvoiceTypes()
        {
            return GetItemsSP("VR_Invoice.sp_InvoiceType_GetAll", InvoiceTypeMapper);
        }
        public bool AreInvoiceTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_Invoice.InvoiceType", ref updateHandle);
        }

        public bool InsertInvoiceType(InvoiceType invoiceType)
        {
            string serializedObj = null;
            if(invoiceType.Settings != null)
                serializedObj =  Vanrise.Common.Serializer.Serialize(invoiceType.Settings);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceType_Insert", invoiceType.InvoiceTypeId, invoiceType.Name, serializedObj);
            return (affectedRows > -1);
        }
        public bool UpdateInvoiceType(InvoiceType invoiceType)
        {
            string serializedObj = null;
            if (invoiceType.Settings != null)
                serializedObj = Vanrise.Common.Serializer.Serialize(invoiceType.Settings);

            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_InvoiceType_Update", invoiceType.InvoiceTypeId, invoiceType.Name, serializedObj);
            return (affectedRows > -1 );
        }

        public bool ApproveInvoice(long invoiceId, DateTime? ApprovedDate, int? ApprovedBy)
        {
            int affectedRows = ExecuteNonQuerySP("VR_Invoice.sp_Invoice_UpdateInvoiceApproved", invoiceId, ApprovedDate, ApprovedBy);
            return (affectedRows > -1);
        }

        #endregion
        
        #region Mappers
        public InvoiceType InvoiceTypeMapper(IDataReader reader)
        {
            InvoiceType invoiceType = new InvoiceType
            {
                InvoiceTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceTypeSettings>(reader["Settings"] as string)
            };
            return invoiceType;
        }
        #endregion
    }
}
