using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CDRProcessing.Data.SQL
{
    public class CDRInvalidDataManager : BaseSQLDataManager, ICDRInvalidDataManager
    {
        
        #region ctor/Local Variables
        public CDRInvalidDataManager()
            : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {

        }
        readonly string[] columns = { "ID", "CustomerID", "SupplierID", "Attempt", "DurationInSeconds", "Alert", "Connect", "Disconnect", "PortOut", "PortIn", "SaleCode", "SaleZoneID", "SupplierCode", "SupplierZoneID", "CDPN", "CGPN", "ReleaseCode", "ReleaseSource" };

        #endregion

        #region Public Methods
        public void SaveInvalidCDRBatchToDB(CDRInvalidBatch cdrBatch)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (BillingInvalidCDR cdr in cdrBatch.InvalidCDRs)
                WriteRecordToStream(cdr, dbApplyStream);
            Object preparedInvalidCDRs = FinishDBApplyStream(dbApplyStream);
            ApplyInvalidCDRsToDB(preparedInvalidCDRs);
        }
      
        #endregion

        #region Private Methods
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private void WriteRecordToStream(BillingInvalidCDR record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^",
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
        private object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                ColumnNames = columns,
                TableName = "[TOneWhS_CDR].[CDRInvalid]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = true,
                FieldSeparator = '^',
            };
        }
        private void ApplyInvalidCDRsToDB(Object cdrInvalidBatch)
        {
            InsertBulkToTable(cdrInvalidBatch as StreamBulkInsertInfo);
        }
      
        #endregion

    }
}
