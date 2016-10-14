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

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        public int ParentWFRuntimeProcessId { get; set; }

        public long ParentBPInstanceId { get; set; }

        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        readonly string[] routeColumns = { "RouteID", "CustomerID", "ProfileID", "Code", "OurZoneID", "OurActiveRate", "OurServicesFlag", "State", "Updated", "IsToDAffected", "IsSpecialRequestAffected", "IsOverrideAffected", "IsBlockAffected", "IsOptionBlock", "BatchID" };

        readonly string[] routeOptionsColumns = { "RouteID", "SupplierID", "SupplierZoneID", "SupplierActiveRate", "SupplierServicesFlag", "Priority", "NumberOfTries", "State", "Updated", "Percentage" };

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
                ColumnNames = routeColumns,
            };

            customerRouteBulkInsertInfo.RouteOptionStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[RouteOption_Temp]",
                Stream = customerRouteBulkInsert.RouteOptionStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = routeOptionsColumns,
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

            int customerServiceFlag = GetServiceFlag(record.CustomerServiceIds, _allZoneServiceConfigs);
            DateTime now = DateTime.Now;
            bool hasOptionBlock = false;

            CustomerRouteBulkInsert customerRouteBulkInsert = dbApplyStream as CustomerRouteBulkInsert;
            int counter = 0;

            int isToDAffected = 0;
            int isSpecialRequestAffected = 0;
            int isOverrideAffected = 0;
            int isBlockAffected = 0;

            switch (record.CorrespondentType)
            {
                case CorrespondentType.Block: isBlockAffected = 1; break;
                case CorrespondentType.Override: isOverrideAffected = 1; counter = 10; break;
                case CorrespondentType.SpecialRequest: isSpecialRequestAffected = 1; break;
                case CorrespondentType.LCR:
                case CorrespondentType.Other:
                default: break;
            }
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
                customerRouteBulkInsert.RouteOptionStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}", routeId, supplier.SourceId, supplierZone.SourceId, option.SupplierRate,
                    supplierServiceFlag, priority, 0, option.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), option.Percentage.HasValue ? Convert.ToInt32(option.Percentage.Value) : 0);
            }

            customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}", routeId, customer.SourceId, profile.SourceId, record.Code, saleZone.SourceId,
                 record.Rate, customerServiceFlag, record.IsBlocked ? 0 : 1, GetDateTimeForBCP(now), isToDAffected, isSpecialRequestAffected, isOverrideAffected, isBlockAffected, hasOptionBlock ? 1 : 0, 0);
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

        public void FinalizeCurstomerRoute(Action<string> trackStep)
        {
            StringBuilder query = new StringBuilder();
            //query.AppendLine(query_CreatingIndexes);
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

        #region Queries

        const string query_DropCustomerRouteTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Route' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.Route;
                                                      if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteOption' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.RouteOption;";

        const string query_DropZoneRateTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'ZoneRates' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.ZoneRates;";

        const string query_DropCodeMatchTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CodeMatch' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.CodeMatch;";

        const string query_CreatingIndexes = @"CREATE NONCLUSTERED INDEX [IX_ZoneRate_Customer] ON [dbo].[ZoneRate_Temp]
                                                      (
	                                                      [CustomerID] ASC
                                                      );
                                                      CREATE NONCLUSTERED INDEX [IX_ZoneRate_ServicesFlag] ON [dbo].[ZoneRate_Temp]
                                                      (
	                                                      [ServicesFlag] ASC
                                                      );
                                                      CREATE NONCLUSTERED INDEX [IX_ZoneRate_Supplier] ON [dbo].[ZoneRate_Temp]
                                                      (
	                                                      [SupplierID] ASC
                                                      );
                                                      CREATE NONCLUSTERED INDEX [IX_ZoneRate_Zone] ON [dbo].[ZoneRate_Temp]
                                                      (
	                                                      [ZoneID] ASC
                                                      );

                                                      CREATE NONCLUSTERED INDEX [IDX_CodeMatch_Code] ON [dbo].[CodeMatch_Temp] 
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