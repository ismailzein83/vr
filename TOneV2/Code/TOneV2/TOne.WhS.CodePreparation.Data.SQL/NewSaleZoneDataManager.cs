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
    public class NewSaleZoneDataManager : BaseTOneDataManager, INewSaleZoneDataManager
    {
        public NewSaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public int SellingNumberPlanId
        {
            set
            {
                _sellingNumberPlanId = value;
            }
        }

        int _sellingNumberPlanId;
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
        public void Insert(int sellingNumberPlanId, long processInstanceID, IEnumerable<AddedZone> zonesList)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();
           
            foreach (AddedZone zone in zonesList)
            {
                WriteRecordToStream(zone, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
  
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(AddedZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.ZoneId,
                       _processInstanceID,
                       record.CountryId,
                       record.Name,
                       _sellingNumberPlanId,
                       record.BED,
                       record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.CP_SaleZone_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public void ApplyNewZonesToDB(object preparedZones, int sellingNumberPlanId, long processInstanceID)
        {
            _sellingNumberPlanId = sellingNumberPlanId;
            _processInstanceID = processInstanceID;
            InsertBulkToTable(preparedZones as BaseBulkInsertInfo);

        }
    }
}
