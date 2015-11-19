using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class CodePreparationDataManager : BaseTOneDataManager, ICodePreparationDataManager
    {
        public CodePreparationDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }


        public object FinishSaleZoneDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SaleZone]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object FinishSaleCodeDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SaleCode]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public object InitialiazeZonesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public object InitialiazeCodesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToZonesStream(SaleZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^",
                       record.SaleZoneId,
                       record.SellingNumberPlanId,
                       record.CountryId,
                       record.Name,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate.HasValue ? (DateTime?)record.EndEffectiveDate.Value : null);
        }

        public void WriteRecordToCodesStream(SaleCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}",
                       0,
                       record.Code,
                       record.ZoneId,
                       record.CodeGroupId,
                       record.BeginEffectiveDate,
                       record.EndEffectiveDate.HasValue ? (DateTime?)record.EndEffectiveDate.Value : null);
        }

        public void ApplySaleZonesForDB(object preparedSaleZones)
        {
            InsertBulkToTable(preparedSaleZones as BaseBulkInsertInfo);
        }

        public void ApplySaleCodesForDB(object preparedSaleCodes)
        {
            InsertBulkToTable(preparedSaleCodes as BaseBulkInsertInfo);
        }

        public void DeleteSaleZones(List<SaleZone> saleZones)
        {
            DataTable dtSaleZonesToUpdate = GetSaleZonesTable();

            dtSaleZonesToUpdate.BeginLoadData();

            foreach (var item in saleZones)
            {
                SaleZone saleZone = new SaleZone
                {
                    SaleZoneId = item.SaleZoneId,
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
                        cmd.Parameters.Add(dtPrm);
                    }
                    );
        }
        public void DeleteSaleCodes(List<SaleCode> saleCodes)
        {
            DataTable dtSaleZCodesToUpdate = GetSaleCodesTable();

            dtSaleZCodesToUpdate.BeginLoadData();

            foreach (var item in saleCodes)
            {
                SaleCode saleCode = new SaleCode
                {
                    SaleCodeId = item.SaleCodeId,
                    EndEffectiveDate = item.EndEffectiveDate
                };
                DataRow dr = dtSaleZCodesToUpdate.NewRow();
                FillSaleCodeRow(dr, saleCode);
                dtSaleZCodesToUpdate.Rows.Add(dr);
            }
            if (dtSaleZCodesToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[TOneWhS_BE].[sp_SaleCode_Update]",
                    (cmd) =>
                    {
                        var dtPrm = new System.Data.SqlClient.SqlParameter("@SaleCodes", SqlDbType.Structured);
                        dtPrm.Value = dtSaleZCodesToUpdate;
                        cmd.Parameters.Add(dtPrm);
                    }
                    );
        }

        DataTable GetSaleCodesTable()
        {
            DataTable dt = new DataTable("SaleCodes");
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("EED", typeof(DateTime));
            return dt;
        }
        void FillSaleCodeRow(DataRow dr, SaleCode saleCode)
        {
            dr["ID"] = saleCode.SaleCodeId;
            if (saleCode.EndEffectiveDate != null)
              dr["EED"] = saleCode.EndEffectiveDate;
        }

        public void InsertSaleCodes(List<SaleCode> saleCodes)
        {
            object dbApplyStream = InitialiazeCodesStreamForDBApply();
            foreach (SaleCode saleCode in saleCodes)
                WriteRecordToCodesStream(saleCode, dbApplyStream);
            object prepareToApplySaleCodes = FinishSaleCodeDBApplyStream(dbApplyStream);
            ApplySaleCodesForDB(prepareToApplySaleCodes);
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
            if (saleZone.EndEffectiveDate!=null)
              dr["EED"] = saleZone.EndEffectiveDate;
        }

        public void InsertSaleZones(List<SaleZone> saleZones)
        {
            object dbApplyStream = InitialiazeZonesStreamForDBApply();
            foreach (SaleZone saleZone in saleZones)
                WriteRecordToZonesStream(saleZone, dbApplyStream);
            object prepareToApplySaleZones = FinishSaleZoneDBApplyStream(dbApplyStream);
            ApplySaleZonesForDB(prepareToApplySaleZones);
        }


    }
}
