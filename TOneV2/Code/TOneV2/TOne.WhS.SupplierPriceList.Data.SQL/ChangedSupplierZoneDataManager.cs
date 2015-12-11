using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class ChangedSupplierZoneDataManager : BaseSQLDataManager, IChangedSupplierZoneDataManager
    {
        public ChangedSupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void Insert(int priceListId, IEnumerable<Entities.SPL.ChangedZone> changedZones)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (ChangedZone zone in changedZones)
            {
                WriteRecordToStream(priceListId, zone, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToStream(int priceListId, ChangedZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}",
                       record.ZoneId,
                       priceListId,
                       record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.SPL_SupplierZone_Changed",
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
    }
}
