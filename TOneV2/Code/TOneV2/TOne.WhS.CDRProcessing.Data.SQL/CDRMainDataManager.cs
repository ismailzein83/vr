using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
    public class CDRMainDataManager : BaseSQLDataManager, ICDRMainDataManager
    {
        public CDRMainDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(BillingMainCDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}",
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
                                    record.MainCost.RateValue,
                                    record.MainCost.TotalNet,
                                    record.MainCost.CurrencyId);

       }
       public object FinishDBApplyStream(object dbApplyStream)
       {

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               TableName = "[TOneWhS_CDR].[CDRMain]",
               Stream = streamForBulkInsert,
               TabLock = false,
               KeepIdentity = false,
               FieldSeparator = '^',
           };
       }
       public void SaveMainCDRBatchToDB(CDRMainBatch cdrBatch)
       {
           Object dbApplyStream = InitialiazeStreamForDBApply();
           foreach (BillingMainCDR cdr in cdrBatch.MainCDRs)
               WriteRecordToStream(cdr, dbApplyStream);
           Object preparedMainCDRs = FinishDBApplyStream(dbApplyStream);
           ApplyMainCDRsToDB(preparedMainCDRs);
       }

       public void ApplyMainCDRsToDB(Object cdrMainBatch)
       {
           InsertBulkToTable(cdrMainBatch as StreamBulkInsertInfo);
       }
    }
}
