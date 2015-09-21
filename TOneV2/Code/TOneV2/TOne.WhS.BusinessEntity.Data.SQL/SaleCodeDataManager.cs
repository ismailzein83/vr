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
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_ByZondId", SaleCodeMapper, zoneID);
        }
        SaleCode SaleCodeMapper(IDataReader reader)
        {
            SaleCode saleCode = new SaleCode
            {
                SaleCodeId = (long)reader["ID"],
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<int>(reader, "ZoneID"),
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
    }
}
