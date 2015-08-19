using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class BlockedAttemptsDataManager : BaseTOneDataManager, IBlockedAttemptsDataManager
    {
        private bool groupByNumber;
        private TrafficStatisticCommon trafficStatisticCommon = new TrafficStatisticCommon();
        public Vanrise.Entities.BigResult<BlockedAttempts> GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptsInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("CustomerName", "CustomerID");
            groupByNumber = input.Query.GroupByNumber;
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.GroupByNumber), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDateTime", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDateTime", input.Query.To));
                });
            };
            Vanrise.Entities.BigResult<BlockedAttempts> data=RetrieveData(input, createTempTableAction, BlockedAttemptsMapper, mapper);
            FillProperties(data);
            return data;
        }


        public void FillProperties(Vanrise.Entities.BigResult<BlockedAttempts> Data)
        {
            BusinessEntityInfoManager manager = new BusinessEntityInfoManager();

            foreach (BlockedAttempts data in Data.Data)
            {
                if(data.CustomerID!=null)
                  data.CustomerName= manager.GetCarrirAccountName(data.CustomerID);
                else
                  data.CustomerName="N/A";
              data.SwitchName= manager.GetSwitchName(data.SwitchID);
               data.OurZoneName= manager.GetZoneName(data.OurZoneID);
       
            }    
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
            if (groupByNumber)
            {
                blockedAttempts.PhoneNumber = reader["PhoneNumber"] as string;
                blockedAttempts.CLI = reader["CLI"] as string;
            }

        }
        public string CreateTempTableIfNotExists(string tempTableName, BlockedAttemptsFilter filter ,bool groupByNumber)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder groupBuilder = new StringBuilder();
            StringBuilder selectBuilder = new StringBuilder();
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
		                        DATEADD(ms,-datepart(ms,Max(BCDRI.Attempt)),Max(BCDRI.Attempt)) AS LastCall
                                #SELECTPART# 
	                            INTO #TEMPTABLE#
	                            FROM  Billing_CDR_Invalid  BCDRI  WITH(NOLOCK)
	                            WHERE
			                    Attempt Between @FromDateTime And @ToDateTime
		                        AND DurationInSeconds = 0
		                        AND SupplierID IS NULL
		                        #FILTER#
                               GROUP BY ReleaseCode,SwitchID,ReleaseSource,OurZoneID,CustomerID #GROUPINGPART#

                                ORDER BY Count (*) DESC
                            END");

            AddFilterToQuery(filter, whereBuilder);
            if (groupByNumber)
            {
                selectBuilder.Append(",CDPN as PhoneNumber,CGPN as CLI");
                groupBuilder.Append(",CDPN,CGPN");
            }

            queryBuilder.Replace("#GROUPINGPART#", groupBuilder.ToString());
            queryBuilder.Replace("#SELECTPART#", selectBuilder.ToString());
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
