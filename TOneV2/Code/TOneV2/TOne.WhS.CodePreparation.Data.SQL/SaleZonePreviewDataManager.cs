using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class SaleZonePreviewDataManager : BaseTOneDataManager, ISaleZonePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "CountryID", "ZoneName", "RecentZoneName", "ZoneChangeType", "ZoneBED", "ZoneEED" };
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SaleZonePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }


        public void ApplyPreviewZonesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }


        public IEnumerable<ZonePreview> GetFilteredZonePreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhS_CP].[sp_SaleZone_Preview_GetFiltered]", ZonePreviewMapper, query.ProcessInstanceId, query.CountryId, query.OnlyModified);
        }


        object Vanrise.Data.IBulkApplyDataManager<ZonePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ZonePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                _processInstanceID,
                record.CountryId,
                record.ZoneName,
                record.RecentZoneName,
                (int)record.ChangeTypeZone,
               GetDateTimeForBCP(record.ZoneBED),
                GetDateTimeForBCP(record.ZoneEED));
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_CP.SaleZone_Preview",
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
                ZoneName = reader["ZoneName"] as string,
                RecentZoneName = reader["RecentZoneName"] as string,
                ChangeTypeZone = (ZoneChangeType)GetReaderValue<int>(reader, "ZoneChangeType"),
                ZoneBED = GetReaderValue<DateTime>(reader, "ZoneBED"),
                ZoneEED = GetReaderValue<DateTime?>(reader, "ZoneEED"),
                NewCodes = (int)reader["NewCodes"],
                DeletedCodes = (int)reader["DeletedCodes"],
                CodesMovedFrom = (int)reader["CodesMovedFrom"],
                CodesMovedTo = (int)reader["CodesMovedTo"]

            };
            return zonePreview;
        }



        
    }
}
