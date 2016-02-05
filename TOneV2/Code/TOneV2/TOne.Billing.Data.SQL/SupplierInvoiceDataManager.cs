﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Entities;
using TOne.BusinessEntity.Entities;
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
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("RateTypeDescription", "RateType");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("Billing.sp_BillingInvoiceDetails_CreateTempByInvoiceID", tempTableName, input.Query.InvoiceID);
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceDetailMapper, mapper);
        }

        public Vanrise.Entities.BigResult<SupplierInvoiceDetailGroupedByDay> GetFilteredSupplierInvoiceDetailsGroupedByDay(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceDetailGroupedByDayQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

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
            };

            return supplierInvoice;
        }

        private SupplierInvoiceDetail SupplierInvoiceDetailMapper(IDataReader reader)
        {
            SupplierInvoiceDetail detail = new SupplierInvoiceDetail();

            detail.DetailID = (long)reader["DetailID"];
            detail.FromDate = GetReaderValue<DateTime>(reader, "FromDate");
            detail.TillDate = GetReaderValue<DateTime>(reader, "TillDate");
            detail.Destination = GetReaderValue<string>(reader, "Destination");
            detail.NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls");
            detail.Duration = GetReaderValue<decimal>(reader, "Duration");
            detail.Rate = GetReaderValue<decimal>(reader, "Rate");
            
            string rateTypeString = GetReaderValue<string>(reader, "RateType");
            int? rateTypeInt = Convert.ToInt32(rateTypeString);
            detail.RateType = (RateTypeEnum)rateTypeInt;
            
            detail.Amount = GetReaderValue<decimal>(reader, "Amount");
            detail.CurrencyID = GetReaderValue<string>(reader, "CurrencyID");

            return detail;
        }

        private SupplierInvoiceDetailGroupedByDay SupplierInvoiceDetailGroupedByDayMapper(IDataReader reader)
        {
            SupplierInvoiceDetailGroupedByDay detail = new SupplierInvoiceDetailGroupedByDay();
             detail.Day = (DateTime)reader["Day"];
                detail.DurationInMinutes = GetReaderValue<decimal?>(reader, "DurationInMinutes");
                detail.Amount = GetReaderValue<Double?>(reader, "Amount");
                detail.CurrencyID = GetReaderValue<string>(reader, "CurrencyID");
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
