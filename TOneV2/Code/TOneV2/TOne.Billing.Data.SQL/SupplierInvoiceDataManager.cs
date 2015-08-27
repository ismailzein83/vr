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
    public class SupplierInvoiceDataManager : BaseTOneDataManager, ISupplierInvoiceDataManager
    {
        public Vanrise.Entities.BigResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("InvoiceID", "InvoiceID");
            mapper.Add("SupplierName", "SupplierID");
            mapper.Add("UserName", "UserID");
            mapper.Add("SerialNumber", "SerialNumber");
            mapper.Add("BeginDate", "BeginDate");
            mapper.Add("EndDate", "EndDate");
            mapper.Add("TimeZone", "InvoiceNotes");
            mapper.Add("IssueDate", "IssueDate");
            mapper.Add("DueDate", "DueDate");
            mapper.Add("CreationDate", "CreationDate");
            mapper.Add("NumberOfCalls", "NumberOfCalls");
            mapper.Add("Duration", "Duration");
            mapper.Add("Amount", "Amount");
            mapper.Add("CurrencyID", "CurrencyID");
            mapper.Add("IsLocked", "IsLocked");
            mapper.Add("IsPaid", "IsPaid");
            mapper.Add("InvoiceNotes", "InvoiceNotes");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoice_CreateTempBySupplierID", tempTableName, input.Query.selectedSupplierID, input.Query.from, input.Query.to);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceMapper, mapper);
        }

        public Vanrise.Entities.BigResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("FromDate", "");
            mapper.Add("TillDate", "");
            mapper.Add("Destination", "");
            mapper.Add("NumberOfCalls", "");
            mapper.Add("Duration", "Duration");
            mapper.Add("Rate", "Rate");
            mapper.Add("RateType", "RateType");
            mapper.Add("Amount", "Amount");
            mapper.Add("CurrencyID", "CurrencyID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoiceDetails_CreateTempByInvoiceID", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceDetailMapper, mapper);
        }

        private SupplierInvoice SupplierInvoiceMapper(IDataReader reader)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice
            {
                InvoiceID = (int)reader["InvoiceID"],
                SupplierID = reader["SupplierID"] as string,
                UserID = GetReaderValue<int>(reader, "UserID"),
                UserName = GetReaderValue<string>(reader, "UserName"),
                SerialNumber = reader["SerialNumber"] as string,
                BeginDate = (DateTime)reader["BeginDate"],
                EndDate = (DateTime)reader["EndDate"],
                TimeZone = GetReaderValue<string>(reader, "InvoiceNotes"),
                IssueDate = (DateTime)reader["IssueDate"],
                DueDate = (DateTime)reader["DueDate"],
                CreationDate = (DateTime)reader["CreationDate"],
                NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Amount = (decimal)reader["Amount"],
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID"),
                IsLocked = reader["IsLocked"] as string,
                IsPaid = reader["IsPaid"] as string,
                InvoiceNotes = GetReaderValue<string>(reader, "InvoiceNotes")
            };

            return supplierInvoice;
        }

        private SupplierInvoiceDetail SupplierInvoiceDetailMapper(IDataReader reader)
        {
            SupplierInvoiceDetail supplierInvoiceDetail = new SupplierInvoiceDetail
            {
                DetailID = (int)reader["DetailID"],
                FromDate = GetReaderValue<DateTime>(reader, "FromDate"),
                TillDate = GetReaderValue<DateTime>(reader, "TillDate"),
                Destination = GetReaderValue<string>(reader, "Destination"),
                NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                RateType = GetReaderValue<string>(reader, "RateType"),
                Amount = GetReaderValue<decimal>(reader, "Amount"),
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID")
            };

            return supplierInvoiceDetail;
        }
    }
}
