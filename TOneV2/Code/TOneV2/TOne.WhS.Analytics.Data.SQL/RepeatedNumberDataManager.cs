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
        private string mainCDRTableName = "[TOneWhS_CDR].[BillingCDR_Main]";
        private string mainCDRTableAlias = "MainCDR";
        private string mainCDRTableIndex = "IX_BillingCDR_Main_AttemptDateTime";

        private string invalidCDRTableName = "[TOneWhS_CDR].[BillingCDR_Invalid]";
        private string invalidCDRTableAlias = "InValidCDR";
        private string invalidCDRTableIndex = "IX_BillingCDR_Invalid_AttemptDateTime";

        private string failedCDRTableName = "[TOneWhS_CDR].[BillingCDR_Failed]";
        private string failedCDRTableAlias = "FailedCDR";
        private string failedCDRTableIndex = "IX_BillingCDR_Failed_AttemptDateTime";


        private string partialPricedCDRTableName = "[TOneWhS_CDR].[BillingCDR_PartialPriced]";
        private string partialPricedCDRTableAlias = "PartialPriced";
        private string partialPricedCDRTableIndex = "IX_BillingCDR_PartialPriced_AttemptDateTime";

        public RepeatedNumberDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {
        }
        #endregion

        #region Public Methods

        public IEnumerable<RepeatedNumber> GetAllFilteredRepeatedNumbers(Vanrise.Entities.DataRetrievalInput<Entities.RepeatedNumberQuery> input)
        {
            DateTime toDate = DateTime.MinValue;
            if (input.Query.To == DateTime.MinValue)
                toDate = DateTime.Now;
            else
                toDate = input.Query.To;
            return GetItemsText(GetQuery(input.Query.Filter, input.Query.CDRType, input.Query.RepeatedMorethan, input.Query.PhoneNumberType, input.Query.PhoneNumber), RepeatedNumberDataMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
                cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(toDate)));
            });
        }

        #endregion

        #region Private Methods
        private string GetQuery(RepeatedNumberFilter filter, CDRType cdrType, int repeatedMorethan, string phoneNumberType, string phoneNumber)
        {
            string aliasTableName = "newtable";
            StringBuilder selectColumnBuilder = new StringBuilder();
            StringBuilder groupByBuilder = new StringBuilder();
            StringBuilder havingBuilder = new StringBuilder();

            AddSelectColumnToQuery(selectColumnBuilder, aliasTableName);
            AddGroupingToQuery(groupByBuilder, aliasTableName);
            AddHavingToQuery(havingBuilder, aliasTableName, repeatedMorethan);

            string invalidCDRTableAppendedFilter = string.Format("{0}.[Type] != 6", invalidCDRTableAlias);

            string queryData = "";

            switch (cdrType)
            {
                case CDRType.All:
                    queryData = String.Format(@"{0} UNION ALL {1}  UNION ALL {2} UNION ALL {3}",
                        GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber),
                        GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber, invalidCDRTableAppendedFilter),
                        GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber),
                        GetSingleQuery(partialPricedCDRTableName, partialPricedCDRTableAlias, filter, partialPricedCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber));
                    break;
                case CDRType.Invalid:
                    queryData = GetSingleQuery(invalidCDRTableName, invalidCDRTableAlias, filter, invalidCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber, invalidCDRTableAppendedFilter);
                    break;
                case CDRType.Failed:
                    queryData = GetSingleQuery(failedCDRTableName, failedCDRTableAlias, filter, failedCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber);
                    break;
                case CDRType.Successful:
                    queryData = GetSingleQuery(mainCDRTableName, mainCDRTableAlias, filter, mainCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber);
                    break;
                case CDRType.PartialPriced:
                    queryData = GetSingleQuery(partialPricedCDRTableName, partialPricedCDRTableAlias, filter, partialPricedCDRTableIndex, repeatedMorethan, phoneNumberType, phoneNumber);
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
        private string GetSingleQuery(string tableName, string alias, RepeatedNumberFilter filter, string tableIndex, int repeatedMorethan, string phoneNumberType, string phoneNumber, string appendedFilter = null)
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

            whereBuilder.Append(String.Format(@"{0}.AttemptDateTime>= @FromDate AND ({0}.AttemptDateTime<= @ToDate)", alias));
            if (!string.IsNullOrEmpty(phoneNumber))
                whereBuilder.Append(String.Format(@"And {0}.{1} like '%{2}%' ", alias, phoneNumberType, phoneNumber));
            groupByBuilder.Append(String.Format(@"SaleZoneID, CustomerID, SupplierID, {0}", phoneNumberType));
            havingBuilder.Append(String.Format(@"HAVING SUM(AttemptDateTime) >= {0} ", repeatedMorethan));
            selectQueryPart.Append(String.Format(@" {0}.SaleZoneID, {0}.CustomerID, {0}.SupplierID, Count({0}.AttemptDateTime) AS Attempt,
                                                    CONVERT(DECIMAL(10,2),SUM({0}.DurationInSeconds)/60.) AS DurationsInMinutes,
		                                            {0}.{1}  AS PhoneNumber ", alias, phoneNumberType));

            AddAppendedFilter(whereBuilder, appendedFilter);

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
        private void AddAppendedFilter(StringBuilder whereBuilder, string appendedFilter)
        {
            if (string.IsNullOrEmpty(appendedFilter))
                return;

            if (whereBuilder.Length > 0)
                whereBuilder.Append(string.Format(" AND {0} ", appendedFilter));
            else
                whereBuilder.Append(appendedFilter);
        }
        private RepeatedNumber RepeatedNumberDataMapper(IDataReader reader)
        {
            RepeatedNumber repeatedNumber = new RepeatedNumber();
            repeatedNumber.SaleZoneId = GetReaderValue<long?>(reader, "SaleZoneID");
            repeatedNumber.CustomerId = GetReaderValue<int?>(reader, "CustomerID");
            repeatedNumber.SupplierId = GetReaderValue<int?>(reader, "SupplierID");
            repeatedNumber.Attempt = GetReaderValue<int>(reader, "Attempt");
            repeatedNumber.DurationInMinutes = GetReaderValue<decimal>(reader, "DurationsInMinutes");
            repeatedNumber.PhoneNumber = reader["PhoneNumber"] as string;
            return repeatedNumber;
        }
        #endregion
    }
}
