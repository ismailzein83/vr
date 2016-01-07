using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Data;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class NewSaleCodeDataManager : BaseTOneDataManager, INewSaleCodeDataManager
    {
        public NewSaleCodeDataManager()
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
        public void Insert(long processInstanceID, IEnumerable<AddedCode> codesList)
        {

            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (AddedCode code in codesList)
            {
                WriteRecordToStream(code, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "TOneWhS_BE.CP_SaleCode_New",
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

        public void WriteRecordToStream(AddedCode record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                       record.CodeId,
                       _processInstanceID,
                       record.Code,
                       record.Zone.ZoneId,
                       record.CodeGroupId,
                       record.BED,
                       record.EED);
        }


        public void ApplyNewCodesToDB(object preparedCodes, long processInstanceID)
        {
            _processInstanceID = processInstanceID;
            InsertBulkToTable(preparedCodes as BaseBulkInsertInfo);
        }
    }
}
