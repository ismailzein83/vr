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

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CustomerRouteDataManager : RoutingDataManager, ICustomerRouteDataManager
    {
        static CustomerRouteDataManager()
        {
            RouteOption dummy = new RouteOption();
        }

        readonly string[] routeColumns = { "RouteID", "CustomerID", "ProfileID", "Code", "OurZoneID", "OurActiveRate", "OurServicesFlag", "State", "Updated", "IsToDAffected", "IsSpecialRequestAffected", "IsOverrideAffected", "IsBlockAffected", "IsOptionBlock", "BatchID" };

        readonly string[] routeOptionsColumns = { "RouteID", "SupplierID", "SupplierZoneID", "SupplierActiveRate", "SupplierServicesFlag", "Priority", "NumberOfTries", "State", "Updated", "Percentage" };

        public void ApplyCustomerRouteForDB(object preparedCustomerRoute)
        {
            CustomerRouteBulkInsertInfo customerRouteBulkInsertInfo = preparedCustomerRoute as CustomerRouteBulkInsertInfo;
            InsertBulkToTable(customerRouteBulkInsertInfo.RouteStreamForBulkInsertInfo);
            InsertBulkToTable(customerRouteBulkInsertInfo.RouteOptionStreamForBulkInsertInfo);
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

        public void WriteRecordToStream(TOne.WhS.Routing.Entities.CustomerRoute record, object dbApplyStream)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();

            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(record.CustomerId);
            SaleZone saleZone = saleZoneManager.GetSaleZone(record.SaleZoneId);

            CustomerRouteBulkInsert customerRouteBulkInsert = dbApplyStream as CustomerRouteBulkInsert;
            customerRouteBulkInsert.RouteStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}", 0, customer.SourceId, 0, record.Code, saleZone.SourceId,
                record.Rate, 0, 0, GetDateTimeForBCP(DateTime.Now), 0, 0, 0, record.IsBlocked ? 1 : 0, 0, 0);

            foreach (RouteOption option in record.Options)
            {
                CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(option.SupplierId);
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(option.SupplierZoneId);
                customerRouteBulkInsert.RouteOptionStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}", 0, supplier.SourceId, supplierZone.SourceId, option.SupplierRate, 0, 0, 0, 0, GetDateTimeForBCP(DateTime.Now), option.Percentage.HasValue ? Convert.ToInt32(option.Percentage.Value) : default(decimal?));
            }
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
            query.AppendLine(query_DropSupplierZoneDetailsTable);
            query.AppendLine(query_DropCodeSaleZoneTable);
            query.AppendLine(query_DropCustomerZoneDetailTable);
            query.AppendLine(query_DropCustomerRouteTable);
            query.AppendLine("EXEC sp_rename 'Route_Temp','Route';");
            query.AppendLine("EXEC sp_rename 'RouteOption_Temp','RouteOption';");
            ExecuteNonQueryText(query.ToString(), null);
        }

        #region Queries

        const string query_DropCustomerRouteTable = @"if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'Route' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.Route;
                                                      if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'RouteOption' AND TABLE_SCHEMA = 'dbo')
                                                      drop table dbo.RouteOption;";

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
    }
}