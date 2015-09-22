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
    public class SaleCodeDataManager : BaseTOneDataManager, ISaleCodeDataManager
    {
        public SaleCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID,DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_ByZondId", SaleCodeMapper, zoneID, effectiveDate);
        }
        SaleCode SaleCodeMapper(IDataReader reader)
        {
            SaleCode saleCode = new SaleCode
            {
                SaleCodeId = (long)reader["ID"],
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED")
            };
            return saleCode;
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[SaleCode]",
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

        public void WriteRecordToStream(SaleCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                       0,
                       record.Code,
                       record.ZoneId,
                       record.BeginEffectiveDate,
                       null);
        }
        public void ApplySaleCodesForDB(object preparedSaleCodes)
        {
            InsertBulkToTable(preparedSaleCodes as BaseBulkInsertInfo);
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
            dr["EED"] = saleCode.EndEffectiveDate;
        }
    }
}
