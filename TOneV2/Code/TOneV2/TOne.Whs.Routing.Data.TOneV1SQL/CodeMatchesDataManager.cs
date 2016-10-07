﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CodeMatchesDataManager : RoutingDataManager, ICodeMatchesDataManager
    {
        readonly string[] columns = { "Code", "SupplierCodeID", "SupplierZoneID", "SupplierID" };

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        SaleZoneManager _saleZoneManager = new SaleZoneManager();
        SupplierZoneManager _supplierZoneManager = new SupplierZoneManager();
        Dictionary<int, CarrierAccount> _allCarriers;
        Dictionary<long, SaleZone> _allSaleZones;
        Dictionary<long, SupplierZone> _allSupplierZones;


        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void ApplyCodeMatchesForDB(object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CodeMatch_Temp]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void WriteRecordToStream(TOne.WhS.Routing.Entities.CodeMatches record, object dbApplyStream)
        {
            InitializeData();

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            SaleCodeMatch saleCodeMatch = record.SaleCodeMatches.First();
            SaleZone saleZone = _allSaleZones.GetRecord(saleCodeMatch.SaleZoneId);
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.Code, saleCodeMatch.SaleCodeSourceId, saleZone.SourceId, "SYS");

            foreach (SupplierCodeMatchWithRate supplierCodeMatchWithRate in record.SupplierCodeMatches)
            {
                if (supplierCodeMatchWithRate.CodeMatch == null)
                    throw new NullReferenceException("supplierCodeMatchWithRate.CodeMatch");

                CarrierAccount supplier = _allCarriers.GetRecord(supplierCodeMatchWithRate.CodeMatch.SupplierId);
                SupplierZone supplierZone = _allSupplierZones.GetRecord(supplierCodeMatchWithRate.CodeMatch.SupplierZoneId);

                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.Code, supplierCodeMatchWithRate.CodeMatch.SupplierCodeSourceId, supplierZone.SourceId, supplier.SourceId);
            }
        }

        public IEnumerable<RPCodeMatches> GetCodeMatches(long fromZoneId, long toZoneId)
        {
            throw new NotImplementedException();
        }


        private void InitializeData()
        {
            if (_allCarriers == null)
                _allCarriers = _carrierAccountManager.GetCachedCarrierAccounts();
            if (_allSaleZones == null)
                _allSaleZones = _saleZoneManager.GetCachedSaleZones();
            if (_allSupplierZones == null)
                _allSupplierZones = _supplierZoneManager.GetCachedSupplierZones();
        }
    }
}
