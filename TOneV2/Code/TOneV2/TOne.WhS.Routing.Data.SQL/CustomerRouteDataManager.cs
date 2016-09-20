﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        readonly string[] columns = { "CustomerId", "Code", "SaleZoneId", "Rate", "IsBlocked", "ExecutedRuleId", "RouteOptions" };
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
                ColumnNames = columns,
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CustomerRoute record, object dbApplyStream)
        {
            string serializedOptions = record.Options != null ? SerializeOptions(record.Options) : null;// Convert.ToBase64String(Vanrise.Common.ProtoBufSerializer.Serialize<List<RouteOption>>(record.Options));
            //string serializedOptions = Convert.ToBase64String(Vanrise.Common.ProtoBufSerializer.Serialize<List<RouteOption>>(record.Options));
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.CustomerId, record.Code, record.SaleZoneId,
                record.Rate, record.IsBlocked ? 1 : 0, record.ExecutedRuleId, serializedOptions);//Vanrise.Common.Serializer.Serialize(record.Options, true));
        }

        public Vanrise.Entities.BigResult<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                query_GetFilteredCustomerRoutes.Replace("#TEMPTABLE#", tempTableName);
                query_GetFilteredCustomerRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

                string customerIdsFilter = string.Empty;

                if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                    customerIdsFilter = string.Format("AND CustomerId In({0})", string.Join(",", input.Query.CustomerIds));

                query_GetFilteredCustomerRoutes.Replace("#CUSTOMERIDS#", customerIdsFilter);

                ExecuteNonQueryText(query_GetFilteredCustomerRoutes.ToString(), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@Code", !string.IsNullOrEmpty(input.Query.Code) ? string.Format("{0}%", input.Query.Code) : (object)DBNull.Value));
                });
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");
            return RetrieveData(input, createTempTableAction, CustomerRouteMapper);
        }


        public void LoadRoutes(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded)
        {
            StringBuilder queryBuilder = new StringBuilder(query_LoadCustomerRoutes);
            List<string> filters = new List<string>();
            if (customerId.HasValue)
                filters.Add("[CustomerID] = @CustomerID");
            if (!String.IsNullOrEmpty(codePrefix))
                filters.Add(String.Format("[Code] like '{0}%'", codePrefix));
            if (filters.Count > 0)
            {
                queryBuilder.Replace("#FILTER#", String.Format("WHERE {0}", String.Join(" AND ", filters)));
            }
            else
            {
                queryBuilder.Replace("#FILTER#", "");
            }
            ExecuteReaderText(queryBuilder.ToString(),
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onRouteLoaded(CustomerRouteMapper(reader));
                    }
                },
                (cmd) =>
                {
                    if (customerId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@CustomerID", customerId.Value));
                });
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep)
        {
            string query;

            trackStep("Starting create Index on CustomerRoute table (CustomerId).");
            query = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_CustomerId] ON dbo.CustomerRoute
                    (
                          CustomerID ASC
                    )";
            ExecuteNonQueryText(query, null);
            trackStep("Finished create Index on CustomerRoute table (CustomerId).");

            trackStep("Starting create Index on CustomerRoute table (Code).");
            query = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_Code] ON dbo.CustomerRoute
                    (
                          Code ASC
                    )";
            ExecuteNonQueryText(query, null);
            trackStep("Finished create Index on CustomerRoute table (Code).");

            trackStep("Starting create Index on CustomerRoute table (SaleZoneId).");
            query = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_SaleZone] ON dbo.CustomerRoute
                    (
                          SaleZoneId ASC
                    )";
            ExecuteNonQueryText(query, null);
            trackStep("Finished create Index on CustomerRoute table (SaleZoneId).");
        }


        private string SerializeOptions(List<RouteOption> options)
        {
            StringBuilder str = new StringBuilder();
            foreach (var op in options)
            {
                if (str.Length > 0)
                    str.Append("|");
                str.AppendFormat("{0}~{1}~{2}~{3}~{4}~{5}~{6}", op.SupplierId, op.SupplierCode, op.ExecutedRuleId, op.Percentage, op.SupplierRate, op.SupplierZoneId, op.IsBlocked);
            }
            return str.ToString();
        }

        List<RouteOption> DeserializeOptions(string serializedOptions)
        {
            List<RouteOption> options = new List<RouteOption>();

            string[] lines = serializedOptions.Split('|');
            foreach (var line in lines)
            {
                string[] parts = line.Split('~');
                var option = new RouteOption
                {
                    SupplierId = int.Parse(parts[0]),
                    SupplierCode = parts[1],
                    SupplierRate = Decimal.Parse(parts[4]),
                    SupplierZoneId = long.Parse(parts[5])
                };
                int ruleId;
                if (int.TryParse(parts[2], out ruleId))
                    option.ExecutedRuleId = ruleId;
                decimal percentage;
                if (decimal.TryParse(parts[3], out percentage))
                    option.Percentage = percentage;
                bool isBlocked;
                if (bool.TryParse(parts[6], out isBlocked))
                    option.IsBlocked = isBlocked;
                options.Add(option);
            }

            return options;
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
                Options = reader["RouteOptions"] != DBNull.Value ? DeserializeOptions(reader["RouteOptions"] as string) : null
            };
        }

        #region Queries

        private StringBuilder query_GetFilteredCustomerRoutes = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                            SELECT TOP #LimitResult# [CustomerID]
                                                                ,[Code]
                                                                ,[SaleZoneID]
                                                                ,[Rate]
                                                                ,[IsBlocked]
                                                                ,[ExecutedRuleId]
                                                                ,[RouteOptions]
                                                            INTO #TEMPTABLE# FROM [dbo].[CustomerRoute] with(nolock)
                                                            Where (@Code is Null or Code like @Code)
                                                                #CUSTOMERIDS#
                                                            END");

        const string query_LoadCustomerRoutes = @"SELECT [CustomerID]
                                                                ,[Code]
                                                                ,[SaleZoneID]
                                                                ,[Rate]
                                                                ,[IsBlocked]
                                                                ,[ExecutedRuleId]
                                                                ,[RouteOptions]
                                                  FROM [dbo].[CustomerRoute] with(nolock)
                                                    #FILTER#";

        #endregion
    }
}
