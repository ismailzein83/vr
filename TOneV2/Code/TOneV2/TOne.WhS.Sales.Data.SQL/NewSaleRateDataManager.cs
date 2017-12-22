using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class NewSaleRateDataManager : BaseSQLDataManager, INewSaleRateDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "PriceListID", "ZoneID", "RateTypeID", "Rate", "CurrencyID", "BED", "EED", "ChangeType" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceId = value;
            }
        }

        #endregion

        #region Constructors

        public NewSaleRateDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyNewRatesToDB(IEnumerable<NewRate> newRates)
        {
            IBulkApplyDataManager<NewRate> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var newRate in newRates)
                bulkApplyDataManager.WriteRecordToStream(newRate, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<NewRate>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<NewRate>.WriteRecordToStream(Entities.NewRate record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                record.RateId,
                _processInstanceId,
                record.PriceListId,
                record.Zone.ZoneId,
                record.RateTypeId,
                decimal.Round(record.Rate, 8),
                record.CurrencyId,
                GetDateTimeForBCP(record.BED),
                GetDateTimeForBCP(record.EED),
                (int)record.ChangeType
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<NewRate>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleRate_New",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion
    }
}
