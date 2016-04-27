using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Data.SQL
{
    public class BlockedAttemptDataManager : BaseTOneDataManager, IBlockedAttemptDataManager
    {

        #region ctor/Local Variables
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        private BlockedAttemptFilter filter;

        static BlockedAttemptDataManager()
        {
            _columnMapper.Add("Entity.ID", "ID");
            _columnMapper.Add("SwitchName", "SwitchID");
            _columnMapper.Add("SaleZoneName", "SaleZoneID");
            _columnMapper.Add("CustomerName", "CustomerID");
            _columnMapper.Add("Entity.BlockedAttempts", "BlockedAttempts");
            _columnMapper.Add("Entity.CGPN", "Caller Number");
            _columnMapper.Add("Entity.CDPN", "Dialed Number");
            _columnMapper.Add("Entity.FirstAttempt", "FirstAttempt");
            _columnMapper.Add("Entity.LastAttempt", "LastAttempt");
            _columnMapper.Add("Entity.ReleaseCode", "ReleaseCode");
            _columnMapper.Add("Entity.ReleaseSource", "ReleaseSource");
        }
        public BlockedAttemptDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<BlockedAttempt> GetBlockedAttemptData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptInput> input)
        {
            filter = input.Query.Filter;

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.To)));
                });
            };
            return RetrieveData(input, createTempTableAction, BlockedAttemptMapper, _columnMapper);
        }

        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName)
        {
            string queryData = GetSingleQuery();

            StringBuilder queryBuilder = new StringBuilder(@" IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
                                                              BEGIN 
                                                                Select newtable.*  INTO #TEMPTABLE# FROM (#Query#) as newtable
                                                                
                                                              END");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#Query#", queryData);
            return queryBuilder.ToString();
        }
        private string GetSingleQuery()
        {
            StringBuilder queryBuilder = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder selectColumnBuilder = new StringBuilder();
            AddSelectColumnToQuery(selectColumnBuilder);
            AddFilterToQuery(whereBuilder);
            AddGroupingToQuery(groupByBuilder);
            queryBuilder.Append(String.Format(@"
                    SELECT   #SELECTCOLUMNPART#
                    FROM [TOneWhS_CDR].[CDRInvalid]
                        WITH(NOLOCK ,INDEX(IX_CDRInvalid_Attempt)) WHERE ( SupplierID is NULL AND DurationInSeconds=0 AND  Attempt>= @FromDate AND (Attempt<= @ToDate OR @ToDate IS NULL))  
                        #WHEREPART#  
                        #GROUPBYPART# 
                        "));

            queryBuilder.Replace("#SELECTCOLUMNPART#", selectColumnBuilder.ToString());
            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());

            return queryBuilder.ToString();

        }
        private BlockedAttempt BlockedAttemptMapper(IDataReader reader)
        {
            BlockedAttempt blockedAttempt = new BlockedAttempt();
            blockedAttempt.CustomerID = GetReaderValue<int>(reader, "CustomerID");
            blockedAttempt.SaleZoneID = GetReaderValue<long>(reader, "SaleZoneID");
            if (filter.GroupByNumber)
            {
                blockedAttempt.CDPN = reader["CDPN"] as string;
                blockedAttempt.CGPN = reader["CGPN"] as string;
            }
            blockedAttempt.ReleaseCode = reader["ReleaseCode"] as string;
            blockedAttempt.ReleaseSource = reader["ReleaseSource"] as string;
            blockedAttempt.BlockedAttempts = (int)reader["BlockedAttempts"];
            blockedAttempt.FirstAttempt = GetReaderValue<DateTime?>(reader, "FirstAttempt");
            blockedAttempt.LastAttempt = GetReaderValue<DateTime?>(reader, "LastAttempt");
            return blockedAttempt;
        }

        private void AddColumnToStringBuilder(StringBuilder builder, string column)
        {
            if (builder.Length > 0)
            {
                builder.Append(" , ");
            }
            builder.Append(column);
        }


        private void AddSelectColumnToQuery(StringBuilder selectColumnBuilder)
        {
            selectColumnBuilder.Append(@" CustomerID,SaleZoneID,ReleaseCode,ReleaseSource,Count (*) AS BlockedAttempts, Min(Attempt) AS FirstAttempt, Max(Attempt) AS LastAttempt ");
            if (filter.GroupByNumber)
                selectColumnBuilder.Append(",CDPN,CGPN");
        }


        private void AddGroupingToQuery(StringBuilder groupBuilder)
        {
            groupBuilder.Append(@" GROUP BY SaleZoneID,ReleaseCode,ReleaseSource,CustomerID ");
            if (filter.GroupByNumber)
                groupBuilder.Append(",CDPN,CGPN");
        }


        private void AddFilterToQuery(StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "SwitchID");
            AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, filter.SaleZoneIds, "SaleZoneID");
        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }
        #endregion

    }
}


