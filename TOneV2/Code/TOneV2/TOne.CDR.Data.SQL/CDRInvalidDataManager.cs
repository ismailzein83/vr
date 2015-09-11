using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Entities;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class CDRInvalidDataManager : BaseTOneDataManager, ICDRInvalidDataManager
    {

        public object FinishDBApplyStream(object dbApplyStream)
        {

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "Billing_CDR_Invalid",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }
        public void ReserverIdRange(bool isMain, bool isNegative, int numberOfIds, out long id)
        {
            id = (long)ExecuteScalarSP("CDR.sp_CDRIDManager_ReserveIDRange", isMain, isNegative, numberOfIds);
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.BillingCDRInvalid record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}^{25}^{26}^{27}",
                                     record.ID,
                                     record.Attempt.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                     record.Alert.HasValue ? record.Alert.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                                     record.Connect.HasValue ? record.Connect.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                                     record.Disconnect.HasValue ? record.Disconnect.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "",
                                     record.DurationInSeconds,
                                     record.CustomerID,
                                     record.OurZoneID,
                                     record.SupplierID,
                                     record.SupplierZoneID,
                                     record.CDPN,
                                     record.CGPN,
                                     record.ReleaseCode,
                                     record.ReleaseSource,
                                     record.SwitchID,
                                     record.SwitchCdrID,
                                     record.Tag,
                                     record.OriginatingZoneID,
                                     record.Extra_Fields,
                                     record.IsRerouted ? 'Y' : 'N',
                                     record.Port_IN,
                                     record.Port_OUT,
                                     record.OurCode,
                                     record.SupplierCode,
                                     record.CDPNOut,
                                     record.SubscriberID,
                                     record.SIP,
                                     0);
         
        }

        public void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs)
        {
            InsertBulkToTable(preparedInvalidCDRs as BaseBulkInsertInfo);
        }
        public void SaveInvalidCDRsToDB(List<BillingCDRInvalid> cdrsInvalid)
        {
           Object dbApplyStream= InitialiazeStreamForDBApply();
           foreach (BillingCDRInvalid cdrInvalid in cdrsInvalid)
               WriteRecordToStream(cdrInvalid, dbApplyStream);
           Object preparedInvalidCDRs =FinishDBApplyStream(dbApplyStream);
           ApplyInvalidCDRsToDB(preparedInvalidCDRs);
        }
    }
}
