using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class BlockedAttemptsDataManager : BaseTOneDataManager, IBlockedAttemptsDataManager
    {
        public Vanrise.Entities.BigResult<BlockedAttempts> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptsInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("Customer", "CustomerID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName,input.Query.Filter), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDateTime", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDateTime", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@GroupByNumber", input.Query.GroupByNumber));
                });
            };

            return RetrieveData(input, createTempTableAction, BlockedAttemptsMapper, mapper);
        }

        BlockedAttempts BlockedAttemptsMapper(IDataReader reader)
        {
            BlockedAttempts obj = new BlockedAttempts();
            BlockedAttemptsFromReader(obj, reader);
            return obj;
        }
        void BlockedAttemptsFromReader(BlockedAttempts blockedAttempts, IDataReader reader)
        {
            blockedAttempts.OurZoneID = GetReaderValue<int>(reader, "OurZoneID");
            blockedAttempts.BlockAttempt = GetReaderValue<int>(reader, "BlockAttempt");
            blockedAttempts.ReleaseCode = reader["ReleaseCode"] as string;
            blockedAttempts.SwitchID = GetReaderValue<Byte>(reader, "SwitchID");
            blockedAttempts.ReleaseSource = reader["ReleaseSource"] as string;
            blockedAttempts.CustomerID = reader["CustomerID"] as string;
            blockedAttempts.FirstCall = GetReaderValue<DateTime>(reader, "FirstCall");
            blockedAttempts.LastCall = GetReaderValue<DateTime>(reader, "LastCall");
            blockedAttempts.PhoneNumber = reader["PhoneNumber"] as string;
            blockedAttempts.CLI = reader["CLI"] as string;
        }
        public string CreateTempTableIfNotExists(string tempTableName, BlockedAttemptsFilter filter)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN 
                                SELECT 
		                        BCDRI.OurZoneID,
		                        Count (*) AS BlockAttempt, 
		                        BCDRI.ReleaseCode,
		                        BCDRI.SwitchID,
		                        BCDRI.ReleaseSource, 
		                        case when CustomerID IS NULL then 'N/A' ELSE CustomerID END  AS CustomerID,
		                        DATEADD(ms,-datepart(ms,Min(BCDRI.Attempt)),Min(BCDRI.Attempt)) AS FirstCall,
		                        DATEADD(ms,-datepart(ms,Max(BCDRI.Attempt)),Max(BCDRI.Attempt)) AS LastCall,
		                        case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		                        case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	                            INTO #TEMPTABLE#
	                            FROM  Billing_CDR_Invalid  BCDRI  WITH(NOLOCK)
	                            WHERE
			                    Attempt Between @FromDateTime And @ToDateTime
		                        AND DurationInSeconds = 0
		                        AND SupplierID IS NULL
		                        #FILTER#
                               GROUP BY ReleaseCode,SwitchID,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
                                ORDER BY Count (*) DESC
                            END");
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            return queryBuilder.ToString();
        }
        private void AddFilterToQuery(BlockedAttemptsFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "BCDRI.SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "BCDRI.CustomerID");
            AddFilter(whereBuilder, filter.ZoneIds, "BCDRI.OurZoneID");
        }

        void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }

    }
}
