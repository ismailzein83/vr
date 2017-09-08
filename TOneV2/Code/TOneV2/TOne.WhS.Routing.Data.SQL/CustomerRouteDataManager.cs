using System;
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
        readonly string[] columns = { "CustomerId", "Code", "SaleZoneId", "Rate", "SaleZoneServiceIds", "IsBlocked", "ExecutedRuleId", "RouteOptions" };

        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        public Vanrise.BusinessProcess.IBPContext BPContext { set; get; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            var streamInfo = preparedCustomerRoute as StreamBulkInsertInfo;
            DateTime start = DateTime.Now;
            InsertBulkToTable(streamInfo);
            if (this.BPContext != null)
            {
                this.BPContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Routes saved to database in {1}", streamInfo.Stream.RecordCount, (DateTime.Now - start));
            }
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
            string saleZoneServiceIds = (record.SaleZoneServiceIds != null && record.SaleZoneServiceIds.Count > 0) ? string.Join(",", record.SaleZoneServiceIds) : null;

            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", record.CustomerId, record.Code, record.SaleZoneId,
                record.Rate, saleZoneServiceIds, record.IsBlocked ? 1 : 0, record.ExecutedRuleId, serializedOptions);//Vanrise.Common.Serializer.Serialize(record.Options, true));
        }

        public IEnumerable<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            query_GetFilteredCustomerRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

            string customerIdsFilter = string.Empty;
            if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                customerIdsFilter = string.Format("AND CustomerId In({0})", string.Join(",", input.Query.CustomerIds));
            query_GetFilteredCustomerRoutes.Replace("#CUSTOMERIDS#", customerIdsFilter);

            string saleZoneIdsFilter = string.Empty;
            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format("AND SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));
            query_GetFilteredCustomerRoutes.Replace("#SALEZONEIDS#", saleZoneIdsFilter);

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            return GetItemsText(query_GetFilteredCustomerRoutes.ToString(), CustomerRouteMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Code", !string.IsNullOrEmpty(input.Query.Code) ? string.Format("{0}%", input.Query.Code) : (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value));
            });
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

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            string maxDOPSyntax = maxDOP.HasValue ? string.Format(",MAXDOP={0}", maxDOP.Value) : "";
            string query;

            trackStep("Starting create Index on CustomerRoute table (CustomerId).");
            query = string.Format(query_CreateIX_CustomerRoute_CustomerId, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (CustomerId).");

            trackStep("Starting create Index on CustomerRoute table (Code).");
            query = string.Format(query_CreateIX_CustomerRoute_Code, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (Code).");

            trackStep("Starting create Index on CustomerRoute table (SaleZoneId).");
            query = string.Format(query_CreateIX_CustomerRoute_SaleZone, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (SaleZoneId).");
        }

        private string SerializeOptions(List<RouteOption> options)
        {
            StringBuilder str = new StringBuilder();
            foreach (var op in options)
            {
                if (str.Length > 0)
                    str.Append("|");

                string supplierServiceIds = op.ExactSupplierServiceIds != null ? string.Join(",", op.ExactSupplierServiceIds) : null;
                str.AppendFormat("{0}~{1}~{2}~{3}~{4}~{5}~{6}~{7}~{8}", op.SupplierId, op.SupplierCode, op.ExecutedRuleId, op.Percentage, op.SupplierRate, op.SupplierZoneId, supplierServiceIds, op.IsBlocked, op.NumberOfTries);
            }
            return str.ToString();
        }

        private List<RouteOption> DeserializeOptions(string serializedOptions)
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
                    SupplierZoneId = long.Parse(parts[5]),
                    ExactSupplierServiceIds = new HashSet<int>(parts[6].Split(',').Select(x => int.Parse(x)))
                };
                int ruleId;
                if (int.TryParse(parts[2], out ruleId))
                    option.ExecutedRuleId = ruleId;
                decimal percentage;
                if (decimal.TryParse(parts[3], out percentage))
                    option.Percentage = percentage;
                bool isBlocked;
                if (bool.TryParse(parts[7], out isBlocked))
                    option.IsBlocked = isBlocked;
                int numberOfTries;
                if (int.TryParse(parts[8], out numberOfTries))
                    option.NumberOfTries = numberOfTries;
                options.Add(option);
            }

            return options;
        }

        private CustomerRoute CustomerRouteMapper(IDataReader reader)
        {
            string saleZoneServiceIds = (reader["SaleZoneServiceIds"] as string);

            return new CustomerRoute()
            {
                CustomerId = (int)reader["CustomerID"],
                CustomerName = reader["CustomerName"].ToString(),
                Code = reader["Code"].ToString(),
                SaleZoneId = (long)reader["SaleZoneID"],
                SaleZoneName = reader["SaleZoneName"].ToString(),
                Rate = GetReaderValue<decimal?>(reader, "Rate"),
                SaleZoneServiceIds = !string.IsNullOrEmpty(saleZoneServiceIds) ? new HashSet<int>(saleZoneServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                IsBlocked = (bool)reader["IsBlocked"],
                ExecutedRuleId = GetReaderValue<int?>(reader, "ExecutedRuleId"),
                Options = reader["RouteOptions"] != DBNull.Value ? DeserializeOptions(reader["RouteOptions"] as string) : null
            };
        }

        #region Queries

        private StringBuilder query_GetFilteredCustomerRoutes = new StringBuilder(@"
                                                            SELECT TOP #LimitResult# cr.[CustomerID]
                                                                ,cr.[Code]
                                                                ,sz.Name as SaleZoneName
                                                                ,ca.Name as  CustomerName
                                                                ,cr.[SaleZoneID]
                                                                ,cr.[Rate]
                                                                ,cr.[SaleZoneServiceIds]
                                                                ,cr.[IsBlocked]
                                                                ,cr.[ExecutedRuleId]
                                                                ,cr.[RouteOptions]
                                                            FROM [dbo].[CustomerRoute] cr with(nolock) JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId=sz.ID JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID=ca.ID
                                                            Where (@Code is null or Code like @Code) and (@IsBlocked is null or IsBlocked = @IsBlocked) #CUSTOMERIDS# #SALEZONEIDS#");

        const string query_LoadCustomerRoutes = @"SELECT [CustomerID]
                                                                ,[Code]
                                                                ,sz.[Name] as SaleZoneName
                                                                ,ca.[Name] as  CustomerName
                                                                ,[SaleZoneID]
                                                                ,[Rate]
                                                                ,[SaleZoneServiceIds]
                                                                ,[IsBlocked]
                                                                ,[ExecutedRuleId]
                                                                ,[RouteOptions]
                                                  FROM [dbo].[CustomerRoute] as cr with(nolock) JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId=sz.ID JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID=ca.ID
                                                    #FILTER#";

        const string query_CreateIX_CustomerRoute_CustomerId = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_CustomerId] ON dbo.CustomerRoute
                                                                (
                                                                      CustomerID ASC
                                                                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_Code = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_Code] ON dbo.CustomerRoute
                                                              (
                                                                      Code ASC
                                                              )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        const string query_CreateIX_CustomerRoute_SaleZone = @"CREATE NONCLUSTERED INDEX [IX_CustomerRoute_SaleZone] ON dbo.CustomerRoute
                                                                (
                                                                      SaleZoneId ASC
                                                                )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [PRIMARY]";

        #endregion
    }
}
