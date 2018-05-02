using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using System.Configuration;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        static bool Routing_TOne_Testing = ConfigurationManager.AppSettings["Routing_TOne_Testing"] == "true";
        static string Routing_TOneV1_FileGroup = ConfigurationManager.AppSettings["Routing_TOneV1_FileGroup"];
        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        readonly string[] routeColumns = { "RouteID", "CustomerID", "ProfileID", "Code", "OurZoneID", "OurActiveRate", "OurServicesFlag", "State", "Updated", "IsToDAffected", "IsSpecialRequestAffected", "IsOverrideAffected", "IsBlockAffected", "IsOptionBlock", "BatchID" };

        readonly string[] routeOptionsColumns = { "RouteID", "SupplierID", "SupplierZoneID", "SupplierActiveRate", "SupplierServicesFlag", "Priority", "NumberOfTries", "State", "Updated", "Percentage" };

        readonly string[] routeColumnsTesting = { "RouteID", "CustomerID", "ProfileID", "Code", "OurZoneID", "OurActiveRate", "OurServicesFlag", "State", "Updated", "IsToDAffected", "IsSpecialRequestAffected", "IsOverrideAffected", "IsBlockAffected", "IsOptionBlock", "BatchID", "ExecutedRuleId" };

        readonly string[] routeOptionsColumnsTesting = { "RouteID", "SupplierID", "SupplierZoneID", "SupplierActiveRate", "SupplierServicesFlag", "Priority", "NumberOfTries", "State", "Updated", "Percentage", "ExecutedRuleId" };

        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            CustomerRouteBulkInsertInfo customerRouteBulkInsertInfo = preparedCustomerRoute as CustomerRouteBulkInsertInfo;
            Parallel.For(0, 2, (i) =>
                {
                    switch (i)
                    {
                        case 0: InsertBulkToTable(customerRouteBulkInsertInfo.RouteStreamForBulkInsertInfo); break;
                        case 1: InsertBulkToTable(customerRouteBulkInsertInfo.RouteOptionStreamForBulkInsertInfo); break;
                    }
                });
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            CustomerRouteBulkInsert customerRouteBulkInsert = dbApplyStream as CustomerRouteBulkInsert;

            customerRouteBulkInsert.RouteStreamForBulkInsert.Close();
            customerRouteBulkInsert.RouteOptionStreamForBulkInsert.Close();

            CustomerRouteBulkInsertInfo customerRouteBulkInsertInfo = new CustomerRouteBulkInsertInfo();

            customerRouteBulkInsertInfo.RouteStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Route_Temp]",
                Stream = customerRouteBulkInsert.RouteStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = Routing_TOne_Testing ? routeColumnsTesting : routeColumns,
            };

            customerRouteBulkInsertInfo.RouteOptionStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[RouteOption_Temp]",
                Stream = customerRouteBulkInsert.RouteOptionStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = Routing_TOne_Testing ? routeOptionsColumnsTesting : routeOptionsColumns,
            };

            return customerRouteBulkInsertInfo;
        }

        public object InitialiazeStreamForDBApply()
        {
            CustomerRouteBulkInsert customerRouteBulkInsert = new CustomerRouteBulkInsert();
            customerRouteBulkInsert.RouteStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            customerRouteBulkInsert.RouteOptionStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            return customerRouteBulkInsert;
        }

        ReserveRouteIdsResponse _lastReservedRouteIds;
        int _lastTakenId;
        Vanrise.Runtime.InterRuntimeServiceManager _interRuntimeServiceManager = new Vanrise.Runtime.InterRuntimeServiceManager();
        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        SaleZoneManager _saleZoneManager = new SaleZoneManager();
        SupplierZoneManager _supplierZoneManager = new SupplierZoneManager();
        ZoneServiceConfigManager _zoneServiceConfigManager = new ZoneServiceConfigManager();
        CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();
        Dictionary<int, CarrierAccount> _allCarriers;
        Dictionary<long, SaleZone> _allSaleZones;
        Dictionary<long, SupplierZone> _allSupplierZones;
        Dictionary<int, ZoneServiceConfig> _allZoneServiceConfigs;
        Dictionary<int, CarrierProfile> _allCarrierProfiles;

        public void WriteRecordToStream(TOne.WhS.Routing.Entities.CustomerRoute record, object dbApplyStream)
        {
            int routeId = SetRouteId();

            InitializeData();

            CarrierAccount customer = _allCarriers.GetRecord(record.CustomerId);
            SaleZone saleZone = _allSaleZones.GetRecord(record.SaleZoneId);
            CarrierProfile profile = _allCarrierProfiles.GetRecord(customer.CarrierProfileId);

            int customerServiceFlag = GetServiceFlag(record.SaleZoneServiceIds, _allZoneServiceConfigs);
            DateTime now = DateTime.Now;
            bool hasOptionBlock = false;

            CustomerRouteBulkInsert customerRouteBulkInsert = dbApplyStream as CustomerRouteBulkInsert;
            int counter = 10;

            int isToDAffected = 0;
            int isSpecialRequestAffected = 0;
            int isOverrideAffected = 0;
            int isBlockAffected = 0;

            switch (record.CorrespondentType)
            {
                case CorrespondentType.Block: isBlockAffected = 1; break;
                case CorrespondentType.Override: isOverrideAffected = 1; break;
                case CorrespondentType.SpecialRequest: isSpecialRequestAffected = 1; break;
                case CorrespondentType.LCR:
                case CorrespondentType.Other:
                default: break;
            }
            if (record.Options != null)
            {
                foreach (RouteOption option in record.Options)
                {
                    int priority;
                    if (option.IsBlocked)
                    {
                        hasOptionBlock = true;
                        priority = 0;
                    }
                    else
                    {
                        priority = Math.Max(0, counter--);
                    }
                    CarrierAccount supplier = _allCarriers.GetRecord(option.SupplierId);
                    SupplierZone supplierZone = _allSupplierZones.GetRecord(option.SupplierZoneId);
                    int supplierServiceFlag = GetServiceFlag(option.ExactSupplierServiceIds, _allZoneServiceConfigs);

                    if (Routing_TOne_Testing)
                    {
                        customerRouteBulkInsert.RouteOptionStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}", routeId, supplier.SourceId, supplierZone.SourceId, option.SupplierRate,
                            supplierServiceFlag, priority, option.NumberOfTries, option.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), option.Percentage.HasValue ? option.Percentage.Value : 0, option.ExecutedRuleId);
                    }
                    else
                    {
                        customerRouteBulkInsert.RouteOptionStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}", routeId, supplier.SourceId, supplierZone.SourceId, option.SupplierRate,
                            supplierServiceFlag, priority, option.NumberOfTries, option.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), option.Percentage.HasValue ? option.Percentage.Value : 0);
                    }
                }
            }
            if (Routing_TOne_Testing)
            {
                customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}", routeId, customer.SourceId, profile.SourceId, record.Code, saleZone.SourceId,
                    record.Rate.HasValue ? record.Rate.Value : -1, customerServiceFlag, record.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), isToDAffected, isSpecialRequestAffected, isOverrideAffected, isBlockAffected, hasOptionBlock ? 1 : 0, 0, record.ExecutedRuleId);
            }
            else
            {
                customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}", routeId, customer.SourceId, profile.SourceId, record.Code, saleZone.SourceId,
                     record.Rate.HasValue ? record.Rate.Value : -1, customerServiceFlag, record.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), isToDAffected, isSpecialRequestAffected, isOverrideAffected, isBlockAffected, hasOptionBlock ? 1 : 0, 0);
            }
        }

        private int SetRouteId()
        {
            int routeId;
            if (_lastReservedRouteIds == null || _lastReservedRouteIds.EndId == _lastTakenId)
            {
                _lastReservedRouteIds = _interRuntimeServiceManager.SendRequest(this.ParentWFRuntimeProcessId, new ReserveRouteIdsRequest { ParentBPInstanceId = this.ParentBPInstanceId });
                _lastTakenId = _lastReservedRouteIds.StartingId;
                routeId = _lastTakenId;
            }
            else
            {
                _lastTakenId++;
                routeId = _lastTakenId;
            }
            return routeId;
        }

        private void InitializeData()
        {
            if (_allCarriers == null)
                _allCarriers = _carrierAccountManager.GetCachedCarrierAccounts();
            if (_allSaleZones == null)
                _allSaleZones = _saleZoneManager.GetCachedSaleZones();
            if (_allSupplierZones == null)
                _allSupplierZones = _supplierZoneManager.GetCachedSupplierZones();
            if (_allZoneServiceConfigs == null)
                _allZoneServiceConfigs = _zoneServiceConfigManager.GetCachedZoneServiceConfigs();
            if (_allCarrierProfiles == null)
                _allCarrierProfiles = _carrierProfileManager.GetCachedCarrierProfiles();
        }

        public IEnumerable<TOne.WhS.Routing.Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<TOne.WhS.Routing.Entities.CustomerRouteQuery> input)
        {
            throw new NotImplementedException();
        }


        public void LoadRoutes(int? customerId, string codePrefix, Func<bool> shouldStop, Action<CustomerRoute> onRouteLoaded)
        {
            throw new NotImplementedException();
        }

        public List<CustomerRouteData> GetAffectedCustomerRoutes(List<AffectedRoutes> affectedRoutesList, List<AffectedRouteOptions> affectedRouteOptionsList, long partialRoutesNumberLimit, out bool maximumExceeded)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomerRoutes(List<CustomerRouteData> customerRouteDataList)
        {
            throw new NotImplementedException();
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commnadTimeoutInSeconds, int? maxDOP)
        {
            StringBuilder query = new StringBuilder();
            string maxDOPSyntax = maxDOP.HasValue ? string.Format(",MAXDOP={0}", maxDOP.Value) : "";

            if (!Routing_TOne_Testing)
            {
                CreateIndex(string.Format(query_CreateZoneRateIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Zone Rate", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateCodeMatchIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Code Match", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Route", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteOptionIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Route Option", commnadTimeoutInSeconds);
            }
            FillData(query_FillZoneMacthTemp, trackStep, "Zone Match", commnadTimeoutInSeconds);
            FillData(query_FillRouteBlockConcatinatedTemp, trackStep, "Route Block Concatinated", commnadTimeoutInSeconds);
            FillData(query_FillPoolTablesTemp, trackStep, "Route Pool and Route Options Pool", commnadTimeoutInSeconds);

            if (!Routing_TOne_Testing)
            {
                CreateIndex(string.Format(query_CreateZoneMatchIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Zone Match", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteBlockConcatinatedIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Route Block Concatinated", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRoutePoolIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Route Pool", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteOptionsPoolIndexes, maxDOPSyntax, Routing_TOneV1_FileGroup), trackStep, "Route Options Pool", commnadTimeoutInSeconds);
            }

            query.AppendLine(query_DropSupplierZoneDetailsTable);
            query.AppendLine(query_DropCodeSaleZoneTable);
            query.AppendLine(query_DropCustomerZoneDetailTable);
            query.AppendLine(query_DropCustomerRouteTable);
            query.AppendLine(query_DropZoneRateTable);
            query.AppendLine(query_DropCodeMatchTable);
            query.AppendLine(query_DropZoneMatchTable);
            query.AppendLine(query_DropRoutePoolTable);
            query.AppendLine(query_DropRouteOptionsPoolTable);
            query.AppendLine(query_DropRouteBlockConcatinatedTable);
            query.AppendLine("EXEC sp_rename 'Route_Temp','Route';");
            query.AppendLine("EXEC sp_rename 'RouteOption_Temp','RouteOption';");
            query.AppendLine("EXEC sp_rename 'ZoneRates_Temp','ZoneRates';");
            query.AppendLine("EXEC sp_rename 'CodeMatch_Temp','CodeMatch';");
            query.AppendLine("EXEC sp_rename 'ZoneMatch_Temp','ZoneMatch';");
            query.AppendLine("EXEC sp_rename 'RoutePool_Temp','RoutePool';");
            query.AppendLine("EXEC sp_rename 'RouteOptionsPool_Temp','RouteOptionsPool';");
            query.AppendLine("EXEC sp_rename 'RouteBlockConcatinated_Temp','RouteBlockConcatinated';");

            if (!Routing_TOne_Testing)
                query.AppendLine("EXEC sp_rename 'PK_ZoneMatch_Temp','PK_ZoneMatch';");

            ExecuteNonQueryText(query.ToString(), null);
        }


        void CreateIndex(string query, Action<string> trackStep, string obejctName, int commandTimeoutInSeconds)
        {
            DateTime startDate = DateTime.Now;
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            DateTime endDate = DateTime.Now;
            trackStep(string.Format("Building Indexes for {0} table took {1}", obejctName, endDate.Subtract(startDate)));
        }

        void FillData(string query, Action<string> trackStep, string obejctName, int commandTimeoutInSeconds)
        {
            DateTime startDate = DateTime.Now;
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            DateTime endDate = DateTime.Now;
            trackStep(string.Format("Building {0} table took {1}", obejctName, endDate.Subtract(startDate)));
        }

        #region Queries

        const string query_DropCustomerRouteTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Route' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.Route;
                                                      if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteOption' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.RouteOption;";

        const string query_DropZoneRateTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ZoneRates' AND TABLE_SCHEMA = 'dbo')
                                                    drop table dbo.ZoneRates;";

        const string query_DropCodeMatchTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CodeMatch' AND TABLE_SCHEMA = 'dbo')
                                                     drop table dbo.CodeMatch;";

        const string query_DropRoutePoolTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RoutePool' AND TABLE_SCHEMA = 'dbo')
                                                    drop table dbo.RoutePool;";

        const string query_DropRouteOptionsPoolTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteOptionsPool' AND TABLE_SCHEMA = 'dbo')
                                                            drop table dbo.RouteOptionsPool;";

        const string query_DropRouteBlockConcatinatedTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteBlockConcatinated' AND TABLE_SCHEMA = 'dbo')
                                                                drop table dbo.RouteBlockConcatinated;";

        const string query_DropZoneMatchTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ZoneMatch' AND TABLE_SCHEMA = 'dbo')
                                                    drop table dbo.ZoneMatch;";

        const string query_CreateZoneRateIndexes = @"  CREATE NONCLUSTERED INDEX [IX_ZoneRates_ServicesFlag] ON [dbo].[ZoneRates_Temp]
                                                        (
                                                            [ServicesFlag] ASC
                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

                                                        CREATE NONCLUSTERED INDEX [IX_ZoneRates_SupplierIsBlock] ON [dbo].[ZoneRates_Temp]
                                                        (	
                                                            [SupplierID] ASC, [IsBlock] ASC, [ZoneID] ASC
                                                        )
                                                        INCLUDE 
                                                        ( 
                                                            [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]
                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_CreateCodeMatchIndexes = @"  CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [Code] ASC
                                                      )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];
                                                      
                                                      CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Supplier] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [SupplierID] ASC
                                                      )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

                                                      CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Zone] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [SupplierZoneID] ASC
                                                      )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_CreateZoneMatchIndexes = @"  ALTER TABLE [dbo].[ZoneMatch_Temp] ADD  CONSTRAINT [PK_ZoneMatch_Temp] PRIMARY KEY CLUSTERED 
                                                        (
                                                        	[OurZoneID] ASC,
                                                        	[SupplierZoneID] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON {0}) ON [{1}];

                                                        CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierZoneID] ON [dbo].[ZoneMatch_Temp]
                                                        (
                                                        	[SupplierZoneID] ASC
                                                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON {0}) ON [{1}];";

        const string query_CreateRouteBlockConcatinatedIndexes = @" CREATE NONCLUSTERED INDEX [IX_RouteBlockConcatinated_multikey] ON [dbo].[RouteBlockConcatinated_Temp]
                                                                    (
	                                                                    [CustomerID] ASC,
	                                                                    [SupplierID] ASC,
	                                                                    [Code] ASC
                                                                    )
                                                                    INCLUDE ([ZoneID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON {0}) ON [{1}];";


        const string query_CreateRouteIndexes = @"  CREATE CLUSTERED INDEX [PK_RouteID] ON [dbo].[Route_Temp] 
			                                        (
				                                        [RouteID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[Route_Temp] 
			                                        (
				                                        [OurZoneID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[Route_Temp] 
			                                        (
				                                        [Updated] DESC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[Route_Temp] 
			                                        (
				                                        [CustomerID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[Route_Temp] 
			                                        (
				                                        [Code] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_CreateRouteOptionIndexes = @"CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[RouteOption_Temp] 
			                                            (
				                                            [Updated] DESC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                            CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[RouteOption_Temp]
			                                            (
				                                            [SupplierZoneID] ASC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

			                                            CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[RouteOption_Temp] 
			                                            (
				                                            [RouteID] ASC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_CreateRoutePoolIndexes = @"CREATE NONCLUSTERED INDEX [IX_RoutePool_ZoneIncCode] ON [dbo].[RoutePool_Temp] 
                                                    (	
                                                        [ZoneID] ASC
                                                    )INCLUDE ( [Code]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];
		
                                                    CREATE NONCLUSTERED INDEX [IX_RoutePool_CodeIncZone] ON [dbo].[RoutePool_Temp] (	
                                                        [Code] ASC
                                                    )INCLUDE ( [ZoneID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_CreateRouteOptionsPoolIndexes = @"CREATE NONCLUSTERED INDEX [IX_RouteOptionsPool_CodeSupplierID] ON [dbo].[RouteOptionsPool_Temp] 
                                                            (	
                                                                [SupplierID] ASC,	
                                                                [Code] ASC
                                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];
		
                                                            CREATE NONCLUSTERED INDEX [IX_RouteCodePool_SupplierZone] ON [dbo].[RouteOptionsPool_Temp] 
                                                            (	
                                                                [SupplierID] ASC,	
                                                                [SupplierZoneID] ASC
                                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];

                                                            CREATE NONCLUSTERED INDEX [IX_RouteCodePool_Multikeys] ON [dbo].[RouteOptionsPool_Temp]
                                                            (
                                                                [SupplierID] ASC,
                                                                [Code] ASC,
                                                                [SupplierZoneID] ASC,
                                                                [ActiveRate] ASC,	
                                                                [SupplierServicesFlag] ASC
                                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON {0}) ON [{1}];";

        const string query_FillZoneMacthTemp = @" INSERT INTO ZoneMatch_Temp (OurZoneID, SupplierZoneID)
                                                  SELECT DISTINCT OC.SupplierZoneID, SC.SupplierZoneID 
                                                  FROM   [dbo].[CodeMatch_Temp] OC WITH(NOLOCK), [dbo].[CodeMatch_Temp] SC WITH(NOLOCK)
                                                  WHERE  OC.Code = SC.Code 
                                                  AND OC.SupplierID = 'SYS' 
                                                  AND SC.SupplierID <> 'SYS'";

        const string query_FillRouteBlockConcatinatedTemp = @"CREATE TABLE #DistinctParentRules (RuleID INT,ParentCode varchar(20), ExcludedCodes VARCHAR(max), MainExcluded varchar(max), GeneratedExcluding varchar(max))
                                                              
                                                              INSERT INTO RouteBlockConcatinated_Temp
                                                              SELECT RouteBlockID, CustomerID, SupplierID, Code, ZoneID, UpdateDate, IncludeSubCodes, ExcludedCodes, NULL parentID, ExcludedCodes								
							                                  FROM RouteBlock WITH (NOLOCK) 
							                                  WHERE IsEffective = 'Y' AND CustomerID IS NULL AND SupplierID IS NOT NULL AND ZoneID IS NULL AND Code IS NOT NULL 


                                                             UPDATE RouteBlockConcatinated_Temp 
                                                             SET parentID = (SELECT TOP 1 RouteBlockID FROM RouteBlockConcatinated_Temp cbr2 
                                                                             WHERE RouteBlockConcatinated_Temp.Code LIKE (cbr2.code + '%') 
                                                                             AND LEN( RouteBlockConcatinated_Temp.Code) != LEN( cbr2.code) 
                                                                             AND RouteBlockConcatinated_Temp.SupplierID = cbr2.SupplierID 
                                                                             ORDER BY code ASC) 
                                                            WHERE RouteBlockConcatinated_Temp.IncludeSubCodes= 'Y'
 
 
                                                            INSERT INTO #DistinctParentRules With(Tablock)
                                                            SELECT DISTINCT(tor.RouteBlockID), tor.Code, tor.ExcludedCodes, '', tor.OriginalExcluded
                                                            FROM RouteBlockConcatinated_Temp tor 
	                                                        WHERE EXISTS (SELECT * FROM RouteBlockConcatinated_Temp ov WHERE tor.RouteBlockID = ov.ParentID)
	                                                        ORDER BY tor.Code DESC	
 

                                                            DECLARE @RuleID INT
                                                            DECLARE @ExcludedCodes varchar(max)
                                                            DECLARE	@Result varchar(max)
                                                            DECLARE @ResultWithSubCode varchar(max)
                                                            DECLARE @ResultWithOutSubCode varchar(max)
                                                            DECLARE DistinctRulesCursor CURSOR LOCAL FOR select RuleID from #DistinctParentRules
                                                            OPEN DistinctRulesCursor
                                                            FETCH NEXT FROM DistinctRulesCursor into @RuleID
                                                            
                                                            WHILE @@FETCH_STATUS = 0
                                                            BEGIN
                                                            	
                                                            	SET @Result = NULL
                                                            	SET @ResultWithOutSubCode = NULL
                                                            	SET @ResultWithSubCode = NULL

                                                            	Select @ResultWithOutSubCode = COALESCE(@ResultWithOutSubCode + ',', '') + Code  
                                                                                               FROM  RouteBlockConcatinated_Temp 
                                                                                               WHERE ParentID = @RuleID 
                                                                                               AND IncludeSubCodes = 'N' 
                                                            	
                                                            	Select @ResultWithSubCode = COALESCE(@ResultWithSubCode + ',', '') + tor.Code  
                                                            								FROM  RouteBlockConcatinated_Temp tor 
                                                            								where routeblockid = @RuleID OR ParentID = @RuleID AND IncludeSubCodes = 'Y' 
                                                            
                                                            	
                                                            	SET @Result =
                                                            	CASE WHEN  @ResultWithOutSubCode IS Null THEN '' ELSE (@ResultWithOutSubCode + ',') END
                                                            	 + CASE WHEN @ResultWithSubCode IS NULL THEN '' ELSE (@ResultWithSubCode + ',') END 
                                                            	
                                                            	SET @Result = CASE WHEN len(@Result)-1 <=0 THEN ',' ELSE  isnull(substring(','+@Result,1,len(','+@Result)-1),'') END
                                                            	
                                                            	UPDATE RouteBlockConcatinated_Temp SET ExcludedCodes = isnull(OriginalExcluded, '') + @Result WHERE routeblockid = @RuleID
                                                            	
                                                                FETCH NEXT FROM DistinctRulesCursor into @RuleID
                                                            END
                                                            
                                                            CLOSE DistinctRulesCursor
                                                            DEALLOCATE DistinctRulesCursor";


        const string query_FillPoolTablesTemp = @";WITH BlockedCodesTemp AS (
                                                    SELECT CM.Code as BlockCode, cm.SupplierID, 1 IsBlock
                                                    FROM CodeMatch_Temp CM 
		                                            LEFT JOIN RouteBlockConcatinated_Temp grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
                                                    WHERE (grb.SupplierID <> 'Sys' AND grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT  like (grb.Code + '%') AND CM.Code NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
		                                                  OR (grb.SupplierID <> 'Sys' and grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 	
                                                  )			
		
                                                INSERT INTO RouteOptionsPool_Temp With(Tablock)
                                                SELECT CM.Code, CM.SupplierID, CM.SupplierZoneID, ZR.ServicesFlag, ZR.ProfileID, ZR.ActiveRate, ISNULL(bc.IsBlock,zr.IsBlock) IsBlock, ZR.IsTOD
                                                FROM CodeMatch_Temp CM 
                                                INNER JOIN ZoneRates_Temp ZR ON CM.SupplierZoneID = ZR.ZoneID AND ZR.SupplierID <> 'SYS'
		                                        LEFT JOIN BlockedCodesTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode COLLATE DATABASE_DEFAULT AND bc.SupplierID COLLATE DATABASE_DEFAULT = ZR.SupplierID COLLATE DATABASE_DEFAULT

                                                ;WITH BlockedCodesPoolTemp AS (
                                                SELECT CM.Code as	BlockCode ,cm.SupplierID, 1 IsBlock
                                                FROM CodeMatch_Temp CM 
		                                        LEFT JOIN RouteBlockConcatinated_Temp grb ON cm.SupplierID COLLATE DATABASE_DEFAULT  = grb.SupplierID
                                                WHERE (grb.SupplierID = 'Sys' AND  grb.IncludeSubCodes = 'Y' AND  CM.Code COLLATE DATABASE_DEFAULT like (grb.Code + '%') AND CM.Code   NOT IN (SELECT * FROM ParseArray(grb.ExcludedCodes,','))) 
		                                              OR (grb.SupplierID = 'Sys' AND grb.IncludeSubCodes = 'N' AND CM.Code COLLATE DATABASE_DEFAULT  = grb.Code) 
                                                )	
	
                                                INSERT INTO RoutePool_Temp With(Tablock)
                                                SELECT CM.Code, CM.SupplierZoneID, ISNULL(bc.IsBlock,0) IsBlock, z.CodeGroup CodeGroup
                                                FROM CodeMatch_Temp CM 
		                                        LEFT JOIN BlockedCodesPoolTemp bc ON cm.Code COLLATE DATABASE_DEFAULT = bc.BlockCode
		                                        LEFT JOIN Zone z ON z.ZoneID = cm.SupplierZoneID
                                                WHERE	CM.SupplierID = 'sys' AND z.IsEffective = 'y' AND z.SupplierID = 'sys'";
        #endregion

        private class CustomerRouteBulkInsert
        {
            public StreamForBulkInsert RouteStreamForBulkInsert { get; set; }
            public StreamForBulkInsert RouteOptionStreamForBulkInsert { get; set; }
        }

        private class CustomerRouteBulkInsertInfo
        {
            public StreamBulkInsertInfo RouteStreamForBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo RouteOptionStreamForBulkInsertInfo { get; set; }
        }

        public Vanrise.BusinessProcess.IBPContext BPContext
        {
            set;
            get;
        }

        public long GetTotalCount()
        {
            throw new NotImplementedException();
        }

        public List<CustomerRoute> GetCustomerRoutesAfterVersionNb(int versionNb)
        {
            throw new NotImplementedException();
        }
    }

    public class ReserveRouteIdsRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<ReserveRouteIdsResponse>
    {
        public long ParentBPInstanceId { get; set; }
        static Dictionary<long, int> s_routingBPInstanceLastReservedRouteId = new Dictionary<long, int>();
        public override ReserveRouteIdsResponse Execute()
        {
            ReserveRouteIdsResponse response = new ReserveRouteIdsResponse();
            lock (s_routingBPInstanceLastReservedRouteId)
            {
                int lastTakenId = s_routingBPInstanceLastReservedRouteId.GetOrCreateItem(this.ParentBPInstanceId);
                response.StartingId = lastTakenId + 1;
                response.EndId = response.StartingId + 50000;
                s_routingBPInstanceLastReservedRouteId[this.ParentBPInstanceId] = response.EndId;
            }
            return response;
        }
    }

    public class ReserveRouteIdsResponse
    {
        public int StartingId { get; set; }

        public int EndId { get; set; }
    }
}