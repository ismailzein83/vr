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
    public class ChangedSaleCodeDataManager : BaseTOneDataManager, IChangedSaleCodeDataManager
    {
        public ChangedSaleCodeDataManager()
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
        public void Insert(long processInstanceID, IEnumerable<ChangedCode> changedCodes)
        {

            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (ChangedCode code in changedCodes)
            {
                WriteRecordToStream(code, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);

        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(ChangedCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}",
                       record.CodeId,
                       _processInstanceID,
                       record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.CP_SaleCode_Changed",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        public void ApplyChangedCodesToDB(object preparedObject, long processInstanceID)
        {
            _processInstanceID = processInstanceID;
            InsertBulkToTable(preparedObject as BaseBulkInsertInfo);
        }
    }
}
