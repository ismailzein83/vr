using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.CDR.Data.SQL
{
    public class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {

        public Object PrepareMainCDRsForDBApply(List<TOne.CDR.Entities.BillingCDRMain> cdrs)
        {
            BulkInsertInfo preparedMainCDRs = PrepareMainCDRsForDBApply2(cdrs);

            string filePathCost = GetFilePathForBulkInsert();
            string filePathSale = GetFilePathForBulkInsert();

            System.IO.StreamWriter wrCost = new System.IO.StreamWriter(filePathCost);
            System.IO.StreamWriter wrSale = new System.IO.StreamWriter(filePathSale);

            foreach (TOne.CDR.Entities.BillingCDRMain cdr in cdrs)
            {

                if (cdr.cost != null)
                {
                    wrCost.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                            cdr.cost.ID,
                            cdr.cost.ZoneID.ToString(),
                            cdr.cost.Net,
                            cdr.cost.CurrencySymbol != null ? cdr.cost.CurrencySymbol.ToString() : "",
                            cdr.cost.RateValue,
                            cdr.cost.RateID.ToString() ,
                            cdr.cost.Discount.HasValue ? cdr.cost.Discount.Value.ToString() : null,
                            Convert.ChangeType(cdr.cost.RateType, cdr.cost.RateType.GetTypeCode()),
                            cdr.cost.ToDConsiderationID.ToString(),
                            cdr.cost.FirstPeriod.HasValue ? cdr.cost.FirstPeriod.Value.ToString() : null,
                            cdr.cost.RepeatFirstperiod.HasValue ? cdr.cost.RepeatFirstperiod.Value.ToString() : null,
                            cdr.cost.FractionUnit.HasValue ? cdr.cost.RepeatFirstperiod.Value.ToString() : null,
                            cdr.cost.TariffID.ToString(),
                            cdr.cost.CommissionValue,
                            cdr.cost.CommissionID.ToString(),
                            cdr.cost.ExtraChargeValue,
                            cdr.cost.ExtraChargeID.ToString() ,
                            cdr.cost.Updated.ToShortDateString(),
                            cdr.cost.DurationInSeconds,
                            cdr.cost.Code,
                            cdr.cost.Attempt.ToString()));
                }

                if (cdr.sale != null)
                {
                    wrSale.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                        cdr.sale.ID,
                        cdr.sale.ZoneID.ToString(),
                        cdr.sale.Net,
                        cdr.sale.CurrencySymbol != null ? cdr.sale.CurrencySymbol : "",
                        cdr.sale.RateValue,
                        cdr.sale.RateID.ToString() ,
                        cdr.sale.Discount.HasValue ? cdr.sale.Discount.Value.ToString() : null,
                        Convert.ChangeType(cdr.sale.RateType, cdr.sale.RateType.GetTypeCode()),
                        cdr.sale.ToDConsiderationID.ToString(),
                        cdr.sale.FirstPeriod.HasValue ? cdr.sale.FirstPeriod.Value.ToString() : null,
                        cdr.sale.RepeatFirstperiod.HasValue ? cdr.sale.RepeatFirstperiod.Value.ToString() : null,
                        cdr.sale.FractionUnit.HasValue ? cdr.sale.FractionUnit.Value.ToString() : null,
                        cdr.sale.TariffID.ToString(),
                        cdr.sale.CommissionValue,
                        cdr.sale.CommissionID.ToString(),
                        cdr.sale.ExtraChargeValue,
                        cdr.sale.ExtraChargeID.ToString(),
                        cdr.sale.Updated.ToShortDateString(),
                        cdr.sale.DurationInSeconds,
                        cdr.sale.Code,
                        cdr.sale.Attempt.ToString()));
                }
            }

            wrCost.Close();
            wrSale.Close();

            BulkInsertInfo preparedCostCDRs = new BulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Cost]",
                DataFilePath = filePathCost,
                TabLock = false,
                FieldSeparator = '^'
            };

            BulkInsertInfo preparedSaleCDRs = new BulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Sale]",
                DataFilePath = filePathSale,
                TabLock = false,
                FieldSeparator = '^'
            };

            List<BulkInsertInfo> preparedCDRs = new List<BulkInsertInfo>();
            preparedCDRs.Add(preparedMainCDRs);
            preparedCDRs.Add(preparedCostCDRs);
            preparedCDRs.Add(preparedSaleCDRs);

            return preparedCDRs;
        }

        private BulkInsertInfo PrepareMainCDRsForDBApply2(List<TOne.CDR.Entities.BillingCDRMain> cdrs)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25}",
                        cdr.ID,
                        cdr.Attempt.ToString(),
                        cdr.Alert.HasValue ? cdr.Alert.Value.ToString() : "",
                        cdr.Connect.HasValue ? cdr.Connect.Value.ToString() : "",
                        cdr.Disconnect.HasValue ? cdr.Disconnect.Value.ToString() : "",
                        cdr.DurationInSeconds,
                        cdr.CustomerID,
                        cdr.OurZoneID == default(int) ? "" : cdr.OurZoneID.ToString(),
                        cdr.OriginatingZoneID == default(int) ? "" : cdr.OriginatingZoneID.ToString(),
                        cdr.SupplierID,
                        cdr.SupplierZoneID == default(int) ? "" : cdr.SupplierZoneID.ToString(),
                        cdr.CDPN,
                        cdr.CGPN,
                        cdr.ReleaseCode,
                        cdr.ReleaseSource,
                        cdr.SwitchID.ToString() ,
                        cdr.SwitchCdrID,
                        cdr.Tag,
                        cdr.Extra_Fields,
                        cdr.Port_IN,
                        cdr.Port_OUT,
                        cdr.OurCode,
                        cdr.SupplierCode,
                        cdr.CDPNOut,
                        cdr.SubscriberID,
                        cdr.SIP));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "[dbo].[Billing_CDR_Main]",
                DataFilePath = filePath,
                TabLock = false,
                FieldSeparator = '^'
            };
        }

        public void ApplyMainCDRsToDB(Object preparedMainCDRs)
        {
            List<BulkInsertInfo> listPreparedCDRs = (List<BulkInsertInfo>)preparedMainCDRs;

            Parallel.ForEach(listPreparedCDRs, item =>
            {
                InsertBulkToTable(item);
            });
    
        }

        public void ApplyInvalidCDRsToDB(Object preparedInvalidCDRs)
        {
            InsertBulkToTable(preparedInvalidCDRs as BulkInsertInfo);
        }

        public void ApplyCDRsToDB(Object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BulkInsertInfo);
        }

        public Object PrepareInvalidCDRsForDBApply(List<TOne.CDR.Entities.BillingCDRInvalid> cdrs)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}",
                        cdr.ID,
                        cdr.Attempt.ToString(),
                        cdr.Alert.HasValue ? cdr.Alert.Value.ToString() : "",
                        cdr.Connect.HasValue ? cdr.Connect.Value.ToString() : "",
                        cdr.Disconnect.HasValue ? cdr.Disconnect.Value.ToString() : "",
                        cdr.DurationInSeconds,
                        cdr.CustomerID,
                        cdr.OurZoneID,
                        cdr.SupplierID,
                        cdr.SupplierZoneID,
                        cdr.CDPN,
                        cdr.CGPN,
                        cdr.ReleaseCode,
                        cdr.ReleaseSource,
                        cdr.SwitchID,
                        cdr.SwitchCdrID,
                        cdr.Tag,
                        cdr.OriginatingZoneID,
                        cdr.Extra_Fields,
                        cdr.IsRerouted,
                        cdr.Port_IN,
                        cdr.Port_OUT,
                        cdr.OurCode,
                        cdr.SupplierCode,
                        cdr.CDPNOut,
                        cdr.SubscriberID,
                        cdr.SIP));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "Billing_CDR_Invalid",
                DataFilePath = filePath,
                TabLock = false,
                FieldSeparator = '^'
            };
        }

        public Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs, int SwitchId)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cdr in cdrs)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}", 
                        cdr.CDRID,
                        cdr.Switch == null ? SwitchId.ToString() : cdr.Switch.SwitchID.ToString(), 
                        cdr.IDonSwitch,
                        cdr.Tag, 
                        cdr.AttemptDateTime, 
                        cdr.AlertDateTime.HasValue ? cdr.AlertDateTime.Value.ToString() : "", 
                        cdr.ConnectDateTime.HasValue ? cdr.ConnectDateTime.Value.ToString() : "", 
                        cdr.DisconnectDateTime.HasValue ? cdr.DisconnectDateTime.Value.ToString() : "", 
                        cdr.DurationInSeconds, 
                        cdr.IN_TRUNK, 
                        cdr.IN_CIRCUIT, 
                        cdr.IN_CARRIER, 
                        cdr.IN_IP, 
                        cdr.OUT_TRUNK, 
                        cdr.OUT_CIRCUIT, 
                        cdr.OUT_CARRIER, 
                        cdr.OUT_IP, 
                        cdr.CGPN, 
                        cdr.CDPN, 
                        cdr.CAUSE_FROM_RELEASE_CODE, 
                        cdr.CAUSE_FROM, 
                        cdr.CAUSE_TO_RELEASE_CODE, 
                        cdr.CAUSE_TO, 
                        cdr.Extra_Fields, 
                        cdr.IsRerouted ? 'Y' : 'N', 
                        cdr.CDPNOut, 
                        cdr.SIP));
                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "CDR",
                DataFilePath = filePath,
                TabLock = false,
                FieldSeparator = '^'
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
                            Switch = TABS.Switch.All[(byte)reader["SwitchId"]],
                            IDonSwitch = (long)reader["IDonSwitch"],
                            Tag = reader["Tag"] as string,
                            AttemptDateTime = GetReaderValue<DateTime>(reader, "AttemptDateTime"),
                            AlertDateTime = GetReaderValue<DateTime>(reader, "AlertDateTime"),
                            ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime"),
                            DisconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime"),
                            DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                            IN_TRUNK = reader["IN_TRUNK"] as string,
                            IN_CIRCUIT = reader["IN_CIRCUIT"] != DBNull.Value ? short.Parse(reader["IN_CIRCUIT"].ToString()) : 0,// (short)GetReaderValue<Int64>(reader, "IN_CIRCUIT"),
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
