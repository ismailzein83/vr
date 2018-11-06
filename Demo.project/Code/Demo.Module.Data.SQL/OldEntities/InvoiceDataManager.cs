using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class InvoiceDataManager : BaseSQLDataManager, IInvoiceDataManager
    {

        #region Constructors
        public InvoiceDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<Invoice> GetInvoices()
        {
            return GetItemsSP("[CcEntities].[sp_Invoice_GetInvoices]", InvoiceMapper);
        }
        #endregion  


        #region Mappers
        Invoice InvoiceMapper(IDataReader reader)
        {
            return new Invoice
            {
                ServiceId = GetReaderValue<long>(reader, "ID"),
                SerialNumber = GetReaderValue<string>(reader, "SerialNumber"),
                FromDate = GetReaderValue<DateTime>(reader, "FromDate"),
                ToDate = GetReaderValue<DateTime>(reader, "ToDate"),
                IssueDate = GetReaderValue<DateTime>(reader, "IssueDate"),
                Amount = GetReaderValue<decimal>(reader, "Amount"),
                Currency = GetReaderValue<string>(reader, "Currency")
            };
        }
        #endregion
    }
}
