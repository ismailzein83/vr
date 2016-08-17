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
