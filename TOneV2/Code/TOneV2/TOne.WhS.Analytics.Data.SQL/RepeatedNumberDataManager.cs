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
    public class RepeatedNumberDataManager : BaseTOneDataManager, IRepeatedNumberDataManager
    {
        #region ctor/Local Variables
        private string mainCDRTableName="[TOneWhS_CDR].[CDRMain]";
        private string mainCDRTableAlias = "MainCDR";
        private string mainCDRTableIndex = "IX_CDRMain_Attempt";

        private string invalidCDRTableName = "[TOneWhS_CDR].[CDRInvalid]";
        private string invalidCDRTableAlias = "InValidCDR";
        private string invalidCDRTableIndex = "IX_CDRInvalid_Attempt";

        private string failedCDRTableName = "[TOneWhS_CDR].[CDRFailed]";
        private string failedCDRTableAlias = "FailedCDR";
        private string failedCDRTableIndex = "IX_CDRFailed_Attempt";

        public RepeatedNumberDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {
        }
        #endregion

        #region Public Methods

        public IEnumerable<RepeatedNumber> GetAllFilteredRepeatedNumbers(Vanrise.Entities.DataRetrievalInput<Entities.RepeatedNumberQuery> input)
        {
            return GetItemsText(GetQuery(input.Query.Filter, input.Query.CDRType, input.Query.RepeatedMorethan), RepeatedNumberDataMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.To)));
                cmd.Parameters.Add(new SqlParameter("@PhoneNumberType", input.Query.PhoneNumber));
            });
        }

        #endregion

        #region Private Methods
        private string GetQuery(RepeatedNumberFilter filter, CDRType cdrType, int repeatedMorethan)
        {
            string aliasTableName = "newtable";
            StringBuilder selectColumnBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();
            AddSelectColumnToQuery(selectColumnBuilder, aliasTableName);
            AddGroupingToQuery(groupByBuilder, aliasTableName);
            AddHavingToQuery(havingBuilder, aliasTableName, repeatedMorethan);
            string queryData = "";
            switch (cdrType)
            {
                case CDRType.All:
                    queryData = String.Format(@"{0} UNION ALL {1}  UNION ALL {2}", GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex, repeatedMorethan),
                        GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex, repeatedMorethan),
                        GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex, repeatedMorethan));
                    break;
                case CDRType.Invalid:
                    queryData = GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex, repeatedMorethan);
                    break;
                case CDRType.Failed:
                    queryData = GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex, repeatedMorethan);
                    break;
                case CDRType.Successful:
                    queryData = GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex, repeatedMorethan);
                    break;
            }

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
        private string GetSingleQuery(string tableName, string alias, RepeatedNumberFilter filter, string tableIndex, int repeatedMorethan)
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
                                               GROUP BY #GROUPBYPART# ", tableName, alias));

            whereBuilder.Append(String.Format(@"{0}.Attempt>= @FromDate AND ({0}.Attempt<= @ToDate OR @ToDate IS NULL)", alias));
            groupByBuilder.Append(@"SaleZoneID, CustomerID, SupplierID, CASE @PhoneNumberType WHEN 'CDPN' THEN CDPN  WHEN 'CGPN' THEN CGPN  END");
            havingBuilder.Append(String.Format(@"HAVING SUM(Attempt) >= {0} ", repeatedMorethan));
            selectQueryPart.Append(String.Format(@" {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, Count({0}.Attempt) AS Attempt,
                                                    CONVERT(DECIMAL(10,2),SUM({0}.DurationInSeconds)/60.) AS DurationsInMinutes,
		                                            CASE @PhoneNumberType WHEN 'CDPN' THEN {0}.CDPN  WHEN 'CGPN' THEN {0}.CGPN  END AS PhoneNumber ", alias));

            queryBuilder.Replace("#SELECTPART#", selectQueryPart.ToString());
            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByBuilder.ToString());
            queryBuilder.Replace("#HAVINGPART#", havingBuilder.ToString());
            queryBuilder.Replace("#TABLEINDEX#", tableIndex);

            return queryBuilder.ToString();
        }
        private void AddSelectColumnToQuery(StringBuilder selectColumnBuilder, string aliasTableName)
        {
            selectColumnBuilder.Append(String.Format(@" {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, SUM({0}.Attempt) AS Attempt,
                                                    SUM({0}.DurationsInMinutes) AS DurationsInMinutes, {0}.PhoneNumber", aliasTableName));
        }
        private void AddGroupingToQuery(StringBuilder groupBuilder, string aliasTableName)
        {
            groupBuilder.Append(String.Format(@"  {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, {0}.PhoneNumber ", aliasTableName));
        }
        private void AddHavingToQuery(StringBuilder groupBuilder, string aliasTableName, int repeatedMorethan)
        {
            groupBuilder.Append(String.Format(@"  SUM({0}.Attempt) >=  {1} ", aliasTableName, repeatedMorethan));
        }        
        private void AddFilterToQuery(RepeatedNumberFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "SwitchId");
        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat(" {0} IN ('{1}') AND ", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat(" {0} IN ({1}) AND ", column, String.Join(", ", values));
            }
        }
        private RepeatedNumber RepeatedNumberDataMapper(IDataReader reader)
        {
            RepeatedNumber repeatedNumber = new RepeatedNumber();
            repeatedNumber.SaleZoneId = GetReaderValue<long>(reader, "SaleZoneID");
            repeatedNumber.CustomerId = GetReaderValue<int>(reader, "CustomerID");
            repeatedNumber.SupplierId = GetReaderValue<int>(reader, "SupplierID");
            repeatedNumber.Attempt = GetReaderValue<int>(reader, "Attempt");
            repeatedNumber.DurationInMinutes = GetReaderValue<decimal>(reader, "DurationsInMinutes");
            repeatedNumber.PhoneNumber = reader["PhoneNumber"] as string;
            return repeatedNumber;
        }
        #endregion
    }
}
