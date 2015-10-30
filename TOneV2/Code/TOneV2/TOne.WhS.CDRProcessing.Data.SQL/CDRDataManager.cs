using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
   public class CDRDataManager:BaseSQLDataManager,ICDRDataManager
    {

       public CDRDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(CDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}",
                                    record.ID,
                                    record.Attempt,
                                    record.IN_Carrier,
                                    record.IN_Trunk,
                                    record.CDPN,
                                    record.OUT_Trunk,
                                    record.OUT_Carrier);

       }
       public void ApplyRawCDRsToDB(Object preparedCDRs)
       {
           InsertBulkToTable(preparedCDRs as StreamBulkInsertInfo);
       }
      
       public object FinishDBApplyStream(object dbApplyStream)
       {

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               TableName = "[TOneWhS_CDR].[CDR]",
               Stream = streamForBulkInsert,
               TabLock = false,
               KeepIdentity = false,
               FieldSeparator = '^',
           };
       }
       public void SaveCDRBatchToDB(CDRBatch cdrBatch)
       {
           Object dbApplyStream = InitialiazeStreamForDBApply();
           foreach (CDR cdr in cdrBatch.CDRs)
               WriteRecordToStream(cdr, dbApplyStream);
           Object preparedRawCDRs = FinishDBApplyStream(dbApplyStream);
           ApplyRawCDRsToDB(preparedRawCDRs);
       }
    }
}
