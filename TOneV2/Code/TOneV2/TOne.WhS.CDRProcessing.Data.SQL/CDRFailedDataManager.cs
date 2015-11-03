using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
    public class CDRFailedDataManager : BaseSQLDataManager, ICDRFailedDataManager
    {
        public CDRFailedDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(BillingFailedCDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                                    record.ID,
                                    record.CustomerId,
                                    record.SupplierId,
                                    record.Attempt,
                                    record.DurationInSeconds,
                                    record.Alert,
                                    record.Connect,
                                    record.Disconnect,
                                    record.PortOut,
                                    record.PortIn);

       }
       public object FinishDBApplyStream(object dbApplyStream)
       {

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               TableName = "[TOneWhS_CDR].[CDRFailed]",
               Stream = streamForBulkInsert,
               TabLock = false,
               KeepIdentity = false,
               FieldSeparator = '^',
           };
       }
       public void SaveFailedCDRBatchToDB(CDRFailedBatch cdrBatch)
       {
           Object dbApplyStream = InitialiazeStreamForDBApply();
           foreach (BillingFailedCDR cdr in cdrBatch.FailedCDRs)
               WriteRecordToStream(cdr, dbApplyStream);
           Object preparedFailedCDRs = FinishDBApplyStream(dbApplyStream);
           ApplyFailedCDRsToDB(preparedFailedCDRs);
       }

       public void ApplyFailedCDRsToDB(Object cdrFailedBatch)
       {
           InsertBulkToTable(cdrFailedBatch as StreamBulkInsertInfo);
       }
    }
}
