using System;

namespace TABS.DAL
{
    public partial class QueryBuilder
    {
        public static string Ex_SP_TrafficStats_BlockedAttempts()
        {
            string sql = @"EXEC SP_TrafficStats_BlockedAttempts
                            @CustomerID=@P1, 
                            @OurZoneID=@P2,
                            @FromDateTime=@p3,  
	                        @ToDateTime =@p4,
                            @GroupByNumber = @P5";
            return sql;
        }
        //----Custom paging
        public static string Ex_SP_TrafficStats_BlockedAttempts_CustomPaging()
        {
            string sql = @"SP_TrafficStats_BlockedAttempts";
            return sql;
        }

        //-----------------
        public static string Ex_Sp_Traffic_CDRQuery(int? cdrOption, string fromDuration, string toDuration, DateTime? fromDate,
                                                  DateTime? toDate, int? limitResult, string supplierID, string customerID,
                                                  string ourZoneID, string number, string cli, string releaseCode)
        {
            string sql = @"exec sp_Traffic_CDR
                                    @CDROption       ={0},
                                    @FromDuration    ={1},
                                    @ToDuration      ={2},
                                    @FromDate        ='{3:yyyy-MM-dd HH:mm:sss}',
                                    @ToDate          ='{4:yyyy-MM-dd HH:mm:sss}',
                                    @TopRecord       ={5},
                                    @SupplierID      ={6},
                                    @CustomerID      ={7},
                                    @OurZoneID       ={8},
                                    @Number          ={9},
                                    @CLI             ={10},
                                    @ReleaseCode     ={11}";
            return string.Format(sql, cdrOption, fromDuration, toDuration, fromDate, toDate, limitResult, supplierID, customerID,
                                ourZoneID, number, cli, releaseCode);
        }

        public static NHibernate.IQuery GetCustomerRatesQuery(CarrierAccount account, DateTime effectiveDate)
        {
            NHibernate.IQuery query = DataConfiguration.CurrentSession
                       .CreateQuery(@"FROM Rate R 
                                    WHERE 
                                    R.PriceList.Customer =:customer
                                    AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)")
                        .SetParameter("customer", account)
                        .SetParameter("when", effectiveDate);
            return query;
        }
        public static string Ex_Ea_SupplierSummaryQuery(DateTime fromDate, DateTime toDate, string supplierID)
        {
            string sql = string.Format(@"EXEC EA_SupplierSummary
                                                    @FromDate = '{0}',
                                                    @ToDate = '{1}',
                                                    @SupplierID = '{2}'"
                                                  , fromDate.ToString("yyyy-MM-dd")
                                                  , toDate.ToString("yyyy-MM-dd")
                                                  , supplierID);
            return sql;
        }
        public static string Ex_Ea_CustomerSummaryQuery(DateTime fromDate, DateTime toDate, string customerID)
        {
            string sql = string.Format(@"EXEC EA_CustomerSummary
                                                    @FromDate = '{0}',
                                                    @ToDate = '{1}',
                                                    @CustomerID = '{2}'"
                                                  , fromDate.ToString("yyyy-MM-dd")
                                                  , toDate.ToString("yyyy-MM-dd")
                                                  , customerID);
            return sql;
        }

        public static string Ex_Sp_TrafficStats_HourlyReportQuery()
        {
            string sql = @"EXEC SP_TrafficStats_HourlyReport 
                             @FromDateTime =@P1,
    	                     @ToDateTime  = @P2,
    	                     @CustomerID =@P3,
    	                     @SupplierID =@P4,
                             @OurZoneID =@P5,
                             @CodeGroup=@P6";
            return sql;
        }

        public static string Ex_Sp_TrafficStats_ReleaseCodeStatsQuery()
        {
            string sql = @"EXEC Sp_TrafficStats_ReleaseCodeStats                                   
                                    @FromDate          =@p1,
                                    @ToDate            =@p2 ,
                                    @CustomerID        =@p3,
                                    @SupplierID        =@p4,
                                    @OurZoneID         =@p5
                                    ";
            return sql;
        }

        public static string Ex_Sp_TrafficStats_RepeatedNumberQuery()
        {
            string sql = @"EXEC SP_TrafficStats_RepeatedNumber  
                            @FromDate=@P1, 
                            @ToDate=@P2, 
                            @Number=@P3,
                            @Type=@P4,
                            @SwitchID=@P5,
                            @CustomerID=@P6";
            return sql;
        }

        public static string Ex_Sp_TrafficStats_TopNDestinationQuery()
        {
            string sql = @"EXEC [EA_TrafficStats_TopNDestination]
                            @FromDate=@P1, 
                            @ToDate=@P2, 
                            @CustomerID=@P3, 
                            @SupplierID=@P4,
                            @TopRecords=@P5,
                            @HighestTraffic=@p6";
            return sql;
        }

        public static string Ex_Sp_TrafficStats_ByPeriodsQuery()
        {
            string sql = @"EXEC sp_TrafficStats_ByPeriods
                              @PeriodType=@P1, 
                              @ZoneID=@P2,
                              @SupplierID=@P3,
                              @CustomerID=@P4,
                              @FromDate=@P5,
                              @TillDate=@P6";
            return sql;
        }

        public static string Ex_Sp_TrafficStats_ByOriginatingZoneQuery()
        {
            string sql = @"EXEC Sp_TrafficStats_ByOriginatingZone
                            @FromDateTime=@P1, 
                            @ToDateTime=@P2, 
                            @CustomerID=@P3,
                            @SupplierID=@p4, 
                            @OurZoneID =@P5";
            return sql;
        }

        public static string Ex_Bp_GetPostPaidAccountStatsQuery()
        {
            return @"EXEC bp_GetPostPaidAccountStats @ShowCustomerTotal=@P1, @ShowSupplierTotal=@P2,@CarrierAccountID=@P3,@CarrierProfileID=@P4";
        }

    }
}
