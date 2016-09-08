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
    public class NewSaleZoneServiceDataManager : BaseSQLDataManager, INewSaleZoneServiceDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "Services", "ZoneID", "BED", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public NewSaleZoneServiceDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyNewSaleZoneServicesToDB(IEnumerable<NewSaleZoneService> newSaleZoneServices)
        {
            IBulkApplyDataManager<NewSaleZoneService> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var newSaleZoneService in newSaleZoneServices)
                bulkApplyDataManager.WriteRecordToStream(newSaleZoneService, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<NewSaleZoneService>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<NewSaleZoneService>.WriteRecordToStream(NewSaleZoneService record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}^{5}",
                record.SaleEntityServiceId,
                _processInstanceId,
                Vanrise.Common.Serializer.Serialize(record.Services),
                record.SaleZoneId,
                record.BED,
                record.EED
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<NewSaleZoneService>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_SaleZoneService_New",
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
