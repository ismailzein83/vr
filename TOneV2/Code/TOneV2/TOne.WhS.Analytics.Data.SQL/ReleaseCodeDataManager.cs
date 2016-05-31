using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class ReleaseCodeDataManager : BaseTOneDataManager, IReleaseCodeDataManager
    {
        #region ctor/Local Variables
        private string mainCDRTableName = "[TOneWhS_CDR].[BillingCDR_Main]";
        private string mainCDRTableAlias = "MainCDR";
        private string mainCDRTableIndex = "BillingCDR_Main_AttemptDateTime";

        private string invalidCDRTableName = "[TOneWhS_CDR].[BillingCDR_Invalid]";
        private string invalidCDRTableAlias = "InValidCDR";
        private string invalidCDRTableIndex = "BillingCDR_Invalid_AttemptDateTime";

        private string failedCDRTableName = "[TOneWhS_CDR].[BillingCDR_Failed]";
        private string failedCDRTableAlias = "FailedCDR";
        private string failedCDRTableIndex = "BillingCDR_Failed_AttemptDateTime";
        private long totalCount = 0;
        public ReleaseCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {
        }
        #endregion

        #region Public Methods

        public IEnumerable<ReleaseCode> GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<Entities.ReleaseCodeQuery> input)
        {
            DateTime toDate = DateTime.MinValue;
            if(input.Query.To == DateTime.MinValue)
                toDate = DateTime.Now;
            return GetItemsText(GetQuery(input.Query.Filter), ReleaseCodeDataMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(toDate)));
            });
        }

        #endregion

        #region Private Methods
        private string GetQuery(ReleaseCodeFilter filter)
        {
            string aliasTableName = "newtable";
            StringBuilder selectColumnBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();
            AddSelectColumnToQuery(selectColumnBuilder, aliasTableName);
            AddGroupingToQuery(groupByBuilder, aliasTableName);
            AddHavingToQuery(havingBuilder, aliasTableName);
            string queryData = String.Format(@"{0} UNION ALL {1}  UNION ALL {2}",
                        GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex),
                        GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex),
                        GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex));
               
            StringBuilder queryBuilder = new StringBuilder(String.Format(@"  SELECT #SELECTCOLUMNPART#
                                                                FROM (#Query#) {0}
                                                                GROUP BY #GROUPBYPART#
                                                                HAVING #HAVINGPART# ", aliasTableName));
            queryBuilder.Replace("#SELECTCOLUMNPART#", selectColumnBuilder.ToString());
            queryBuilder.Replace("#Query#", queryData);
            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());
            queryBuilder.Replace("#HAVINGPART#", havingBuilder.ToString());
            return queryBuilder.ToString();
        }
        private string GetSingleQuery(string tableName, string alias, ReleaseCodeFilter filter, string tableIndex)
        {
            StringBuilder queryBuilder = new StringBuilder();
            StringBuilder selectQueryPart = new StringBuilder();
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Append(String.Format(@"SELECT                                                
                                               #SELECTPART#
                                               INTO #RESULT
                                               FROM {0} {1} WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                               WHERE (#WHEREPART#)   
                                               GROUP BY #GROUPBYPART# 
                                               SELECT COUNT(*) AS TotalAttempts FROM #RESULT
                                               SELECT * FROM #RESULT", tableName, alias));

            whereBuilder.Append(String.Format(@"{0}.AttemptDateTime>= @FromDate AND ({0}.AttemptDateTime<= @ToDate)", alias));
            groupByBuilder.Append(String.Format(@"SwitchId, ReleaseCode,ReleaseSource"));
            selectQueryPart.Append(String.Format(@" {0}.SaleZoneID, 
                                                            {0}.CustomerID,
                                                            {0}.SupplierID,
                                                            
                                                            Count({0}.AttemptDateTime)  Attempt,
			                                                Sum({0}.SuccessfulAttempts) SuccessfulAttempts,
			                                                Sum({0}.Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			                                                Min({0}.FirstAttempt) FirstAttempt,	
			                                                Max({0}.LastAttempt) LastAttempt,,
                                                            CONVERT(DECIMAL(10,2),SUM({0}.DurationInSeconds)/60.) AS DurationsInMinutes  ", alias));

            queryBuilder.Replace("#SELECTPART#", selectQueryPart.ToString());
            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());
            queryBuilder.Replace("#HAVINGPART#", havingBuilder.ToString());
            queryBuilder.Replace("#TABLEINDEX#", tableIndex);

            return queryBuilder.ToString();
        }
        private void AddSelectColumnToQuery(StringBuilder selectColumnBuilder, string aliasTableName)
        {
            //selectColumnBuilder.Append(String.Format(@" {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, SUM({0}.Attempt) AS Attempt,
                                                    //SUM({0}.DurationsInMinutes) AS DurationsInMinutes, {0}.PhoneNumber", aliasTableName));
        }
        private void AddGroupingToQuery(StringBuilder groupBuilder, string aliasTableName)
        {
           // groupBuilder.Append(String.Format(@"  {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, {0}.PhoneNumber ", aliasTableName));
        }
        private void AddHavingToQuery(StringBuilder groupBuilder, string aliasTableName)
        {
            //groupBuilder.Append(String.Format(@"  SUM({0}.Attempt) >=  {1} ", aliasTableName));
        }        
        private void AddFilterToQuery(ReleaseCodeFilter filter, StringBuilder whereBuilder)
        {
            //AddFilter(whereBuilder, filter.SwitchIds, "SwitchId");
        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            //if (values != null && values.Count() > 0)
            //{
            //    if (typeof(T) == typeof(string))
            //        whereBuilder.AppendFormat(" {0} IN ('{1}') AND ", column, String.Join("', '", values));
            //    else
            //        whereBuilder.AppendFormat(" {0} IN ({1}) AND ", column, String.Join(", ", values));
            //}
        }
        private ReleaseCode ReleaseCodeDataMapper(IDataReader reader)
        {
            ReleaseCode releaseCode = new ReleaseCode();
            while (reader.Read())
            {
                totalCount = GetReaderValue<long>(reader, "TotalAttempts");
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                   
                    releaseCode.SaleZoneId = GetReaderValue<long>(reader, "SaleZoneID");
                    if (totalCount != 0)
                    {
                        releaseCode.Percentage = GetReaderValue<long>(reader, "Attempts") * 100 / totalCount;
                    }
                    
                }


            }
            return releaseCode;
           
        }
        #endregion
    }
}
