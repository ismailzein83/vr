using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data
{
    public class ChangedDefaultServiceDataManager : BaseSQLDataManager, IChangedDefaultServiceDataManager
    {
        #region Fields / Properties

        readonly string[] columns = { "ID", "ProcessInstanceID", "EED" };

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set { _processInstanceId = value; }
        }

        #endregion

        #region Constructors

        public ChangedDefaultServiceDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void ApplyChangedDefaultServicesToDB(IEnumerable<ChangedDefaultService> changedDefaultServices)
        {
            IBulkApplyDataManager<ChangedDefaultService> bulkApplyDataManager = this;
            object streamForDBApply = bulkApplyDataManager.InitialiazeStreamForDBApply();

            foreach (var changedDefaultService in changedDefaultServices)
                bulkApplyDataManager.WriteRecordToStream(changedDefaultService, streamForDBApply);

            object bulkInsertInfo = bulkApplyDataManager.FinishDBApplyStream(streamForDBApply);
            this.InsertBulkToTable(bulkInsertInfo as StreamBulkInsertInfo);
        }

        #endregion

        #region Private Methods

        object Vanrise.Data.IBulkApplyDataManager<ChangedDefaultService>.InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        void Vanrise.Data.IBulkApplyDataManager<ChangedDefaultService>.WriteRecordToStream(ChangedDefaultService record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord
            (
                "{0}^{1}^{2}",
                record.SaleEntityServiceId,
                _processInstanceId,
                record.EED
            );
        }

        object Vanrise.Data.IBulkApplyDataManager<ChangedDefaultService>.FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_Sales.RP_DefaultService_Changed",
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
