using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities;
using Vanrise.Common;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class ReleaseCodeDataManager : BaseTOneDataManager, IReleaseCodeDataManager
    {
        #region ctor/Local Variables
        private string mainCDRTableName = "[TOneWhS_CDR].[BillingCDR_Main]";
        private string mainCDRTableAlias = "MainCDR";
        private string mainCDRTableIndex = "IX_BillingCDR_Main_AttemptDateTime";

        private string invalidCDRTableName = "[TOneWhS_CDR].[BillingCDR_Invalid]";
        private string invalidCDRTableAlias = "InValidCDR";
        private string invalidCDRTableIndex = "IX_BillingCDR_Invalid_AttemptDateTime";

        private string failedCDRTableName = "[TOneWhS_CDR].[BillingCDR_Failed]";
        private string failedCDRTableAlias = "FailedCDR";
        private string failedCDRTableIndex = "IX_BillingCDR_Failed_AttemptDateTime";
        private int totalCount;
        public ReleaseCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {
        }
        #endregion
        private ReleaseCodeQuery query;
        private List<string> codes;
        #region Public Methods

        public List<ReleaseCodeStat> GetAllFilteredReleaseCodes(Vanrise.Entities.DataRetrievalInput<Entities.ReleaseCodeQuery> input, List<string> salecodesIds)
        {
            query = input.Query;
            codes = salecodesIds;
            List<ReleaseCodeStat> releaseCodes = new List<ReleaseCodeStat>();
            ExecuteReaderText(GetQuery(input.Query.Filter), (reader) =>
            {

                while (reader.Read())
                {

                    totalCount = GetReaderValue<int>(reader, "TotalAttempts");
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        ReleaseCodeStat releaseCode = new ReleaseCodeStat();
                        releaseCode.SwitchId = GetReaderValue<int>(reader, "SwitchId");
                        releaseCode.ReleaseCode = reader["ReleaseCode"] as string;
                        releaseCode.ReleaseSource = reader["ReleaseSource"] as string;
                        releaseCode.Attempt = GetReaderValue<int>(reader, "Attempt");
                        releaseCode.DurationInMinutes = GetReaderValue<decimal>(reader, "DurationsInMinutes");
                        releaseCode.FailedAttempt = releaseCode.Attempt - (int)reader["SuccessfulAttempts"];
                        releaseCode.FirstAttempt = GetReaderValue<DateTime?>(reader, "FirstAttempt");
                        releaseCode.LastAttempt = GetReaderValue<DateTime?>(reader, "LastAttempt");
                        releaseCode.Percentage = decimal.Round(((decimal)GetReaderValue<int>(reader, "Attempt") * 100 / totalCount), 2);
                        if (query.Filter.Dimession != null && query.Filter.Dimession.Count() > 0)
                        {
                            if (query.Filter.Dimession.Contains(ReleaseCodeDimension.Supplier))
                            {
                                releaseCode.SupplierId = GetReaderValue<int>(reader, "SupplierID");
                            }
                            if (query.Filter.Dimession.Contains(ReleaseCodeDimension.MasterZone))
                            {
                                releaseCode.MasterPlanZoneId = GetReaderValue<long>(reader, "MasterPlanZoneID");
                            }
                            if (query.Filter.Dimession.Contains(ReleaseCodeDimension.PortIn))
                            {
                                releaseCode.PortIn = reader["PortIN"] as string;
                            }
                            if (query.Filter.Dimession.Contains(ReleaseCodeDimension.PortOut))
                            {
                                releaseCode.PortOut = reader["PortOUT"] as string;
                            }
                        }
                        releaseCodes.Add(releaseCode);
                    }
                }
            }, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                if (input.Query.To.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));


            });
            return releaseCodes;
        }

        #endregion

        #region Private Methods
        private string GetQuery(ReleaseCodeFilter filter)
        {
            string aliasTableName = "newtable";
            StringBuilder selectColumnBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();
            string queryData = String.Format(@"{0} UNION ALL {1}  UNION ALL {2}",
                        GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex),
                        GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex),
                        GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex));

            StringBuilder queryBuilder = new StringBuilder(String.Format(@"  SELECT *
                                                                INTO #RESULT FROM (#Query#) {0}
                                                                SELECT SUM(Attempt) AS TotalAttempts FROM #RESULT
                                                                SELECT #SELECTCOLUMNPART# FROM #RESULT  GROUP BY #GROUPBYPART# ", aliasTableName));
            selectColumnBuilder.Append(String.Format(@"    SwitchId,
                                                            ReleaseCode,
                                                            ReleaseSource,
                                                            Count(*)  Attempt,
                                                            SUM(SuccessfulAttempts) SuccessfulAttempts,
			                                                Min(FirstAttempt) FirstAttempt,	
			                                                Max(LastAttempt) LastAttempt,
                                                            SUM(DurationsInMinutes) AS DurationsInMinutes  "));
            AddSelectColumnToGenaralQuery(selectColumnBuilder);
            groupByBuilder.Append(String.Format(@"SwitchId,ReleaseCode,ReleaseSource"));
            AddGroupingToGenaralQuery(groupByBuilder);

            queryBuilder.Replace("#SELECTCOLUMNPART#", selectColumnBuilder.ToString());

            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());

            queryBuilder.Replace("#Query#", queryData);
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
                                               FROM {0} {1} WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                               WHERE (#WHEREPART#)   
                                               GROUP BY #GROUPBYPART#                                              
                                              ", tableName, alias));

            whereBuilder.Append(String.Format(@"{0}.AttemptDateTime>= @FromDate ", alias));
            if (query.To.HasValue)
                whereBuilder.AppendFormat("AND {0}.AttemptDateTime<= @ToDate  ", alias);
            groupByBuilder.Append(String.Format(@"{0}.SwitchId, {0}.ReleaseCode,{0}.ReleaseSource", alias));
            selectQueryPart.Append(String.Format(@"         {0}.SwitchId,
                                                            {0}.ReleaseCode,{0}.ReleaseSource,
                                                            Count(*)  Attempt,
                                                            SUM( CASE WHEN {0}.DurationInSeconds > 0 THEN 1 ELSE 0 END) SuccessfulAttempts,
			                                                Min({0}.AttemptDateTime) FirstAttempt,	
			                                                Max({0}.AttemptDateTime) LastAttempt,
                                                            CONVERT(DECIMAL,SUM({0}.DurationInSeconds)/60.) AS DurationsInMinutes  ", alias, tableIndex));
            AddSelectColumnToQuery(selectQueryPart, alias);
            AddGroupingToQuery(groupByBuilder, alias);
            queryBuilder.Replace("#SELECTPART#", selectQueryPart.ToString());
            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());
            queryBuilder.Replace("#TABLEINDEX#", tableIndex);

            return queryBuilder.ToString();
        }

        private void AddSelectColumnToGenaralQuery(StringBuilder selectColumnBuilder)
        {
            if (query.Filter.Dimession != null && query.Filter.Dimession.Count() > 0)
            {

                foreach (ReleaseCodeDimension a in Enum.GetValues(typeof(ReleaseCodeDimension)))
                {
                    if (query.Filter.Dimession.Contains(a))
                    {
                        selectColumnBuilder.Append(String.Format(@",{0}", Utilities.GetEnumDescription(a)));
                    }
                }

            }


        }
        private void AddGroupingToGenaralQuery(StringBuilder groupBuilder)
        {
            if (query.Filter.Dimession != null && query.Filter.Dimession.Count() > 0)
            {

                foreach (ReleaseCodeDimension a in Enum.GetValues(typeof(ReleaseCodeDimension)))
                {
                    if (query.Filter.Dimession.Contains(a))
                    {
                        groupBuilder.Append(String.Format(@",{0}", Utilities.GetEnumDescription(a)));
                    }
                }

            }
        }



        private void AddSelectColumnToQuery(StringBuilder selectColumnBuilder, string aliasTableName)
        {
            if (query.Filter.Dimession != null && query.Filter.Dimession.Count() > 0)
            {

                foreach (ReleaseCodeDimension a in Enum.GetValues(typeof(ReleaseCodeDimension)))
                {
                    if (query.Filter.Dimession.Contains(a))
                    {
                        selectColumnBuilder.Append(String.Format(@",{0}.{1}", aliasTableName, Utilities.GetEnumDescription(a)));
                    }
                }

            }


        }
        private void AddGroupingToQuery(StringBuilder groupBuilder, string aliasTableName)
        {
            if (query.Filter.Dimession != null && query.Filter.Dimession.Count() > 0)
            {

                foreach (ReleaseCodeDimension a in Enum.GetValues(typeof(ReleaseCodeDimension)))
                {
                    if (query.Filter.Dimession.Contains(a))
                    {
                        groupBuilder.Append(String.Format(@",{0}.{1}", aliasTableName, Utilities.GetEnumDescription(a)));
                    }
                }

            }
        }

        private void AddFilterToQuery(ReleaseCodeFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "SwitchId");
            AddFilter(whereBuilder, filter.SupplierIds, "SupplierID");
            AddFilter(whereBuilder, filter.CustomerIds, "CustomerID");
            AddFilter(whereBuilder, codes, "SaleCode");


        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null)
            {
                if (values.Count() > 0)
                {
                    if (typeof(T) == typeof(string))
                        whereBuilder.AppendFormat(" {0} IN ('{1}') AND ", column, String.Join("', '", values));
                    else
                        whereBuilder.AppendFormat(" {0} IN ({1}) AND ", column, String.Join(", ", values));
                }
                else if (values.Count() == 0)
                {
                    whereBuilder.AppendFormat(" 0 = 1 AND ");
                }
            }
        }

        #endregion
    }
}
