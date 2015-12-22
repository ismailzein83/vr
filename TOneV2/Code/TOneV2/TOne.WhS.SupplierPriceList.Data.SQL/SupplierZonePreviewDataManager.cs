using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierZonePreviewDataManager : BaseSQLDataManager, ISupplierZonePreviewDataManager
    {
        readonly string[] _columns = { "PriceListId", "Name", "ChangeType", "BED", "EED" };

        public SupplierZonePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }

        public void Insert(int priceListId, IEnumerable<Entities.ZonePreview> zonePreviewList)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (ZonePreview zone in zonePreviewList)
            {
                WriteRecordToStream(priceListId, zone, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }
        public Vanrise.Entities.BigResult<Entities.ZonePreview> GetZonePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                
                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierZone_Preview_CreateTempByFiltered]", tempTableName, input.Query.PriceListId);
            };

            return RetrieveData(input, createTempTableAction, ZonePreviewMapper);
        }
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(int priceListId, ZonePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                priceListId,
                record.Name,
                (int)record.ChangeType,
                record.BED,
                record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_SPL.SupplierZone_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private void ApplyForDB(object preparedObject)
        {
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }

        private ZonePreview ZonePreviewMapper(IDataReader reader)
        {
            ZonePreview zonePreview = new ZonePreview
            {
                Name = reader["Name"] as string,
                ChangeType = (ZoneChangeType)GetReaderValue<int>(reader, "ChangeType"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime>(reader, "EED")
            };
            return zonePreview;
        }
    }
}
