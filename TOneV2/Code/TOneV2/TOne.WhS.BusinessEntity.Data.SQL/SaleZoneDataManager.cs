﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleZoneDataManager : BaseTOneDataManager, ISaleZoneDataManager
    {
        public SaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public List<SaleZone> GetSaleZones(int packageId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetAll", SaleZoneMapper, packageId);
        }
        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone saleZonePackage = new SaleZone
            {
                SaleZoneId = (int)reader["ID"],
                SaleZonePackageId = (int)reader["PackageID"],
                Name = reader["Name"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED")
            };
            return saleZonePackage;
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SaleZone]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SaleZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.SaleZonePackageId,
                       record.Name,
                       record.BeginEffectiveDate,
                       null );
        }
        public void ApplySaleZonesForDB(object preparedSaleZones)
        {
            InsertBulkToTable(preparedSaleZones as BaseBulkInsertInfo);
        }

    }
}
