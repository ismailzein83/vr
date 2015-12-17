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

        #region ctor/Local Variables
        public CDRMainDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
        readonly string[] columns = { "ID", "CustomerID", "SupplierID", "Attempt", "DurationInSeconds", "Alert", "Connect", "Disconnect", "PortOut", "PortIn", "SaleCode", "SaleZoneID", "SupplierCode", "SupplierZoneID", "CDPN", "CGPN", "CostRateValue", "CostTotalNet", "CostCurrencyID", "SaleRateValue", "SaleTotalNet", "SaleCurrencyID", "ReleaseCode", "ReleaseSource","SwitchID" };

        #endregion

        #region Public Methods
        public void SaveMainCDRBatchToDB(CDRMainBatch cdrBatch)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (BillingMainCDR cdr in cdrBatch.MainCDRs)
                WriteRecordToStream(cdr, dbApplyStream);
            Object preparedMainCDRs = FinishDBApplyStream(dbApplyStream);
            ApplyMainCDRsToDB(preparedMainCDRs);
        }
      
        #endregion

        #region Private Methods
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(BillingMainCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}",
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
                                     record.BillingCDR.ReleaseSource,
                                     record.BillingCDR.SwitchID);

        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = columns,
                TableName = "[TOneWhS_CDR].[CDRMain]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        private void ApplyMainCDRsToDB(Object cdrMainBatch)
        {
            InsertBulkToTable(cdrMainBatch as StreamBulkInsertInfo);
        }
        #endregion

    }
}
