
namespace TABS.DAL
{
    public partial class QueryBuilder
    {
        public static string Ex_Bp_BuildBillingStatsPeriodDefinedQuery()
        {
            return @"EXEC bp_BuildBillingStatsPeriodDefined
                                           @CustomerID = @P1
	                                       @FromDate = @P2,
	                                       @ToDate   = @P3";
        }

        public static string Ex_BP_ErroneousPricedCDRQuery()
        {
            return @"EXEC [bp_ErroneousPricedCDR]
	                       @CarrierAccountID = @P1,
                           @IsSale = @P2,    
	                       @FromDate = @P3,
	                       @TillDate = @P4";
        }

        public static string Ex_Bp_CreateSupplierInvoiceGroupByDayQuery()
        {
            return @" EXEC bp_CreateSupplierInvoiceGroupByDay 
                                @SupplierID=@P1,
                                @FromDate=@P2,
                                @ToDate=@P3,
                                @GMTShifttime=@P4";
        }

        public static string DeleteBillingInvoiceQuery(int invoiceID)
        {
            return string.Format(@"
                DELETE FROM Billing_Invoice_Details  WHERE InvoiceID = {0}
                DELETE FROM Billing_Invoice WHERE InvoiceID = {0}", invoiceID);
        }

        public static string GetStatsVolumesQuery(string fromDate, string toDate)
        {
            return string.Format(@"WITH OurZone AS (
                                                    SELECT *
                                                    FROM   Zone z 
                                                    WHERE  z.SupplierID = 'SYS'
                                                    )
                                   SELECT Z.ZoneID AS ZoneID, SUM(tsd.DurationsInSeconds) / 60.0 AS Volume
                                   FROM   OurZone Z 
                                   LEFT   JOIN  TrafficStatsDaily tsd WITH(NOLOCK) ON  Z.ZoneID = tsd.OurZoneID
                                    AND tsd.CallDate BETWEEN '{0}' AND '{1}' 
                                   GROUP  BY Z.Zoneid", fromDate, toDate);
            //return string.Format(@"WITH OurZone AS (
            //                         SELECT *
            //                         FROM   Zone z 
            //                         WHERE  z.SupplierID = 'SYS'
            //                         )
            //                         SELECT Z.ZoneID, SUM(bs.SaleDuration) / 60.0
            //                         FROM   OurZone Z 
            //                         LEFT   JOIN  Billing_Stats bs WITH(NOLOCK) ON  Z.ZoneID = bs.SaleZoneID
            //                                AND bs.CallDate BETWEEN '{0}' AND '{1}' 
            //                         GROUP  BY
            //                                Z.Zoneid", fromDate, toDate);
        }

        public static string Ex_Sp_TrafficStats_BySupplierSaleZone_EnhancedQuery()
        {
            return @"EXEC [SP_TrafficStats_BySupplierSaleZone_Enhanced]
                                                          @fromDate = @P1,
                                                          @ToDate = @P2,
                                                          @CustomerID = @P3";
        }
    }
}
