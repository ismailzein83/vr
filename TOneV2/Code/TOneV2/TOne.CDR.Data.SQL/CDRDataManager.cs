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
        public void ApplyCDRsToDB(Object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as StreamBulkInsertInfo);
        }

        public Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs, int switchId)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (var cdr in cdrs)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}^{25}^{26}",
                    cdr.CDRID,
                    cdr.Switch == null ? switchId.ToString() : cdr.Switch.SwitchID.ToString(),
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
                    cdr.SIP);
            }

            stream.Close();

            return new StreamBulkInsertInfo
            {
                TableName = "CDR",
                Stream = stream,
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
                            AlertDateTime = reader["AlertDateTime"] != DBNull.Value ? GetReaderValue<DateTime>(reader, "AlertDateTime") : (DateTime?)null,
                            ConnectDateTime = reader["ConnectDateTime"] != DBNull.Value ? GetReaderValue<DateTime>(reader, "ConnectDateTime") : (DateTime?)null,
                            DisconnectDateTime = reader["DisconnectDateTime"] != DBNull.Value ? GetReaderValue<DateTime>(reader, "DisconnectDateTime") : (DateTime?)null,
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
