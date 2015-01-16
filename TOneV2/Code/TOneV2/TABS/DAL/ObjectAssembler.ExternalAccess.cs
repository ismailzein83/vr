using System;
using System.Collections.Generic;
using System.Data;

namespace TABS.DAL
{
    public partial class ObjectAssembler
    {
        public static DataTable Ex_SP_TrafficStats_BlockedAttempts(string customerID, int? ourZoneID, DateTime? fromDate,
                                                  DateTime? toDate, string groupByName)
        {
            string sql = QueryBuilder.Ex_SP_TrafficStats_BlockedAttempts();
            return TABS.DataHelper.GetDataTable(sql,
                            customerID,
                            ourZoneID,
                            fromDate.Value,
                            toDate.Value,
                            groupByName);
        }
        //----------------------------------------------> Custom paging <-------------------------------------------------------------
        public static DataTable Ex_SP_TrafficStats_BlockedAttempts_CustomPaging(string customerID, int? ourZoneID, DateTime? fromDate,
                                                 DateTime? toDate, string groupByName, int pageIndex, int pageSize, out int RecordCount)
        {
            string sql = QueryBuilder.Ex_SP_TrafficStats_BlockedAttempts_CustomPaging();
            TABS.DataHelper.ParameterGenerator[] parameters = new TABS.DataHelper.ParameterGenerator[] {
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@CustomerID", Value=customerID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@OurZoneID", Value=ourZoneID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@FromDateTime", Value=fromDate.Value },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@ToDateTime", Value=toDate.Value },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@GroupByNumber", Value=groupByName },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageIndex", Value= pageIndex },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageSize", Value= pageSize },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Output, Name="@RecordCount", Value = 0 }
        };
            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            DataTable data = TABS.DataHelper.GetDataTable(sql, parameters, out outputParams);
            RecordCount = (int)outputParams["@RecordCount"];
            return data;
        }
        //-------------------------------------------------------------------------------------------------------------------------------
        public static DataTable Ex_Sp_Traffic_CDR(int? cdrOption, string fromDuration, string toDuration, DateTime? fromDate,
                                                  DateTime? toDate, int? limitResult, string supplierID, string customerID,
                                                  string ourZoneID, string number, string cli, string releaseCode)
        {
            string sql = QueryBuilder.Ex_Sp_Traffic_CDRQuery(cdrOption, fromDuration, toDuration, fromDate, toDate, limitResult, supplierID, customerID,
                                ourZoneID, number, cli, releaseCode);
            return DataHelper.GetDataTable(sql);
        }

        public static IList<TABS.Rate> GetCustomerRates(TABS.CarrierAccount account, DateTime effectiveDate)
        {
            NHibernate.IQuery query = QueryBuilder.GetCustomerRatesQuery(account, effectiveDate);
            return query.List<TABS.Rate>();
        }
        //-----------------------------------------------------------------For Custom Paging-----------------------------------------------------------------------------
        public static IList<TABS.Rate> GetCustomerRates_CustomPaging(TABS.CarrierAccount account, DateTime effectiveDate, int pageIndex, int pageSize, out int RecordCount)
        {
            NHibernate.IQuery query = QueryBuilder.GetCustomerRatesQuery(account, effectiveDate);
            RecordCount = query.List<TABS.Rate>().Count;
            return query.SetFirstResult(pageSize * (pageIndex - 1)).SetMaxResults(pageSize).List<TABS.Rate>();
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------

        public static DataTable Ex_Ea_SupplierSummary(DateTime fromDate, DateTime toDate, string supplierID)
        {
            string sql = QueryBuilder.Ex_Ea_SupplierSummaryQuery(fromDate, toDate, supplierID);
            return DataHelper.GetDataTable(sql);
        }

        public static DataTable Ex_Ea_CustomerSummary(DateTime fromDate, DateTime toDate, string customerID)
        {
            string sql = QueryBuilder.Ex_Ea_CustomerSummaryQuery(fromDate, toDate, customerID);
            return DataHelper.GetDataTable(sql);
        }
        public static DataTable Ex_Sp_TrafficStats_HourlyReport(DateTime? fromDate, DateTime? toDate, string CustomerID, string SupplierID,
                                                 int? ourZoneID, string codeGroup)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_HourlyReportQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, CustomerID, SupplierID, ourZoneID, codeGroup);
        }

        public static DataTable Ex_Sp_TrafficStats_ReleaseCodeStats(DateTime? fromDate, DateTime? toDate, string customerID, string SupplierID, int? ourZoneID)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_ReleaseCodeStatsQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, customerID, SupplierID, ourZoneID);
        }
        //-----------------------------------------------------> Custom Paging <----------------------------------------------------------------------
        public static DataTable Ex_Sp_TrafficStats_ReleaseCodeStats_CustomPaging(DateTime? fromDate, DateTime? toDate, string customerID, string SupplierID, int? ourZoneID, int pageIndex, int pageSize, out int RecordCount)
        {
            string sql = @"Sp_TrafficStats_ReleaseCodeStats";
            TABS.DataHelper.ParameterGenerator[] parameters = new TABS.DataHelper.ParameterGenerator[] {
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@FromDate", Value=fromDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@ToDate", Value=toDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@CustomerID", Value=customerID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@SupplierID", Value=SupplierID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@OurZoneID", Value=ourZoneID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageIndex", Value= pageIndex },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageSize", Value= pageSize },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Output, Name="@RecordCount", Value = 0 }
        };

            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            DataTable data = TABS.DataHelper.GetDataTable(sql, parameters, out outputParams);
            RecordCount = (int)outputParams["@RecordCount"];
            return data;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------
        public static DataTable Ex_Sp_TrafficStats_RepeatedNumbers(DateTime? fromDate, DateTime? toDate,
                                                  string number, string type, int? switchID, string carrierAccountID)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_RepeatedNumberQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, number, type, switchID, carrierAccountID);
        }
        //------------------------------------------------------> Custom Paging <----------------------------------------------------------------------------------------------
        public static DataTable Ex_Sp_TrafficStats_RepeatedNumbers_CustomPaging(DateTime? fromDate, DateTime? toDate,
                                                  string number, string type, int? switchID, string carrierAccountID, int pageIndex, int pageSize, out int RecordCount)
        {
            string sql = @"SP_TrafficStats_RepeatedNumber";
            TABS.DataHelper.ParameterGenerator[] parameters = new TABS.DataHelper.ParameterGenerator[] {
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@FromDate", Value=fromDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@ToDate", Value=toDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@Number", Value=number },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@Type", Value=type },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@SwitchID", Value=switchID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@CustomerID", Value=carrierAccountID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageIndex", Value= pageIndex },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageSize", Value= pageSize },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Output, Name="@RecordCount", Value = 0 }
        };

            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            DataTable data = TABS.DataHelper.GetDataTable(sql, parameters, out outputParams);
            RecordCount = (int)outputParams["@RecordCount"];
            return data;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        public static DataTable Ex_Sp_TrafficStats_TopNDestination(DateTime? fromDate, DateTime? toDate, string customerID, string supplierID,
                                                  int topRecords, string sorting)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_TopNDestinationQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, customerID, supplierID, topRecords, sorting);
        }

        public static DataTable Ex_Sp_TrafficStats_ByPeriods(string periodType, string zoneID, string supplierID, string customerID,
                                                  DateTime fromDate, DateTime toDate)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_ByPeriodsQuery();
            return DataHelper.GetDataTable(sql, periodType, zoneID, supplierID, customerID, fromDate, toDate);
        }

        public static DataTable Ex_Sp_TrafficStats_ByOriginatingZone(DateTime fromDate, DateTime toDate, string customerID,
                                                  string supplierID, string ourZoneID)
        {
            string sql = QueryBuilder.Ex_Sp_TrafficStats_ByOriginatingZoneQuery();
            return DataHelper.GetDataTable(sql, fromDate, toDate, customerID, supplierID, ourZoneID);
        }
        //------------------------------------------------------> Custom Paging <---------------------------------------------------------------------------------
        public static DataTable Ex_Sp_TrafficStats_ByOriginatingZone_CustomPaging(DateTime fromDate, DateTime toDate, string customerID,
                                                  string supplierID, string ourZoneID, int pageIndex, int pageSize, out int RecordCount)
        {
            string sql = @"EXEC Sp_TrafficStats_ByOriginatingZone";
            TABS.DataHelper.ParameterGenerator[] parameters = new TABS.DataHelper.ParameterGenerator[] {
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@FromDateTime", Value=fromDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@ToDateTime", Value=toDate },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@CustomerID", Value=customerID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@SupplierID", Value=supplierID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@OurZoneID", Value=ourZoneID },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageIndex", Value= pageIndex },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Input, Name="@PageSize", Value= pageSize },
            new TABS.DataHelper.ParameterGenerator { Direction = ParameterDirection.Output, Name="@RecordCount", Value = 0 }
        };

            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            DataTable data = TABS.DataHelper.GetDataTable(sql, parameters, out outputParams);
            RecordCount = (int)outputParams["@RecordCount"];
            return data;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------
        public static DataTable Ex_Bp_GetPostPaidAccountStats(string showCustomer, string showSupplier, string carrierAccountID, int? carrierProfileID)
        {
            string sql = QueryBuilder.Ex_Bp_GetPostPaidAccountStatsQuery();
            return DataHelper.GetDataTable(sql, showCustomer, showSupplier, carrierAccountID, carrierProfileID);
        }

    }
}
