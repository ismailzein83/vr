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

        public void Insert(int supplierId, int priceListId, IEnumerable<Entities.SPL.NewZone> zonesList)
        {
            object dbApplyStreamNewZones = InitialiazeNewZonesStreamForDBApply();
            
            foreach (NewZone zone in zonesList)
            {
                WriteRecordToZonesStream(supplierId, priceListId, zone, dbApplyStreamNewZones);
            }

            object prepareToApplyNewZones = FinishDBApplyStream(dbApplyStreamNewZones);
        }

        private object InitialiazeNewZonesStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        private void WriteRecordToZonesStream(int supplierId, int priceListId, NewZone record, object dbApplyStreamNewZones)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStreamNewZones as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.ZoneId,
                       priceListId,
                       record.CountryId,
                       record.Name,
                       supplierId,
                       record.BED,
                       record.EED);
        }

        private object FinishDBApplyStream(object dbApplyStream)
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
    }
}
