using System;
using System.Collections.Generic;
using System.Data;

namespace TABS.DAL
{
    public partial class ObjectAssembler
    {
        public static void Ex_Bp_BuildBillingStatsPeriodDefined(string customerID, DateTime from, DateTime to)
        {
            string sql = QueryBuilder.Ex_Bp_BuildBillingStatsPeriodDefinedQuery();
            DataHelper.ExecuteNonQuery(sql, customerID, from, to);
        }

        public static object Ex_BP_ErroneousPricedCDR(string carrierAccountID, string isSale, DateTime fromDate, DateTime toDate)
        {
            string sql = QueryBuilder.Ex_BP_ErroneousPricedCDRQuery();
            var data = TABS.DataHelper.ExecuteScalar(sql, carrierAccountID, isSale, fromDate, toDate);
            return data;
        }

        public static DataTable Ex_Bp_CreateSupplierInvoiceGroupByDay(string supplierID, DateTime from, DateTime to, TABS.CustomTimeZoneInfo timeinfo)
        {
            string sql = QueryBuilder.Ex_Bp_CreateSupplierInvoiceGroupByDayQuery();
            return DataHelper.GetDataTable(sql, supplierID, from, to, timeinfo.BaseUtcOffset);
        }

        public static void DeleteBillingInvoice(int invoiceID)
        {
            string sqlString = QueryBuilder.DeleteBillingInvoiceQuery(invoiceID);
            TABS.DataHelper.ExecuteNonQuery(sqlString);
        }

        public static DataTable GetStatsVolumes(string fromDate, string toDate)
        {
            Dictionary<int, double> results = new Dictionary<int, double>();

            string SQL = QueryBuilder.GetStatsVolumesQuery(fromDate, toDate);
            return TABS.DataHelper.GetDataTable(SQL);
        }

        public static DataTable Ex_Sp_TrafficStats_BySupplierSaleZone_Enhanced(DateTime? fromDate, DateTime? toDate, string customerID)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_BySupplierSaleZone_EnhancedQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, customerID);
        }
    }
}
