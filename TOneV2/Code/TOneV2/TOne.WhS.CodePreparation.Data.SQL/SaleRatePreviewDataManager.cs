using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class SaleRatePreviewDataManager : BaseTOneDataManager, ISaleRatePreviewDataManager
    {
        readonly string[] _columns = { "ProcessInstanceID", "ZoneName", "OwnerType", "OwnerID", "Rate", "BED", "EED" };

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;

        public SaleRatePreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public void ApplyPreviewRatesToDB(object preparedRates)
        {
            InsertBulkToTable(preparedRates as BaseBulkInsertInfo);
        }

        public IEnumerable<RatePreview> GetFilteredRatesPreview(SPLPreviewQuery query)
        {
            return GetItemsSP("[TOneWhs_CP].[sp_SaleRate_Preview_GetFiltered]", RatePreviewMapper, query.ProcessInstanceId, query.ZoneName);
        }
 

        object Vanrise.Data.IBulkApplyDataManager<RatePreview>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(RatePreview record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                _processInstanceID,
                record.ZoneName,
                (int)record.OnwerType,
                record.OwnerId,
                record.Rate,
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
                TableName = "TOneWhS_CP.SaleRate_Preview",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }


        private RatePreview RatePreviewMapper(IDataReader reader)
        {
            RatePreview ratePreview = new RatePreview
            {
                OnwerType = (SalePriceListOwnerType)GetReaderValue<byte>(reader, "OwnerType"),
                OwnerId = (int) reader["OwnerID"],
                Rate = (decimal) reader["Rate"],
                BED = (DateTime) reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return ratePreview;
        }
    }
}
