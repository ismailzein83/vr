﻿using System;
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
           : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(CDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}",
                                    record.ID,
                                    record.Attempt,
                                    record.InCarrier,
                                    record.InTrunk,
                                    record.CDPN,
                                    record.OutTrunk,
                                    record.OutCarrier,
                                    record.DurationInSeconds,
                                    record.Alert,
                                    record.Connect,
                                    record.Disconnect,
                                    record.CGPN,
                                    record.PortOut,
                                    record.PortIn,
                                    record.ReleaseCode,
                                    record.ReleaseSource);

       }
       public void ApplyRawCDRsToDB(Object preparedCDRs)
       {
           InsertBulkToTable(preparedCDRs as StreamBulkInsertInfo);
       }
      
       public object FinishDBApplyStream(object dbApplyStream)
       {
           List<string> columns = new List<string>();
           columns.Add("ID");
           columns.Add("Attempt");
           columns.Add("InCarrier");
           columns.Add("InTrunk");
           columns.Add("CDPN");
           columns.Add("OutTrunk");
           columns.Add("OutCarrier");
           columns.Add("DurationInSeconds");
           columns.Add("Alert");
           columns.Add("Connect");
           columns.Add("Disconnect");
           columns.Add("CGPN");
           columns.Add("PortOut");
           columns.Add("PortIn");
           columns.Add("ReleaseCode");
           columns.Add("ReleaseSource");

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               ColumnNames=columns,
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
