using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {

        public Object PrepareMainCDRsForDBApply(List<TABS.Billing_CDR_Main> cdrs)
        {
            BulkInsertInfo preparedMainCDRs = PrepareMainCDRsForDBApply2(cdrs);

            string filePathCost = GetFilePathForBulkInsert();
            string filePathSale = GetFilePathForBulkInsert();
            
            System.IO.StreamWriter wrCost= new System.IO.StreamWriter(filePathCost);
            System.IO.StreamWriter wrSale = new System.IO.StreamWriter(filePathSale);

            foreach (TABS.Billing_CDR_Main cdr in cdrs)
            {

                if (cdr.Billing_CDR_Cost != null)
                {
                    wrCost.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}", cdr.Billing_CDR_Cost.ID, 
                            cdr.Billing_CDR_Cost.Zone.ZoneID, cdr.Billing_CDR_Cost.Net, cdr.Billing_CDR_Cost.Currency.Symbol, cdr.Billing_CDR_Cost.RateValue, cdr.Billing_CDR_Cost.Rate.ID,
                            cdr.Billing_CDR_Cost.Discount, cdr.Billing_CDR_Cost.RateType, cdr.Billing_CDR_Cost.ToDConsideration.ToDConsiderationID, cdr.Billing_CDR_Cost.FirstPeriod,
                            cdr.Billing_CDR_Cost.RepeatFirstperiod, cdr.Billing_CDR_Cost.FractionUnit, cdr.Billing_CDR_Cost.Tariff.TariffID, cdr.Billing_CDR_Cost.CommissionValue,
                            cdr.Billing_CDR_Cost.Commission.CommissionID, cdr.Billing_CDR_Cost.ExtraChargeValue, cdr.Billing_CDR_Cost.ExtraCharge.CommissionID,
                            cdr.Billing_CDR_Cost.Updated, cdr.Billing_CDR_Cost.DurationInSeconds, cdr.Billing_CDR_Cost.Code, cdr.Billing_CDR_Cost.Attempt));
                }

                if (cdr.Billing_CDR_Sale != null)
                {
                    wrSale.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                        cdr.Billing_CDR_Sale.ID, cdr.Billing_CDR_Sale.Zone.ZoneID, cdr.Billing_CDR_Sale.Net, cdr.Billing_CDR_Sale.Currency.Symbol, cdr.Billing_CDR_Sale.RateValue,
                        cdr.Billing_CDR_Sale.Rate.ID, cdr.Billing_CDR_Sale.Discount, cdr.Billing_CDR_Sale.RateType, cdr.Billing_CDR_Sale.ToDConsideration.ToDConsiderationID, 
                        cdr.Billing_CDR_Sale.FirstPeriod, cdr.Billing_CDR_Sale.RepeatFirstperiod, cdr.Billing_CDR_Sale.FractionUnit, cdr.Billing_CDR_Sale.Tariff.TariffID, 
                        cdr.Billing_CDR_Sale.CommissionValue, cdr.Billing_CDR_Sale.Commission.CommissionID, cdr.Billing_CDR_Sale.ExtraChargeValue,
                        cdr.Billing_CDR_Sale.ExtraCharge.CommissionID, cdr.Billing_CDR_Sale.Updated, cdr.Billing_CDR_Sale.DurationInSeconds, cdr.Billing_CDR_Sale.Code, cdr.Billing_CDR_Sale.Attempt));
                }
            }

            wrCost.Close();
            wrSale.Close();
            
            BulkInsertInfo preparedCostCDRs = new BulkInsertInfo
            {
                TableName = "Billing_CDR_Cost",
                DataFilePath = filePathCost,
                TabLock = true,
                FieldSeparator = ','
            };

            BulkInsertInfo preparedSaleCDRs = new BulkInsertInfo
            {
                TableName = "Billing_CDR_Sale",
                DataFilePath = filePathSale,
                TabLock = true,
                FieldSeparator = ','
            };

            List<BulkInsertInfo> preparedCDRs = new List<BulkInsertInfo>();
            preparedCDRs.Add(preparedMainCDRs);
            preparedCDRs.Add(preparedCostCDRs);
            preparedCDRs.Add(preparedSaleCDRs);

            return preparedCDRs;
        }

        private BulkInsertInfo PrepareMainCDRsForDBApply2(List<TABS.Billing_CDR_Main> cdrs)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25}", 
                        cdr.ID, cdr.Attempt,cdr.Alert, cdr.Connect, cdr.Disconnect, cdr.DurationInSeconds, cdr.CustomerID, cdr.OurZone.ZoneID, cdr.OriginatingZone.ZoneID,
                        cdr.SupplierID, cdr.SupplierZone.ZoneID, cdr.CDPN, cdr.CGPN, cdr.ReleaseCode, cdr.ReleaseSource, cdr.Switch.SwitchID, cdr.SwitchCdrID, cdr.Tag, cdr.Extra_Fields, 
                        cdr.Port_IN, cdr.Port_OUT, cdr.OurCode, cdr.SupplierCode, cdr.CDPNOut, cdr.SubscriberID, cdr.SIP));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "Billing_CDR_Main",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = ','
            };
        }

        public void ApplyMainCDRsToDB(Object preparedMainCDRs)
        {
            List<BulkInsertInfo> listPreparedCDRs = (List<BulkInsertInfo>)preparedMainCDRs;

            foreach (BulkInsertInfo item in listPreparedCDRs)
                InsertBulkToTable(item);
        }

        public void ApplyInvalidToDB(Object preparedInvalidCDRs)
        {
            InsertBulkToTable(preparedInvalidCDRs as BulkInsertInfo);
        }

        private Object PrepareInvalidCDRsForDBApply(List<TABS.Billing_CDR_Invalid> cdrs)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}", cdr.ID, 
                        cdr.Attempt, cdr.Alert, cdr.Connect, cdr.Disconnect, cdr.DurationInSeconds, cdr.CustomerID, cdr.OurZone.ZoneID, cdr.SupplierID, cdr.SupplierZone.ZoneID, cdr.CDPN, cdr.CGPN, 
                        cdr.ReleaseCode, cdr.ReleaseSource, cdr.Switch.SwitchID, cdr.SwitchCdrID, cdr.Tag, cdr.OriginatingZone.ZoneID , 
                        cdr.Extra_Fields, cdr.IsRerouted, cdr.Port_IN, cdr.Port_OUT, cdr.OurCode, cdr.SupplierCode, cdr.CDPNOut, cdr.SubscriberID, cdr.SIP));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "Billing_CDR_Invalid",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = ','
            };
        }
        
        public void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<List<TABS.CDR>> onBatchReady)
        {
            ExecuteReaderText(query_GetCDRRange, (reader) =>
                {
                    List<TABS.CDR> cdrBatch = new List<TABS.CDR>();

                    while (reader.Read())
                    {
                        TABS.CDR cdr = new TABS.CDR
                        {
                            CDRID = (long)reader["CDRID"],
                            Switch = TABS.Switch.All[ (byte)reader["SwitchId"]],
                            IDonSwitch = (long)reader["IDonSwitch"],
                            Tag = reader["Tag"] as string,
                            AttemptDateTime = GetReaderValue<DateTime>(reader, "AttemptDateTime"),
                            AlertDateTime = GetReaderValue<DateTime>(reader, "AlertDateTime"),
                            ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime"),
                            DisconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime"),
                            DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                            IN_TRUNK = reader["IN_TRUNK"] as string,
                            IN_CIRCUIT =  reader["IN_CIRCUIT"] != DBNull.Value ? short.Parse(reader["IN_CIRCUIT"].ToString()) : 0,// (short)GetReaderValue<Int64>(reader, "IN_CIRCUIT"),
                            IN_CARRIER = reader["IN_CARRIER"] as string,
                            IN_IP = reader["IN_IP"] as string,
                            OUT_TRUNK = reader["OUT_TRUNK"] as string,
                            OUT_CIRCUIT = reader["OUT_CIRCUIT"] != DBNull.Value ? int.Parse(reader["OUT_CIRCUIT"].ToString()) : 0, // GetReaderValue<Int16>(reader, "OUT_CIRCUIT"),
                            OUT_CARRIER = reader["OUT_CARRIER"] as string,
                            OUT_IP = reader["OUT_IP"] as string,
                            CGPN = reader["CGPN"] as string,
                            CDPN = reader["CDPN"] as string,
                            CDPNOut = reader["CDPNOut"] as string,
                            CAUSE_FROM = reader["CAUSE_FROM"] as string,
                            CAUSE_FROM_RELEASE_CODE = reader["CAUSE_FROM_RELEASE_CODE"] as string,
                            CAUSE_TO = reader["CAUSE_TO"] as string,
                            CAUSE_TO_RELEASE_CODE = reader["CAUSE_TO_RELEASE_CODE"] as string,
                            Extra_Fields = reader["Extra_Fields"] as string,
                            IsRerouted = (reader["IsRerouted"] as string) == "Y"
                        };
                        cdrBatch.Add(cdr);
                        if (batchSize.HasValue && cdrBatch.Count == batchSize)
                        {
                            onBatchReady(cdrBatch);
                            cdrBatch = new List<TABS.CDR>();
                        }
                    }
                    if (cdrBatch.Count > 0)
                        onBatchReady(cdrBatch);
                },
                (cmd) =>
                    {
                        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@From", from));
                        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@To", to));
                    });
        }

        const string query_GetCDRRange = @"SELECT  CDRID,
            SwitchId,
            IDonSwitch,
            Tag,
            AttemptDateTime,
            AlertDateTime,
            ConnectDateTime,
            DisconnectDateTime,
            DurationInSeconds,
            IN_TRUNK,
            IN_CIRCUIT,
            IN_CARRIER,
            IN_IP,
            OUT_TRUNK,
            OUT_CIRCUIT, 
            OUT_CARRIER,
            OUT_IP,
            CGPN,
            CDPN,
            CDPNOut,
            CAUSE_FROM,
            CAUSE_FROM_RELEASE_CODE,
            CAUSE_TO,
            CAUSE_TO_RELEASE_CODE,
            Extra_Fields,
            IsRerouted,
            SIP
     FROM CDR WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) 
     WHERE AttemptDateTime >= @From AND AttemptDateTime < @To ";
    }
}
