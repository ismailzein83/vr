using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierZonePreviewDataManager : BaseTOneDataManager, ISupplierZonePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "Name", "ChangeType", "BED", "EED" };
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static SupplierZonePreviewDataManager()
        {
            _columnMapper.Add("ChangeTypeDecription", "ChangeType");
        }

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SupplierZonePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_SPL_DBConnStringKey", "TOneWhS_SPL_DBConnString"))
        {

        }


        public void ApplyPreviewZonesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }


        public Vanrise.Entities.BigResult<Entities.ZonePreview> GetZonePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {

                ExecuteNonQuerySP("[TOneWhS_SPL].[sp_SupplierZone_Preview_CreateTempByFiltered]", tempTableName, input.Query.PriceListId);
            };
            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, ZonePreviewMapper, _columnMapper);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZonePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZonePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}",
                _processInstanceID,
                record.Name,
                (int)record.ChangeType,
                record.BED,
                record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
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

        private ZonePreview ZonePreviewMapper(IDataReader reader)
        {
            ZonePreview zonePreview = new ZonePreview
            {
                Name = reader["Name"] as string,
                ChangeType = (ZoneChangeType)GetReaderValue<int>(reader, "ChangeType"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return zonePreview;
        }
    }
}
