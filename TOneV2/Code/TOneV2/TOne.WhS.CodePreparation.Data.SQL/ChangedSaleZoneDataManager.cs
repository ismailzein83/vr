using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class ChangedSaleZoneDataManager : BaseTOneDataManager, IChangedSaleZoneDataManager
    {
        public ChangedSaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
        public void Insert(long processInstanceID, IEnumerable<ChangedZone> changedZones)
        {
            _processInstanceID = processInstanceID;
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (ChangedZone zone in changedZones)
            {
                WriteRecordToStream(zone, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
           
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ChangedZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}",
                       record.ZoneId,
                       _processInstanceID,
                       record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.CP_SaleZone_Changed",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplyChangedZonesToDB(object preparedZones, long processInstanceID)
        {
            _processInstanceID = processInstanceID;
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }
    }
}
