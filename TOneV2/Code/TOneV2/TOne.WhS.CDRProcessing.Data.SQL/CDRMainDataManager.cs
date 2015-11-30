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
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
     
       public object InitialiazeStreamForDBApply()
       {
           return base.InitializeStreamForBulkInsert();
       }
       public void WriteRecordToStream(BillingMainCDR record, object dbApplyStream)
       {
           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^",
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
                                    record.Cost.RateValue,
                                    record.Cost.TotalNet,
                                    record.Cost.CurrencyId,
                                    record.Sale.RateValue,
                                    record.Sale.TotalNet,
                                    record.Sale.CurrencyId,
                                    record.BillingCDR.ReleaseCode,
                                    record.BillingCDR.ReleaseSource);

       }
       public object FinishDBApplyStream(object dbApplyStream)
       {
           List<string> columns = new List<string>();
           columns.Add("ID");
           columns.Add("CustomerID");
           columns.Add("SupplierID");
           columns.Add("Attempt");
           columns.Add("DurationInSeconds");
           columns.Add("Alert");
           columns.Add("Connect");
           columns.Add("Disconnect");
           columns.Add("PortOut");
           columns.Add("PortIn");
           columns.Add("SaleCode");
           columns.Add("SaleZoneID");
           columns.Add("SupplierCode");
           columns.Add("SupplierZoneID");
           columns.Add("CDPN");
           columns.Add("CGPN");
           columns.Add("CostRateValue");
           columns.Add("CostTotalNet");
           columns.Add("CostCurrencyID");
           columns.Add("SaleRateValue");
           columns.Add("SaleTotalNet");
           columns.Add("SaleCurrencyID");
           columns.Add("ReleaseCode");
           columns.Add("ReleaseSource");

           StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
           streamForBulkInsert.Close();
           return new StreamBulkInsertInfo
           {
               ColumnNames=columns,
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
