﻿using System;
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
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}",
                                    record.BillingCDR.ID,
                                    record.BillingCDR.CustomerId,
                                    record.BillingCDR.SupplierId,
                                    record.BillingCDR.Attempt,
                                    record.BillingCDR.DurationInSeconds,
                                    record.BillingCDR.Alert,
                                    record.BillingCDR.Connect,
                                    record.BillingCDR.Disconnect,
                                    record.BillingCDR.PortOut,
                                    record.BillingCDR.PortIn,
                                    record.BillingCDR.SaleCode,
                                    record.BillingCDR.SaleZoneID,
                                    record.BillingCDR.SupplierCode,
                                    record.BillingCDR.SupplierZoneID,
                                    record.BillingCDR.CDPN,
                                    record.BillingCDR.CGPN,
                                    record.BillingCDR.ReleaseCode,
                                    record.BillingCDR.ReleaseSource);

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
