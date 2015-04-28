using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MYSQL
{
    public class NumberProfileDataManager : BaseMySQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void LoadNumberProfile(DateTime from, DateTime to, int? batchSize, Action<List<Vanrise.Fzero.FraudAnalysis.Entities.NumberProfile>> onBatchReady)
        {
            //ExecuteReaderText(query_GetCDRRange, (reader) =>
            //{
            //    List<TABS.CDR> cdrBatch = new List<NumberProfile>();

            //    while (reader.Read())
            //    {
            //        TABS.CDR cdr = new TABS.CDR
            //        {
            //            CDRID = (long)reader["CDRID"],
            //            Switch = TABS.Switch.All[(byte)reader["SwitchId"]],
            //            IDonSwitch = (long)reader["IDonSwitch"],
            //            Tag = reader["Tag"] as string,
            //            AttemptDateTime = GetReaderValue<DateTime>(reader, "AttemptDateTime"),
            //            AlertDateTime = GetReaderValue<DateTime>(reader, "AlertDateTime"),
            //            ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime"),
            //            DisconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime"),
            //            DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
            //            IN_TRUNK = reader["IN_TRUNK"] as string,
            //            IN_CIRCUIT = reader["IN_CIRCUIT"] != DBNull.Value ? short.Parse(reader["IN_CIRCUIT"].ToString()) : 0,// (short)GetReaderValue<Int64>(reader, "IN_CIRCUIT"),
            //            IN_CARRIER = reader["IN_CARRIER"] as string,
            //            IN_IP = reader["IN_IP"] as string,
            //            OUT_TRUNK = reader["OUT_TRUNK"] as string,
            //            OUT_CIRCUIT = reader["OUT_CIRCUIT"] != DBNull.Value ? int.Parse(reader["OUT_CIRCUIT"].ToString()) : 0, // GetReaderValue<Int16>(reader, "OUT_CIRCUIT"),
            //            OUT_CARRIER = reader["OUT_CARRIER"] as string,
            //            OUT_IP = reader["OUT_IP"] as string,
            //            CGPN = reader["CGPN"] as string,
            //            CDPN = reader["CDPN"] as string,
            //            CDPNOut = reader["CDPNOut"] as string,
            //            CAUSE_FROM = reader["CAUSE_FROM"] as string,
            //            CAUSE_FROM_RELEASE_CODE = reader["CAUSE_FROM_RELEASE_CODE"] as string,
            //            CAUSE_TO = reader["CAUSE_TO"] as string,
            //            CAUSE_TO_RELEASE_CODE = reader["CAUSE_TO_RELEASE_CODE"] as string,
            //            Extra_Fields = reader["Extra_Fields"] as string,
            //            IsRerouted = (reader["IsRerouted"] as string) == "Y"
            //        };
            //        cdrBatch.Add(cdr);
            //        if (batchSize.HasValue && cdrBatch.Count == batchSize)
            //        {
            //            onBatchReady(cdrBatch);
            //            cdrBatch = new List<TABS.CDR>();
            //        }
            //    }
            //    if (cdrBatch.Count > 0)
            //        onBatchReady(cdrBatch);
            //},
            //    (cmd) =>
            //    {
            //        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@From", from));
            //        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@To", to));
            //    });
        }

    }
}
