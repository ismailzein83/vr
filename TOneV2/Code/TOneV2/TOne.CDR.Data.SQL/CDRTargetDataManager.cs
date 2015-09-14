using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class CDRTargetDataManager : BaseTOneDataManager, ICDRTargetDataManager
    {
        //public CDRTargetDataManager()
        //    : base("CDRTargetDBConnString")
        //{
        //}

        //public long GetMinCDRMainID()
        //{
        //    string sql = @"SELECT Min(ID) FROM Billing_CDR_Main WITH(NOLOCK)";
        //    object idAsObj = ExecuteScalarText(sql, null);
        //    return idAsObj != DBNull.Value ? (long)idAsObj : 0;
        //}

        //public long GetMinCDRInvalidID()
        //{
        //    string sql = @"SELECT Min(ID) FROM Billing_CDR_Invalid WITH(NOLOCK)";
        //    object idAsObj = ExecuteScalarText(sql, null);
        //    return idAsObj != DBNull.Value ? (long)idAsObj : 0;
        //}

       




     

        public void DeleteDailyTrafficStats(DateTime date)
        {
            ExecuteNonQueryText("DELETE TrafficStatsDaily FROM TrafficStatsDaily WITH(NOLOCK, INDEX(IX_TrafficStatsDaily_DateTimeFirst)) WHERE  calldate = @Date",
               (cmd) =>
               {
                   cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Date", date));
               });
        }

        public void UpdateDailyPostpaid(DateTime date)
        {
            ExecuteNonQueryText("EXEC bp_PostpaidDailyTotalUpdate @FromCallDate = @date, @ToCallDate = @date",
                (cmd) =>
                {
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@date", date));
                });
        }

        public void UpdateDailyPrepaid(DateTime date)
        {
            ExecuteNonQueryText("EXEC bp_PrepaidDailyTotalUpdate @FromCallDate = @date, @ToCallDate = @date",
                (cmd) =>
                {
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@date", date));
                });
        }

        public void UpdateDailyBillingStatistics(DateTime date)
        {
            ExecuteNonQueryText("EXEC bp_BuildBillingStats @Day = @date, @CustomerID = NULL",
                (cmd) =>
                {
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@date", date));
                });
        }

        //public void DeleteDailyTrafficStats(DateTime from, DateTime to)
        //{
        //    throw new NotImplementedException();
        //}

        #region Queries


        const string query_DeleteTemplate = @"DELETE {0} FROM {0} WITH(NOLOCK, INDEX({3})) WHERE  {1} >= @From AND {1} < @To --{2}";

        #region temp

//        const string query_DeleteCDRInvalid = @"declare @sql varchar(1000), @viewName varchar(50)
//set @viewName = 'vwCDRInvalid_' + Replace(Replace(Convert(varchar, GETDATE(), 9), ' ', ''), ':', '')
//
//set @sql='create view ' + @viewName + ' as (SELECT *
//  FROM  [dbo].[Billing_CDR_Invalid]
//  WHERE Attempt >= Convert(Datetime, ''' + Convert(varchar, @From, 25) + ''') AND Attempt < Convert(Datetime, ''' + Convert(varchar, @To, 25) + ''') )'
//exec (@sql)
//
//set @sql = 'Delete ' + @viewName
//exec (@sql)
//
//set @sql = 'Drop View ' + @viewName
//exec (@sql)";


        //        const string query_DeleteCDRMain = @"DELETE FROM [Billing_CDR_Main] WITH(NOLOCK
        //      WHERE Attempt >= @From AND Attempt < @To";

        //        const string query_DeleteCDRInvalid = @"DELETE FROM [Billing_CDR_Invalid] WITH(NOLOCK
        //      WHERE Attempt >= @From AND Attempt < @To";

        //        const string query_DeleteCDRSale = @"DELETE FROM [Billing_CDR_Sale] WITH(NOLOCK
        //      WHERE Attempt >= @From AND Attempt < @To";

        //        const string query_DeleteCDRCost = @"DELETE FROM [Billing_CDR_Cost]
        //      WHERE Attempt >= @From AND Attempt < @To";

        //        const string query_DeleteTrafficStats = @"DELETE FROM [TrafficStats]
        //      WHERE FirstCDRAttempt  >= @From AND FirstCDRAttempt < @To";

        //        const string query_DeleteTemplate = @"declare @sql varchar(1000), @viewName varchar(50)
        //set @viewName = 'vw{0}_{2}'
        //
        //set @sql='create view ' + @viewName + ' as (SELECT *
        //  FROM  {0} WITH(NOLOCK, INDEX({3}))
        //  WHERE {1} >= Convert(Datetime, ''' + Convert(varchar, @From, 25) + ''') AND {1} < Convert(Datetime, ''' + Convert(varchar, @To, 25) + ''') )'
        //exec (@sql)
        //
        //set @sql = 'Delete ' + @viewName
        //exec (@sql)
        //
        //set @sql = 'Drop View ' + @viewName
        //exec (@sql)";

        #endregion

       

        #endregion

    }
}
