using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
    public class CDRInvalidDataManager:BaseSQLDataManager,ICDRInvalidDataManager
    {
        public CDRInvalidDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(BillingInvalidCDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}",
                                    record.ID,
                                    record.CustomerId,
                                    record.SupplierId);

       }
       public object FinishDBApplyStream(object dbApplyStream)
       {

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               TableName = "[TOneWhS_CDR].[CDRInvalid]",
               Stream = streamForBulkInsert,
               TabLock = false,
               KeepIdentity = false,
               FieldSeparator = '^',
           };
       }
       public void SaveInvalidCDRBatchToDB(CDRInvalidBatch cdrBatch)
       {
           Object dbApplyStream = InitialiazeStreamForDBApply();
           foreach (BillingInvalidCDR cdr in cdrBatch.InvalidCDRs)
               WriteRecordToStream(cdr, dbApplyStream);
           Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);
           ApplyInvalidCDRsToDB(preparedInvalidCDRs);
       }

       public void ApplyInvalidCDRsToDB(Object cdrInvalidBatch)
       {
           InsertBulkToTable(cdrInvalidBatch as StreamBulkInsertInfo);
       }
    }
}
