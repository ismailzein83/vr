using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class RawCDRLogDataManager : BaseTOneDataManager, IRawCDRLogDataManager
    {
      public  Vanrise.Entities.BigResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("SwitchName", "SwitchID");
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromData", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
 
                });
            };

            return RetrieveData(input, createTempTableAction, RawCDRLogDataMapper, mapper);
        }

        #region Methods
        private string CreateTempTableIfNotExists(string tempTableName, RawCDRInput query)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN

			                        WITH Switches AS (SELECT [SwitchID],[Name] FROM [Switch])

                                    SELECT CDRID,
	                                        Name as SwitchName,
	                                        IDonSwitch,
	                                        Tag,
	                                        DATEADD(ms,-datepart(ms,AttemptDateTime),AttemptDateTime) as AttemptDateTime,
	                                        DATEADD(ms,-datepart(ms,AlertDateTime),AlertDateTime) as AlertDateTime,
	                                        DATEADD(ms,-datepart(ms,ConnectDateTime),ConnectDateTime) as ConnectDateTime,
	                                        DATEADD(ms,-datepart(ms,DisconnectDateTime),DisconnectDateTime) as DisconnectDateTime,
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
                                            CAUSE_FROM_RELEASE_CODE,
	                                        CAUSE_FROM,
	                                        CAUSE_TO_RELEASE_CODE,
	                                        CAUSE_TO,
	                                        Extra_Fields,
                                            IsRerouted
                                            INTO  #TEMPTABLE#
                                            FROM   CDR C WITH(
                                            NOLOCK,
                                            INDEX(IX_CDR_AttemptDateTime) )

                                      LEFT JOIN Switches S ON (C.SwitchID = S.SwitchID)
                                        Where AttemptDateTime BETWEEN @FromData AND @ToDate
                                            #WHEREPART#

                                        ORDER BY AttemptDateTime DESC
                                         END
                                ");

            if (!string.IsNullOrEmpty(query.InCarrier))
                whereBuilder.Append("AND (IN_CARRIER Like '%" + query.InCarrier + "%')");
            if (!string.IsNullOrEmpty(query.OutCarrier))
                whereBuilder.Append("AND (OUT_CARRIER Like '%" + query.OutCarrier + "%')");
           // if ( query.Switches.Count!=0)
                AddFilter<string>(whereBuilder, query.Switches, "C.SwitchID");
               // whereBuilder.AppendFormat("AND C.SwitchID IN ('{0}')", String.Join("', '", query.Switches));
            if (!string.IsNullOrEmpty(query.InCDPN))
                whereBuilder.AppendFormat(@" AND CDPN LIKE '{0}'", query.InCDPN);
            else
                whereBuilder.AppendFormat(@" AND CDPNOut LIKE '{0}'", query.OutCDPN);
            if (!string.IsNullOrEmpty(query.CGPN))
                whereBuilder.AppendFormat(@" AND (CGPN LIKE {0})","'" + query.CGPN + "'");
             whereBuilder.AppendFormat(@" AND ( {0} IS NULL  OR DurationInSeconds >= {0} )
                           AND ({1} IS NULL OR DurationInSeconds <= {1})", query.MinDuration == null ? "NULL" : query.MinDuration.ToString(), query.MaxDuration == null ? "NULL" : query.MaxDuration.ToString());
        
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);

            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            
            return queryBuilder.ToString();

        }

        void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (typeof(T) == typeof(string))
                    whereBuilder.AppendFormat("AND {0} IN ('{1}')", column, String.Join("', '", values));
                else
                    whereBuilder.AppendFormat("AND {0} IN ({1})", column, String.Join(", ", values));
            }
        }
        private RawCDRLog RawCDRLogDataMapper(IDataReader reader)
        {

            RawCDRLog instance = new RawCDRLog();
            FillRawCDRDataFromReader(instance, reader);
            return instance;
        }
        void FillRawCDRDataFromReader(RawCDRLog rawCDRLog, IDataReader reader)
        {
            rawCDRLog.CDRID = GetReaderValue<Int64>(reader, "CDRID");
            rawCDRLog.AlertDateTime = GetReaderValue<DateTime>(reader, "AlertDateTime");
            rawCDRLog.AttemptDateTime = GetReaderValue<DateTime>(reader, "AttemptDateTime");
            rawCDRLog.CAUSE_FROM = reader["CAUSE_FROM"] as string;
            rawCDRLog.CAUSE_FROM_RELEASE_CODE = reader["CAUSE_FROM_RELEASE_CODE"] as string;
            rawCDRLog.CAUSE_TO = reader["CAUSE_TO"] as string;
            rawCDRLog.CAUSE_TO_RELEASE_CODE = reader["CAUSE_TO_RELEASE_CODE"] as string;
            rawCDRLog.CDPN = reader["CDPN"] as string;
            rawCDRLog.CDPNOut = reader["CDPNOut"] as string;
            rawCDRLog.CGPN = reader["CGPN"] as string;
            rawCDRLog.ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
            rawCDRLog.DisconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime");
            rawCDRLog.DurationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
            rawCDRLog.Extra_Fields = reader["Extra_Fields"] as string;
            rawCDRLog.IDonSwitch = GetReaderValue<Int64>(reader, "IDonSwitch");
            rawCDRLog.IN_CARRIER = reader["IN_CARRIER"] as string;
            rawCDRLog.IN_CIRCUIT = GetReaderValue<Int64>(reader, "IN_CIRCUIT");
            rawCDRLog.IN_IP = reader["IN_IP"] as string;
            rawCDRLog.IN_TRUNK = reader["IN_TRUNK"] as string;
            rawCDRLog.IsRerouted = GetReaderValue<string>(reader, "IsRerouted");
            rawCDRLog.OUT_CARRIER = reader["OUT_CARRIER"] as string;
            rawCDRLog.OUT_CIRCUIT = GetReaderValue<Int16>(reader, "OUT_CIRCUIT");
            rawCDRLog.OUT_IP = reader["OUT_IP"] as string;
            rawCDRLog.OUT_TRUNK = reader["OUT_TRUNK"] as string;
            rawCDRLog.SwitchName = reader["SwitchName"] as string;
            rawCDRLog.Tag = reader["Tag"] as string;

        }
        #endregion
    }
}
