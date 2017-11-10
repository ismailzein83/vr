using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        readonly string[] columns = { "CustomerId", "Code", "SaleZoneId", "IsBlocked", "ExecutedRuleId", "RouteOptions", "VersionNumber" };

        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        public Vanrise.BusinessProcess.IBPContext BPContext { set; get; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CustomerRoute record, object dbApplyStream)
        {
            string serializedOptions = record.Options != null ? SerializeOptions(record.Options) : null;

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            string saleZoneServiceIds = (record.SaleZoneServiceIds != null && record.SaleZoneServiceIds.Count > 0) ? string.Join(",", record.SaleZoneServiceIds) : null;

            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.CustomerId, record.Code, record.SaleZoneId,
               record.IsBlocked ? 1 : 0, record.ExecutedRuleId, serializedOptions, record.VersionNumber);
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

        public IEnumerable<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input)
        {
            query_GetFilteredCustomerRoutes.Replace("#LimitResult#", input.Query.LimitResult.ToString());

            string customerIdsFilter = string.Empty;
            if (input.Query.CustomerIds != null && input.Query.CustomerIds.Count > 0)
                customerIdsFilter = string.Format("AND cr.CustomerId In({0})", string.Join(",", input.Query.CustomerIds));
            query_GetFilteredCustomerRoutes.Replace("#CUSTOMERIDS#", customerIdsFilter);

            string saleZoneIdsFilter = string.Empty;
            if (input.Query.SaleZoneIds != null && input.Query.SaleZoneIds.Count > 0)
                saleZoneIdsFilter = string.Format("AND cr.SaleZoneId In({0})", string.Join(",", input.Query.SaleZoneIds));
            query_GetFilteredCustomerRoutes.Replace("#SALEZONEIDS#", saleZoneIdsFilter);

            bool? isBlocked = null;
            if (input.Query.RouteStatus.HasValue)
                isBlocked = input.Query.RouteStatus.Value == RouteStatus.Blocked ? true : false;

            IEnumerable<Entities.CustomerRoute> customerRoutes = GetItemsText(query_GetFilteredCustomerRoutes.ToString(), CustomerRouteMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Code", !string.IsNullOrEmpty(input.Query.Code) ? string.Format("{0}%", input.Query.Code) : (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@IsBlocked", isBlocked.HasValue ? isBlocked.Value : (object)DBNull.Value));
            });


            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        private void CompleteSupplierData(IEnumerable<CustomerRoute> customerRoutes)
        {
            if (customerRoutes == null || customerRoutes.Count() == 0)
                return;

            HashSet<long> supplierZoneIds = new HashSet<long>();
            foreach (CustomerRoute customerRoute in customerRoutes)
            {
                if (customerRoute.Options == null || customerRoute.Options.Count == 0)
                    continue;

                foreach (RouteOption routeOption in customerRoute.Options)
                {
                    supplierZoneIds.Add(routeOption.SupplierZoneId);
                }
            }
            if (supplierZoneIds.Count > 0)
            {
                SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
                supplierZoneDetailsDataManager.RoutingDatabase = this.RoutingDatabase;

                Dictionary<long, SupplierZoneDetail> supplierZoneDetails = supplierZoneDetailsDataManager.GetFilteredSupplierZoneDetailsBySupplierZone(supplierZoneIds).ToDictionary(itm => itm.SupplierZoneId, itm => itm);

                foreach (CustomerRoute customerRoute in customerRoutes)
                {
                    if (customerRoute.Options == null || customerRoute.Options.Count == 0)
                        continue;

                    foreach (RouteOption routeOption in customerRoute.Options)
                    {
                        SupplierZoneDetail supplierZoneDetail = supplierZoneDetails.GetRecord(routeOption.SupplierZoneId);
                        routeOption.SupplierId = supplierZoneDetail.SupplierId;
                        routeOption.SupplierRate = supplierZoneDetail.EffectiveRateValue;
                        routeOption.ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds;
                    }
                }
            }
        }

        public void LoadRoutes(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded)
        {
            StringBuilder queryBuilder = new StringBuilder(query_LoadCustomerRoutes);
            List<string> filters = new List<string>();
            if (customerId.HasValue)
                filters.Add("cr.[CustomerID] = @CustomerID");
            if (!String.IsNullOrEmpty(codePrefix))
                filters.Add(String.Format("cr.[Code] like '{0}%'", codePrefix));
            if (filters.Count > 0)
            {
                queryBuilder.Replace("#FILTER#", String.Format("WHERE {0}", String.Join(" AND ", filters)));
            }
            else
            {
                queryBuilder.Replace("#FILTER#", "");
            }
            SupplierZoneDetailsDataManager supplierZoneDetailsDataManager = new SupplierZoneDetailsDataManager();
            supplierZoneDetailsDataManager.RoutingDatabase = RoutingDatabase;

            ExecuteReaderText(queryBuilder.ToString(),
                (reader) =>
                {
                    while (reader.Read())
                    {
                        CustomerRoute customerRoute = CustomerRouteMapper(reader);

                        if (customerRoute.Options != null && customerRoute.Options.Count > 0)
                        {
                            var cachedSupplierZoneDetails = supplierZoneDetailsDataManager.GetCachedSupplierZoneDetails();
                            foreach (RouteOption routeOption in customerRoute.Options)
                            {
                                SupplierZoneDetail supplierZoneDetail = cachedSupplierZoneDetails.GetRecord(routeOption.SupplierZoneId);
                                routeOption.SupplierId = supplierZoneDetail.SupplierId;
                                routeOption.SupplierRate = supplierZoneDetail.EffectiveRateValue;
                                routeOption.ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds;
                            }
                        }
                        onRouteLoaded(customerRoute);
                    }
                },
                (cmd) =>
                {
                    if (customerId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@CustomerID", customerId.Value));
                });
        }


        public List<CustomerRoute> GetAffectedCustomerRoutes(List<AffectedRoutes> affectedRoutesList, int partialRoutesNumberLimit, out bool maximumExceeded)
        {
            maximumExceeded = false;

            if (affectedRoutesList == null || affectedRoutesList.Count == 0)
                return null;

            List<string> conditions = new List<string>();
            foreach (AffectedRoutes affectedRoutes in affectedRoutesList)
            {
                List<string> subConditions = new List<string>();

                if (affectedRoutes.CustomerIds != null && affectedRoutes.CustomerIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.CustomerID in ({0})", string.Join<int>(",", affectedRoutes.CustomerIds)));
                }

                if (affectedRoutes.ZoneIds != null && affectedRoutes.ZoneIds.Count() > 0)
                {
                    subConditions.Add(string.Format("cr.SaleZoneID in ({0})", string.Join<long>(",", affectedRoutes.ZoneIds)));
                }

                if (affectedRoutes.Codes != null && affectedRoutes.Codes.Count() > 0)
                {
                    List<string> codesConditions = new List<string>();

                    List<string> codesWithoutSubCodes = new List<string>();
                    List<string> codesWithSubCodes = new List<string>();
                    foreach (CodeCriteria codeCriteria in affectedRoutes.Codes)
                    {
                        if (codeCriteria.WithSubCodes)
                            codesWithSubCodes.Add(codeCriteria.Code);
                        else
                            codesWithoutSubCodes.Add(codeCriteria.Code);
                    }

                    if (codesWithoutSubCodes.Count > 0)
                        codesConditions.Add(string.Format("cr.Code in ('{0}')", string.Join<string>("','", codesWithoutSubCodes)));

                    if (codesWithSubCodes.Count > 0)
                    {
                        foreach (string code in codesWithSubCodes)
                        {
                            codesConditions.Add(string.Format("cr.Code like ('{0}%')", code));
                        }
                    }

                    subConditions.Add(string.Format("({0})", string.Join<string>(" or ", codesConditions)));
                }

                if (affectedRoutes.ExcludedCodes != null && affectedRoutes.ExcludedCodes.Count > 0)
                {
                    subConditions.Add(string.Format("cr.Code not in ('{0}')", string.Join<string>("','", affectedRoutes.ExcludedCodes)));
                }
                conditions.Add(string.Format("({0})", string.Join<string>(" and ", subConditions)));
            }
            query_GetAffectedCustomerRoutes.Replace("#AFFECTEDROUTES#", string.Join(" or ", conditions));

            int totalCount = 0;
            List<CustomerRoute> customerRoutes = new List<CustomerRoute>();
            ExecuteReaderText(query_GetAffectedCustomerRoutes.ToString(), (reader) =>
               {
                   while (reader.Read())
                   {
                       totalCount++;
                       customerRoutes.Add(CustomerRouteMapper(reader));

                       if (totalCount > partialRoutesNumberLimit)
                           break;
                   }
               }, (cmd) => { });

            if (totalCount > partialRoutesNumberLimit)
            {
                maximumExceeded = true;
                return null;
            }

            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public int GetTotalCount()
        {
            return (int)ExecuteScalarText(@"IF OBJECT_ID('[dbo].[CustomerRoute]', N'U') IS NOT NULL 
                                Begin
                                    SELECT CAST(p.rows AS int)
                                    FROM sys.tables AS tbl
                                    INNER JOIN sys.indexes AS idx ON idx.object_id = tbl.object_id and idx.index_id < 2
                                    INNER JOIN sys.partitions AS p ON p.object_id = CAST(tbl.object_id AS int) and p.index_id = idx.index_id
                                    WHERE ((tbl.name=N'CustomerRoute' AND SCHEMA_NAME(tbl.schema_id)=N'dbo'))
                                End", (cmd) => { });
        }

        public void UpdateCustomerRoutes(List<CustomerRoute> customerRoutes)
        {
            DataTable dtCustomerRoutes = BuildCustomerRouteTable(customerRoutes);
            ExecuteNonQueryText(query_UpdateCustomerRoutes.ToString(), (cmd) =>
            {
                var dtPrm = new SqlParameter("@Routes", SqlDbType.Structured);
                dtPrm.TypeName = "CustomerRouteType";
                dtPrm.Value = dtCustomerRoutes;
                cmd.Parameters.Add(dtPrm);
            });
        }

        DataTable BuildCustomerRouteTable(List<CustomerRoute> customerRoutes)
        {
            DataTable dtCustomerRoutes = new DataTable();
            dtCustomerRoutes.Columns.Add("CustomerId", typeof(int));
            dtCustomerRoutes.Columns.Add("Code", typeof(string));
            dtCustomerRoutes.Columns.Add("SaleZoneId", typeof(long));
            dtCustomerRoutes.Columns.Add("IsBlocked", typeof(int));
            dtCustomerRoutes.Columns.Add("ExecutedRuleId", typeof(int));
            dtCustomerRoutes.Columns.Add("RouteOptions", typeof(string));
            dtCustomerRoutes.Columns.Add("VersionNumber", typeof(int));
            dtCustomerRoutes.BeginLoadData();
            foreach (var customerRoute in customerRoutes)
            {
                string serializedOptions = customerRoute.Options != null ? SerializeOptions(customerRoute.Options) : null;
                DataRow dr = dtCustomerRoutes.NewRow();
                dr["CustomerId"] = customerRoute.CustomerId;
                dr["Code"] = customerRoute.Code;
                dr["SaleZoneId"] = customerRoute.SaleZoneId;
                dr["IsBlocked"] = customerRoute.IsBlocked;

                if (customerRoute.ExecutedRuleId.HasValue)
                    dr["ExecutedRuleId"] = customerRoute.ExecutedRuleId;
                else
                    dr["ExecutedRuleId"] = DBNull.Value;

                if (!string.IsNullOrEmpty(serializedOptions))
                    dr["RouteOptions"] = serializedOptions;
                else
                    dr["RouteOptions"] = DBNull.Value;

                dr["VersionNumber"] = customerRoute.VersionNumber;
                dtCustomerRoutes.Rows.Add(dr);
            }
            dtCustomerRoutes.EndLoadData();
            return dtCustomerRoutes;
        }

        public List<CustomerRoute> GetUpdatedCustomerRoutes(List<CustomerRouteDefinition> customerRouteDefinitions, int versionNumber)
        {
            if (customerRouteDefinitions == null || customerRouteDefinitions.Count == 0)
                return null;

            List<string> conditions = new List<string>();

            foreach (CustomerRouteDefinition customerRouteDefinition in customerRouteDefinitions)
            {
                string query = string.Format("(cr.CustomerId = {0} and cr.Code = '{1}')", customerRouteDefinition.CustomerId, customerRouteDefinition.Code);
                conditions.Add(query);
            }

            string concatenedQuery = string.Format("({0}) and cr.VersionNumber = {1}", string.Join<string>(" or ", conditions), versionNumber);

            query_GetAffectedCustomerRoutes.Replace("#AFFECTEDROUTES#", concatenedQuery);
            List<CustomerRoute> customerRoutes = GetItemsText(query_GetAffectedCustomerRoutes.ToString(), CustomerRouteMapper, (cmd) => { });
            CompleteSupplierData(customerRoutes);
            return customerRoutes;
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commandTimeoutInSeconds, int? maxDOP)
        {
            string maxDOPSyntax = maxDOP.HasValue ? string.Format(",MAXDOP={0}", maxDOP.Value) : "";
            string query;

            //trackStep("Starting create Index on CustomerRoute table (CustomerId).");
            //query = string.Format(query_CreateIX_CustomerRoute_CustomerId, maxDOPSyntax);
            //ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            //trackStep("Finished create Index on CustomerRoute table (CustomerId).");

            trackStep("Starting create Index on CustomerRoute table (Code).");
            query = string.Format(query_CreateIX_CustomerRoute_Code, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (Code).");

            trackStep("Starting create Index on CustomerRoute table (SaleZoneId).");
            query = string.Format(query_CreateIX_CustomerRoute_SaleZone, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create Index on CustomerRoute table (SaleZoneId).");

            trackStep("Starting create CLUSTERED Index on CustomerRoute table (CustomerId and Code).");
            query = string.Format(query_CreateIX_CustomerRoute_CustomerId_Code, maxDOPSyntax);
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            trackStep("Finished create CLUSTERED on CustomerRoute table (CustomerId and Code).");
        }

        #endregion

        #region Private Methods

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
                Options = reader["RouteOptions"] != DBNull.Value ? DeserializeOptions(reader["RouteOptions"] as string) : null,
                VersionNumber = GetReaderValue<int>(reader, "VersionNumber")
            };
        }

        private string SerializeOptions(List<RouteOption> options)
        {
            StringBuilder str = new StringBuilder();
            foreach (var op in options)
            {
                if (str.Length > 0)
                    str.Append("|");

                string supplierServiceIds = op.ExactSupplierServiceIds != null ? string.Join(",", op.ExactSupplierServiceIds) : null;
                str.AppendFormat("{0}~{1}~{2}~{3}~{4}~{5}", op.SupplierCode, op.ExecutedRuleId, op.Percentage, op.SupplierZoneId, !op.IsBlocked ? string.Empty : "1", op.NumberOfTries == 1 ? string.Empty : op.NumberOfTries.ToString());
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
                    SupplierCode = parts[0],
                    SupplierZoneId = long.Parse(parts[3]),
                };
                int ruleId;
                if (int.TryParse(parts[1], out ruleId))
                    option.ExecutedRuleId = ruleId;
                int percentage;
                if (int.TryParse(parts[2], out percentage))
                    option.Percentage = percentage;

                string isBlockedAsString = parts[4];
                if (!string.IsNullOrEmpty(isBlockedAsString))
                {
                    int isBlocked;
                    if (int.TryParse(isBlockedAsString, out isBlocked))
                        option.IsBlocked = isBlocked > 0;
                }

                string numberOfTriesAsString = parts[5];
                if (!string.IsNullOrEmpty(isBlockedAsString))
                {
                    int numberOfTries;
                    if (int.TryParse(parts[5], out numberOfTries))
                        option.NumberOfTries = numberOfTries;
                }
                else
                {
                    option.NumberOfTries = 1;
                }

                options.Add(option);

            }

            return options;
        }

        #endregion

        #region Queries

        private StringBuilder query_GetFilteredCustomerRoutes = new StringBuilder(@"
                                                            SELECT TOP #LimitResult# cr.CustomerID
                                                                                    ,cr.Code
                                                                                    ,sz.Name as SaleZoneName
                                                                                    ,ca.Name as  CustomerName
                                                                                    ,cr.SaleZoneID
                                                                                    ,czd.EffectiveRateValue as Rate
                                                                                    ,czd.SaleZoneServiceIds
                                                                                    ,cr.IsBlocked
                                                                                    ,cr.ExecutedRuleId
                                                                                    ,cr.RouteOptions
                                                                                    ,cr.VersionNumber
                                                            FROM [dbo].[CustomerRoute] cr with(nolock) 
                                                            JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId = sz.ID 
                                                            JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID = ca.ID
                                                            JOIN [dbo].[CustomerZoneDetail] as czd ON czd.SaleZoneId = cr.SaleZoneID and czd.CustomerId = cr.CustomerID
                                                            Where (@Code is null or Code like @Code) and (@IsBlocked is null or IsBlocked = @IsBlocked) #CUSTOMERIDS# #SALEZONEIDS#");

        private StringBuilder query_GetAffectedCustomerRoutes = new StringBuilder(@"
                                                            SELECT  cr.CustomerID
                                                                   ,cr.Code
                                                                   ,sz.Name as SaleZoneName
                                                                   ,ca.Name as  CustomerName
                                                                   ,cr.SaleZoneID
                                                                   ,czd.EffectiveRateValue as Rate
                                                                   ,czd.SaleZoneServiceIds
                                                                   ,cr.IsBlocked
                                                                   ,cr.ExecutedRuleId
                                                                   ,cr.RouteOptions
                                                                   ,cr.VersionNumber
                                                            FROM [dbo].[CustomerRoute] cr with(nolock) 
                                                            JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId = sz.ID 
                                                            JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID = ca.ID
                                                            JOIN [dbo].[CustomerZoneDetail] as czd ON czd.SaleZoneId = cr.SaleZoneID and czd.CustomerId = cr.CustomerID
                                                            Where #AFFECTEDROUTES#");

        const string query_LoadCustomerRoutes = @"SELECT cr.CustomerID
                                                        ,cr.Code
                                                        ,sz.Name as SaleZoneName
                                                        ,ca.Name as  CustomerName
                                                        ,cr.SaleZoneID
                                                        ,czd.EffectiveRateValue as Rate
                                                        ,czd.SaleZoneServiceIds
                                                        ,cr.IsBlocked
                                                        ,cr.ExecutedRuleId
                                                        ,cr.RouteOptions
                                                        ,cr.VersionNumber
                                                  FROM [dbo].[CustomerRoute] cr with(nolock) 
                                                  JOIN [dbo].[SaleZone] as sz ON cr.SaleZoneId = sz.ID 
                                                  JOIN [dbo].[CarrierAccount] as ca ON cr.CustomerID = ca.ID
                                                  JOIN [dbo].[CustomerZoneDetail] as czd ON czd.SaleZoneId = cr.SaleZoneID and czd.CustomerId = cr.CustomerID
                                                  #FILTER#";

        private StringBuilder query_UpdateCustomerRoutes = new StringBuilder(@"  UPDATE customerRoute set customerRoute.IsBlocked = routes.IsBlocked, customerRoute.ExecutedRuleId = routes.ExecutedRuleId, 
                                                            customerRoute.RouteOptions = routes.RouteOptions, customerRoute.VersionNumber = routes.VersionNumber
                                                            FROM [dbo].[CustomerRoute] customerRoute WITH(INDEX(IX_CustomerRoute_CustomerId_Code))
                                                            JOIN @Routes routes on routes.CustomerId = customerRoute.CustomerId and routes.Code = customerRoute.Code");


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

        const string query_CreateIX_CustomerRoute_CustomerId_Code = @"CREATE UNIQUE CLUSTERED INDEX [IX_CustomerRoute_CustomerId_Code] ON [dbo].[CustomerRoute]
                                                                    (
	                                                                    [CustomerId] ASC,
	                                                                    [Code] ASC
                                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        #endregion
    }
}
