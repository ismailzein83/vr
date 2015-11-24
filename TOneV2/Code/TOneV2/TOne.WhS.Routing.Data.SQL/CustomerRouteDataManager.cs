using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            InsertBulkToTable(preparedCustomerRoute as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerRoute]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CustomerRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.CustomerId, record.Code, record.SaleZoneId, 
                record.Rate, record.IsBlocked ? 1:0, record.ExecutedRuleId, Vanrise.Common.Serializer.Serialize(record.Options, true));
        }

        public Vanrise.Entities.BigResult<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                query_GetFilteredCustomerRoutes.Replace("#TEMPTABLE#", tempTableName);

                string customerIdsFilter = string.Empty;

                if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                    customerIdsFilter = string.Format("AND CustomerId In({0})", string.Join(",", input.Query.CustomerIds));

                query_GetFilteredCustomerRoutes.Replace("#CUSTOMERIDS#", customerIdsFilter);

                ExecuteNonQueryText(query_GetFilteredCustomerRoutes.ToString(), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@Code", input.Query.Code != null ? input.Query.Code : (object)DBNull.Value));
                });
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");
            return RetrieveData(input, createTempTableAction, CustomerRouteMapper);
        }

        private CustomerRoute CustomerRouteMapper(IDataReader reader)
        {
            return new CustomerRoute()
            {
                CustomerId = (int)reader["CustomerID"],
                Code = reader["Code"].ToString(),
                SaleZoneId = (long)reader["SaleZoneID"],
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = (int)reader["ExecutedRuleId"],
                Options = reader["RouteOptions"] != null? Vanrise.Common.Serializer.Deserialize<List<RouteOption>>(reader["RouteOptions"].ToString()) : null
            };
        }

        #region Queries

        private StringBuilder query_GetFilteredCustomerRoutes = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                            SELECT [CustomerID]
                                                                ,[Code]
                                                                ,[SaleZoneID]
                                                                ,[Rate]
                                                                ,[IsBlocked]
                                                                ,[ExecutedRuleId]
                                                                ,[RouteOptions]
                                                            INTO #TEMPTABLE# FROM [dbo].[CustomerRoute] with(nolock)
                                                            Where (@Code is Null or Code = @Code)
                                                                #CUSTOMERIDS#
                                                            END");

        #endregion
    }
}
