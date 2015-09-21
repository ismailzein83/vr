using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Entities;
using TOne.Data.SQL;

namespace TOne.Billing.Data.SQL
{
    public class CustomerInvoiceDataManager : BaseTOneDataManager, ICustomerInvoiceDataManager
    {
        public Vanrise.Entities.BigResult<CustomerInvoice> GetFilteredCustomerInvoices(Vanrise.Entities.DataRetrievalInput<CustomerInvoiceQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoice_CreateTempByCustomerID", tempTableName, input.Query.SelectedCustomerId, input.Query.FromDate, input.Query.ToDate);
            };

            return RetrieveData(input, createTempTableAction, CustomerInvoiceMapper);

        }

        #region PrivatMethods
        private CustomerInvoice CustomerInvoiceMapper(IDataReader reader)
        {
            CustomerInvoice customerInvoice = new CustomerInvoice
            {
                InvoiceId = (int)reader["InvoiceID"],
                CustomerId = reader["CustomerID"] as string,
                CustomerName = GetCarrierName(reader["CustomerName"] as string, GetReaderValue<string>(reader, "CustomerNameSuffix")),
                SupplierId = reader["SupplierID"] as string,
                SupplierName = GetCarrierName(reader["SupplierName"] as string, GetReaderValue<string>(reader, "SupplierNameSuffix")),
                UserId = GetReaderValue<int>(reader, "UserID"),
                UserName = GetReaderValue<string>(reader, "UserName"),
                SerialNumber = reader["SerialNumber"] as string,
                BeginDate = (DateTime)reader["BeginDate"],
                EndDate = (DateTime)reader["EndDate"],
                TimeZone = GetReaderValue<string>(reader, "TimeZone"),
                IssueDate = (DateTime)reader["IssueDate"],
                DueDate = (DateTime)reader["DueDate"],
                CreationDate = (DateTime)reader["CreationDate"],
                PaidDate =   reader["PaidDate"]!= DBNull.Value ? GetReaderValue<DateTime?>(reader,"PaidDate") : null,
                NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Amount = (decimal)reader["Amount"],
                CurrencyId = GetReaderValue<string>(reader, "CurrencyId"),
                IsLocked = reader["IsLocked"] as string,
                IsPaid =  reader["IsPaid"] as string,
                IsAutomatic = GetReaderValue<string>(reader, "IsAutomatic") ,
                IsSent =  GetReaderValue<string>(reader, "IsSent"),
                InvoiceNotes = GetReaderValue<string>(reader, "InvoiceNotes")
            };

            return customerInvoice;
        }
        private string GetCarrierName(string name, string suffix)
        {

            if (!string.IsNullOrEmpty(suffix))
                return name + " (" + suffix + ")";
            else
                return name;
        }
    }
        #endregion
}
