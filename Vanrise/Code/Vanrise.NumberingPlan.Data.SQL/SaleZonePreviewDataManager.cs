using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class SaleZonePreviewDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleZonePreviewDataManager
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
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {

        }


        public void ApplyPreviewZonesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }


        public IEnumerable<ZonePreview> GetFilteredZonePreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[VR_NumberingPlan].[sp_SaleZone_Preview_GetFiltered]", ZonePreviewMapper, query.ProcessInstanceId, query.CountryId, query.OnlyModified);
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
                record.ZoneBED,
                record.ZoneEED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "VR_NumberingPlan.SaleZone_Preview",
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
