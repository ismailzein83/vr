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

        private BlockedAttemptQuery query;

        public BlockedAttemptDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }

        static BlockedAttemptDataManager()
        {
        }
        #endregion





        #region Public Methods
        public IEnumerable<BlockedAttempt> GetAllFilteredBlockedAttempts(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
        {
            query = input.Query;
            return GetItemsText(GetSingleQuery(), BlockedAttemptMapper, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@TopRecords", input.Query.Top));

                });
        }

        #endregion

        #region Private Methods
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
                    SELECT TOP (@TopRecords)   #SELECTCOLUMNPART#
                    FROM [TOneWhS_CDR].[BillingCDR_Invalid]
                        WITH(NOLOCK ,INDEX(IX_BillingCDR_Invalid_AttemptDateTime)) WHERE SupplierID is NULL AND DurationInSeconds=0 AND  AttemptDateTime>= @FromDate 
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
            if (query.Filter.GroupByNumber)
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
            selectColumnBuilder.Append(@" CustomerID,SaleZoneID,ReleaseCode,ReleaseSource,Count (*) AS BlockedAttempts, Min(AttemptDateTime) AS FirstAttempt, Max(AttemptDateTime) AS LastAttempt ");
            if (query.Filter.GroupByNumber)
                selectColumnBuilder.Append(",CDPN,CGPN");
        }


        private void AddGroupingToQuery(StringBuilder groupBuilder)
        {
            groupBuilder.Append(@" GROUP BY SaleZoneID,ReleaseCode,ReleaseSource,CustomerID ");
            if (query.Filter.GroupByNumber)
                groupBuilder.Append(",CDPN,CGPN");
        }


        private void AddFilterToQuery(StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, query.Filter.SwitchIds, "SwitchID");
            AddFilter(whereBuilder, query.Filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, query.Filter.SaleZoneIds, "SaleZoneID");

            if (query.To.HasValue)
                whereBuilder.AppendFormat("AND AttemptDateTime<= '{0}' ", query.To.Value);

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


