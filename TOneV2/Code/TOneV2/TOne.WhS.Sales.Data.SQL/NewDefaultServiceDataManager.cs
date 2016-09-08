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
    public class NewDefaultServiceDataManager : BaseSQLDataManager, INewDefaultServiceDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "Services", "BED", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public NewDefaultServiceDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyNewDefaultServicesToDB(IEnumerable<NewDefaultService> newDefaultServices)
        {
            IBulkApplyDataManager<NewDefaultService> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var newDefaultService in newDefaultServices)
                bulkApplyDataManager.WriteRecordToStream(newDefaultService, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        public bool Insert(NewDefaultService newDefaultService)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_NewDefaultService_Insert", newDefaultService.SaleEntityServiceId, _processInstanceId, Vanrise.Common.Serializer.Serialize(newDefaultService.Services), newDefaultService.BED, newDefaultService.EED);

            return affectedRows > 0;
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<NewDefaultService>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<NewDefaultService>.WriteRecordToStream(NewDefaultService record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}^{3}^{4}",
                record.SaleEntityServiceId,
                _processInstanceId,
                Vanrise.Common.Serializer.Serialize(record.Services),
                record.BED,
                record.EED
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<NewDefaultService>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_DefaultService_New",
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
