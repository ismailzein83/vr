using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class RawCDRDataManager : BaseTOneDataManager, IRawCDRDataManager
    {
        #region ctor/Local Variables
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static RawCDRDataManager()
        {
            _columnMapper.Add("Entity.ID", "ID");
        }
        public RawCDRDataManager() : base(GetConnectionStringName("TOneWhS_CDR_DBConnStringKey", "TOneWhS_CDR_DBConnString"))
        {}
        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromData", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                    cmd.Parameters.Add(new SqlParameter("@TopRecords", input.Query.NRecords));

                });
            };

            return RetrieveData(input, createTempTableAction, RawCDRLogDataMapper, _columnMapper);
        }
        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName, RawCDRInput query)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN
                                    SELECT TOP (@TopRecords) ID,
	                                        SwitchID,
	                                        DATEADD(ms,-datepart(ms,Attempt),Attempt) as Attempt,
	                                        DATEADD(ms,-datepart(ms,Alert),Alert) as Alert,
	                                        DATEADD(ms,-datepart(ms,Connect),Connect) as Connect,
	                                        DATEADD(ms,-datepart(ms,Disconnect),Disconnect) as Disconnect,
	                                        #Duration# as Duration,
	                                        InTrunk,
	                                        InCarrier,
	                                        PortIn,
	                                        OutTrunk,
	                                        OutCarrier,
	                                        PortOut,
	                                        CGPN,
	                                        CDPN
                                            INTO  #TEMPTABLE#
                                            FROM   TOneWhS_CDR.CDR C WITH(
                                            NOLOCK,
                                            INDEX(IX_CDR_Attempt) )
                                            Where Attempt BETWEEN @FromData AND @ToDate
                                            #WHEREPART#
                                            ORDER BY Attempt DESC
                                         END
                                ");

            string duration="";
            if (query.DurationType == Duration.Sec)
            {
                duration="CONVERT(DECIMAL(16,4), DurationInSeconds)";
                queryBuilder.Replace("#Duration#", duration);
            }   
            else if (query.DurationType == Duration.Min)
            {
                duration = "CONVERT(DECIMAL(16,4), DurationInSeconds/60)";
                queryBuilder.Replace("#Duration#", duration);
            }
               

            if (!string.IsNullOrEmpty(query.InCarrier))
                whereBuilder.Append("AND (InCarrier Like '%" + query.InCarrier + "%')");
            
            if (!string.IsNullOrEmpty(query.OutCarrier))
                whereBuilder.Append("AND (OutCarrier Like '%" + query.OutCarrier + "%')");

            AddFilter<string>(whereBuilder, query.Switches, "C.SwitchID");

            if (!string.IsNullOrEmpty(query.InCDPN))
                whereBuilder.AppendFormat(@" AND CDPN LIKE '{0}'", query.InCDPN);
            //else
            //    whereBuilder.AppendFormat(@" AND CDPNOut LIKE '{0}'", query.OutCDPN);
            if (!string.IsNullOrEmpty(query.WhereCondition))
                whereBuilder.AppendFormat(@"AND {0}", query.WhereCondition);
            if (!string.IsNullOrEmpty(query.CGPN))
                whereBuilder.AppendFormat(@" AND (CGPN LIKE {0})","'" + query.CGPN + "'");

            if (query.MinDuration != null)
                whereBuilder.AppendFormat(@" AND ( {1} IS NULL  OR {0} >= {1} ) ", duration , query.MinDuration);

            if (query.MaxDuration != null)
                whereBuilder.AppendFormat(@" AND ({1} IS NULL OR {0} <= {1}) ",duration, query.MaxDuration);

            queryBuilder.Replace("#TEMPTABLE#", tempTableName);

            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());
            
            return queryBuilder.ToString();

        }
        private void AddFilter<T>(StringBuilder whereBuilder, IEnumerable<T> values, string column)
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
        private void FillRawCDRDataFromReader(RawCDRLog rawCDRLog, IDataReader reader)
        {
            rawCDRLog.ID = GetReaderValue<long>(reader, "ID");
            rawCDRLog.Alert = GetReaderValue<DateTime>(reader, "Alert");
            rawCDRLog.Attempt = GetReaderValue<DateTime>(reader, "Attempt");
          //  rawCDRLog.CAUSE_FROM = reader["CAUSE_FROM"] as string;
          //  rawCDRLog.CAUSE_FROM_RELEASE_CODE = reader["CAUSE_FROM_RELEASE_CODE"] as string;
          //  rawCDRLog.CAUSE_TO = reader["CAUSE_TO"] as string;
          //  rawCDRLog.CAUSE_TO_RELEASE_CODE = reader["CAUSE_TO_RELEASE_CODE"] as string;
            rawCDRLog.CDPN = reader["CDPN"] as string;
          //  rawCDRLog.CDPNOut = reader["CDPNOut"] as string;
            rawCDRLog.CGPN = reader["CGPN"] as string;
            rawCDRLog.Connect = GetReaderValue<DateTime>(reader, "Connect");
            rawCDRLog.Disconnect = GetReaderValue<DateTime>(reader, "Disconnect");
            rawCDRLog.DurationInSeconds = GetReaderValue<Decimal>(reader, "Duration");
          //  rawCDRLog.Extra_Fields = reader["Extra_Fields"] as string;
          //  rawCDRLog.IDonSwitch = GetReaderValue<Int64>(reader, "IDonSwitch");
            rawCDRLog.InCarrier = reader["InCarrier"] as string;
          //  rawCDRLog.IN_CIRCUIT = GetReaderValue<Int64>(reader, "IN_CIRCUIT");
            rawCDRLog.PortIn = reader["PortIn"] as string;
            rawCDRLog.InTrunk = reader["InTrunk"] as string;
          //  rawCDRLog.IsRerouted = GetReaderValue<string>(reader, "IsRerouted");
            rawCDRLog.OutCarrier = reader["OutCarrier"] as string;
          //  rawCDRLog.OUT_CIRCUIT = GetReaderValue<Int16>(reader, "OUT_CIRCUIT");
            rawCDRLog.PortOut = reader["PortOut"] as string;
            rawCDRLog.OutTrunk = reader["OutTrunk"] as string;
            rawCDRLog.SwitchID = GetReaderValue<int>(reader, "SwitchID");
          //  rawCDRLog.Tag = reader["Tag"] as string;

        }
        #endregion
    }
}
