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
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoice_CreateTempBySupplierID", tempTableName, input.Query.SelectedSupplierID, input.Query.From, input.Query.To);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceMapper);
        }

        public Vanrise.Entities.BigResult<SupplierInvoiceDetail> GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoiceDetails_CreateTempByInvoiceID", tempTableName, input.Query.InvoiceID);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceDetailMapper);
        }

        public Vanrise.Entities.BigResult<SupplierInvoiceDetailGroupedByDay> GetFilteredSupplierInvoiceDetailsGroupedByDay(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailGroupedByDayQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingCDRCost_CreateTempBySupplierID", tempTableName, input.Query.SupplierID, input.Query.From, input.Query.To);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceDetailGroupedByDayMapper);
        }

        public bool DeleteInvoice(int invoiceID)
        {
            int recordsEffected = ExecuteNonQuerySP("Billing.sp_BillingInvoice_Delete", invoiceID);
            return (recordsEffected > 0);
        }

        private SupplierInvoice SupplierInvoiceMapper(IDataReader reader)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice
            {
                InvoiceID = (int)reader["InvoiceID"],
                SupplierID = reader["SupplierID"] as string,
                SupplierName = GetCarrierName(reader["SupplierName"] as string, GetReaderValue<string>(reader, "SupplierNameSuffix")),
                UserID = GetReaderValue<int>(reader, "UserID"),
                UserName = GetReaderValue<string>(reader, "UserName"),
                SerialNumber = reader["SerialNumber"] as string,
                BeginDate = (DateTime)reader["BeginDate"],
                EndDate = (DateTime)reader["EndDate"],
                TimeZone = GetReaderValue<string>(reader, "TimeZone"),
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
                DetailID = (long)reader["DetailID"],
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

        private SupplierInvoiceDetailGroupedByDay SupplierInvoiceDetailGroupedByDayMapper(IDataReader reader)
        {
            SupplierInvoiceDetailGroupedByDay detail = new SupplierInvoiceDetailGroupedByDay
            {
                Day = GetReaderValue<String>(reader, "Day"),
                DurationInMinutes = GetReaderValue<decimal>(reader, "DurationInMinutes"),
                Amount = GetReaderValue<float>(reader, "Amount"),
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID")
            };

            return detail;
        }

        private string GetCarrierName(string name, string suffix) {
            
            if (suffix != null && suffix != "")
                return name + " (" + suffix + ")";
            else
                return name;
        }
    }
}
