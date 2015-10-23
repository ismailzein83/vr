using System;
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

        public IEnumerable<SaleZone> GetAllSaleZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetAll", SaleZoneMapper);
        }

        public List<SaleZone> GetSaleZones(int packageId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetByPackage", SaleZoneMapper, packageId);
        }

        public List<SaleZone> GetSaleZones(int packageId,DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetByPackageAndEffectiveDate", SaleZoneMapper, packageId, effectiveDate);
        }
     
        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone sellingNumberPlan = new SaleZone
            {
                SaleZoneId = (long)reader["ID"],
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"],
                Name = reader["Name"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED")
            };
            return sellingNumberPlan;
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
                       record.SellingNumberPlanId,
                       record.Name,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate);
        }
        public void ApplySaleZonesForDB(object preparedSaleZones)
        {
            InsertBulkToTable(preparedSaleZones as BaseBulkInsertInfo);
        }



        public void DeleteSaleZones(List<SaleZone> saleZones)
        {
            DataTable dtSaleZonesToUpdate = GetSaleZonesTable();

            dtSaleZonesToUpdate.BeginLoadData();

            foreach (var item in saleZones)
            {
                SaleZone saleZone = new SaleZone
                {   SaleZoneId=item.SaleZoneId,
                    EndEffectiveDate = item.EndEffectiveDate
                };
                    DataRow dr = dtSaleZonesToUpdate.NewRow();
                    FillSaleZoneRow(dr, saleZone);
                    dtSaleZonesToUpdate.Rows.Add(dr);
            }
            if (dtSaleZonesToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[TOneWhS_BE].[sp_SaleZone_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@SaleZones", SqlDbType.Structured);
                        dtPrm.Value = dtSaleZonesToUpdate;
                        cmd.Parameters.Add(dtPrm);}
                    );
        }
        

        DataTable GetSaleZonesTable()
        {
            DataTable dt = new DataTable("SaleZones");
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("EED", typeof(DateTime));
            return dt;
        }
        void FillSaleZoneRow(DataRow dr, SaleZone saleZone)
        {
            dr["ID"] = saleZone.SaleZoneId;
            dr["EED"] = saleZone.EndEffectiveDate;
        }


        public void InsertSaleZones(List<SaleZone> saleZones)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SaleZone saleZone in saleZones)
               WriteRecordToStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones =FinishDBApplyStream(dbApplyStream);
           ApplySaleZonesForDB(prepareToApplySaleZones);
        }

        public List<SaleZoneInfo> GetSaleZonesInfo(int packageId, string filter)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZoneInfo_GetFiltered", SaleZoneInfoMapper, packageId, filter);
        }

        public List<SaleZoneInfo> GetSaleZonesInfoByIds(int packageId, List<long> saleZoneIds)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZoneInfo_GetByPackageAndZoneIds", SaleZoneInfoMapper, packageId, string.Join(",", saleZoneIds));
        }

        SaleZoneInfo SaleZoneInfoMapper(IDataReader reader)
        {
            SaleZoneInfo saleZoneInfo = new SaleZoneInfo
            {
                SaleZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
            };
            return saleZoneInfo;
        }


        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("TOneWhS_BE.SaleZone", ref lastReceivedDataInfo);
        }
    }
}
