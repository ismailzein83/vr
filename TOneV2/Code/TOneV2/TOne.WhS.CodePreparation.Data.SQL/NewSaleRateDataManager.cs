using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class NewSaleRateDataManager : BaseTOneDataManager, INewSaleRateDataManager
    {
        public NewSaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        readonly string[] _columns = { "ID", "ProcessInstanceID", "ZoneID", "PriceListID", "NormalRate", "BED", "EED" };
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
      
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = _columns,
                TableName = "TOneWhS_BE.CP_SaleRate_New",
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

        public void WriteRecordToStream(AddedRate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                        record.RateId,
                       _processInstanceID,
                       record.AddedZone.ZoneId,
                       record.PriceListToAdd.PriceListId,
                       decimal.Round(record.NormalRate, 8),
                       record.BED,
                       record.EED);
        }


        public void ApplyNewRatesToDB(object preparedRates)
        {
            InsertBulkToTable(preparedRates as BaseBulkInsertInfo);
        }

    }
}
