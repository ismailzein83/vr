using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class NewSupplierZoneDataManager : BaseTOneDataManager, INewSupplierZoneDataManager
    {
        public NewSupplierZoneDataManager()
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

        public int SupplierId
        {
            set
            {
                _supplierId = value;
            }
        }

        int _supplierId;

        public void ApplyNewZonesToDB(object preparedZones)
        {
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);
        }

        object Vanrise.Data.IBulkApplyDataManager<NewZone>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.SPL_SupplierZone_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        object Vanrise.Data.IBulkApplyDataManager<NewZone>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(NewZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                       record.ZoneId,
                       _processInstanceID,
                       record.CountryId,
                       record.Name,
                       _supplierId,
                       GetDateTimeForBCP(record.BED),
                       GetDateTimeForBCP(record.EED),
                        (record.IsExcluded) ? 1 : 0);
          


        }
    }
}
