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
                            supplierServiceFlag, priority, option.NumberOfTries, option.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), option.Percentage.HasValue ? Convert.ToInt32(option.Percentage.Value) : 0, option.ExecutedRuleId);
                    }
                    else
                    {
                        customerRouteBulkInsert.RouteOptionStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}", routeId, supplier.SourceId, supplierZone.SourceId, option.SupplierRate,
                            supplierServiceFlag, priority, option.NumberOfTries, option.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), option.Percentage.HasValue ? Convert.ToInt32(option.Percentage.Value) : 0);
                    }
                }
            }
            if (Routing_TOne_Testing)
            {
                customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}", routeId, customer.SourceId, profile.SourceId, record.Code, saleZone.SourceId,
                  record.Rate, customerServiceFlag, record.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), isToDAffected, isSpecialRequestAffected, isOverrideAffected, isBlockAffected, hasOptionBlock ? 1 : 0, 0, record.ExecutedRuleId);
            }
            else
            {
                customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}", routeId, customer.SourceId, profile.SourceId, record.Code, saleZone.SourceId,
                     record.Rate, customerServiceFlag, record.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), isToDAffected, isSpecialRequestAffected, isOverrideAffected, isBlockAffected, hasOptionBlock ? 1 : 0, 0);
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

        public Vanrise.Entities.BigResult<TOne.WhS.Routing.Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<TOne.WhS.Routing.Entities.CustomerRouteQuery> input)
        {
            throw new NotImplementedException();
        }


        public void LoadRoutes(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded)
        {
            throw new NotImplementedException();
        }

        public void FinalizeCurstomerRoute(Action<string> trackStep, int commnadTimeoutInSeconds)
        {
            StringBuilder query = new StringBuilder();
            if (!Routing_TOne_Testing)
            {
                CreateIndex(string.Format(query_CreateZoneRateIndexes, Routing_TOneV1_FileGroup), trackStep, "Zone Rate", commnadTimeoutInSeconds);
                CreateIndex(query_CreateCodeMatchIndexes, trackStep, "Code Match", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteIndexes, Routing_TOneV1_FileGroup), trackStep, "Route", commnadTimeoutInSeconds);
                CreateIndex(string.Format(query_CreateRouteOptionIndexes, Routing_TOneV1_FileGroup), trackStep, "Route Option", commnadTimeoutInSeconds);
            }

            query.AppendLine(query_DropSupplierZoneDetailsTable);
            query.AppendLine(query_DropCodeSaleZoneTable);
            query.AppendLine(query_DropCustomerZoneDetailTable);
            query.AppendLine(query_DropCustomerRouteTable);
            query.AppendLine(query_DropZoneRateTable);
            query.AppendLine(query_DropCodeMatchTable);
            query.AppendLine("EXEC sp_rename 'Route_Temp','Route';");
            query.AppendLine("EXEC sp_rename 'RouteOption_Temp','RouteOption';");
            query.AppendLine("EXEC sp_rename 'ZoneRates_Temp','ZoneRates';");
            query.AppendLine("EXEC sp_rename 'CodeMatch_Temp','CodeMatch';");
            ExecuteNonQueryText(query.ToString(), null);
        }


        void CreateIndex(string query, Action<string> trackStep, string obejctName, int commandTimeoutInSeconds)
        {
            DateTime startDate = DateTime.Now;
            ExecuteNonQueryText(query, null, commandTimeoutInSeconds);
            DateTime endDate = DateTime.Now;
            trackStep(string.Format("Building Indexes for {0} table took {1}", obejctName, endDate.Subtract(startDate)));
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

        const string query_CreateZoneRateIndexes = @"  CREATE NONCLUSTERED INDEX [IX_ZoneRates_ServicesFlag] ON [dbo].[ZoneRates_Temp]
                                                        (
                                                            [ServicesFlag] ASC
                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

                                                        CREATE NONCLUSTERED INDEX [IX_ZoneRates_SupplierIsBlock] ON [dbo].[ZoneRates_Temp]
                                                        (	
                                                            [SupplierID] ASC, [IsBlock] ASC, [ZoneID] ASC
                                                        )
                                                        INCLUDE 
                                                        ( 
                                                            [CustomerID],[ServicesFlag],[ProfileId],[ActiveRate],[IsTOD]
                                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];";

        const string query_CreateCodeMatchIndexes = @"  CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [Code] ASC
                                                      );
                                                      CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Supplier] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [SupplierID] ASC
                                                      );
                                                      CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Zone] ON [dbo].[CodeMatch_Temp] 
                                                      (
	                                                      [SupplierZoneID] ASC
                                                      );";

        const string query_CreateRouteIndexes = @"  CREATE CLUSTERED INDEX [PK_TempRouteID] ON [dbo].[Route_Temp] 
			                                        (
				                                        [RouteID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Zone] ON [dbo].[Route_Temp] 
			                                        (
				                                        [OurZoneID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Updated] ON [dbo].[Route_Temp] 
			                                        (
				                                        [Updated] DESC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Customer] ON [dbo].[Route_Temp] 
			                                        (
				                                        [CustomerID] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

			                                        CREATE NONCLUSTERED INDEX [IX_Route_Code] ON [dbo].[Route_Temp] 
			                                        (
				                                        [Code] ASC
			                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];";

        const string query_CreateRouteOptionIndexes = @"CREATE NONCLUSTERED INDEX [IDX_RouteOption_Updated] ON [dbo].[RouteOption_Temp] 
			                                            (
				                                            [Updated] DESC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];

			                                            CREATE NONCLUSTERED INDEX [IDX_RouteOption_SupplierZoneID] ON [dbo].[RouteOption_Temp]
			                                            (
				                                            [SupplierZoneID] ASC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];
			                                            CREATE CLUSTERED INDEX [IDX_RouteOption_RouteID] ON [dbo].[RouteOption_Temp] 
			                                            (
				                                            [RouteID] ASC
			                                            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [{0}];";

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