using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Demo.Module.Entities;

namespace Demo.Module.Data.SQL
{
    public class RawCDRDataManager : BaseSQLDataManager, IRawCDRDataManager
    {
        #region ctor/Local Variables
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static RawCDRDataManager()
        {
           
            _columnMapper.Add("Entity.Attempt", "Attempt");
            _columnMapper.Add("Entity.Alert", "Alert");
            _columnMapper.Add("Entity.Connect", "Connect");
            _columnMapper.Add("Entity.Disconnect", "Disconnect");
            _columnMapper.Add("Entity.DurationInSeconds", "Duration");
            _columnMapper.Add("Entity.InTrunk", "InTrunk");
            _columnMapper.Add("Entity.PortIn", "PortIn");
            _columnMapper.Add("Entity.OutTrunk", "OutTrunk");
            _columnMapper.Add("Entity.OutCarrier", "OutCarrier");
            _columnMapper.Add("Entity.PortOut", "PortOut");
            _columnMapper.Add("Entity.CGPN", "CGPN");
            _columnMapper.Add("Entity.CDPN", "CDPN");
            _columnMapper.Add("DataSourceName", "DataSourceID");
            _columnMapper.Add("DirectionDescription", "Direction");
            _columnMapper.Add("ServiceTypeDescription", "ServiceTypeID");
            _columnMapper.Add("CDRTypeDescription", "CDRType");

            
        }
        public RawCDRDataManager() :  base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {}
        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromData", input.Query.FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                    cmd.Parameters.Add(new SqlParameter("@TopRecords", input.Query.NumberRecords));

                });
            };

            return RetrieveData(input, createTempTableAction, RawCDRLogDataMapper, _columnMapper);
        }
        #endregion

        #region Private Methods
        private string CreateTempTableIfNotExists(string tempTableName, RawCDRQuery query)
        {
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN
                                    SELECT TOP (@TopRecords) 
	                                        DATEADD(ms,-datepart(ms,Attempt),Attempt) as Attempt,
	                                        DATEADD(ms,-datepart(ms,Alert),Alert) as Alert,
	                                        DATEADD(ms,-datepart(ms,Connect),Connect) as Connect,
	                                        DATEADD(ms,-datepart(ms,Disconnect),Disconnect) as Disconnect,
	                                        #Duration# as Duration,
	                                        InTrunk,
	                                        PortIn,
	                                        OutTrunk,
	                                        PortOut,
	                                        CGPN,
	                                        CDPN,
                                            Direction,
                                            ServiceTypeID,
                                            CDRType,
                                            DataSourceID
                                            INTO  #TEMPTABLE#
                                            FROM   dbo.CDR C WITH(
                                            NOLOCK,
                                            INDEX(IX_CDR_Attempt) )
                                            Where Attempt BETWEEN @FromData AND @ToDate
                                            #WHEREPART#
                                            ORDER BY Attempt DESC
                                         END
                                ");

            string duration= "CONVERT(DECIMAL(16,4), DurationInSeconds)";
                   queryBuilder.Replace("#Duration#", duration);

            AddFilter<int>(whereBuilder, query.DataSourceIds, "C.DataSourceID");

            AddFilter<int>(whereBuilder, query.Directions, "C.Direction");

            AddFilter<int>(whereBuilder, query.ServiceTypes, "C.ServiceTypeID");

            AddFilter<int>(whereBuilder, query.CDRTypes, "C.CDRType");


            if (!string.IsNullOrEmpty(query.CDPN))
                whereBuilder.AppendFormat(@" AND CDPN LIKE '{0}'", query.CDPN);
            
            if (!string.IsNullOrEmpty(query.CGPN))
                whereBuilder.AppendFormat(@" AND (CGPN LIKE {0})","'" + query.CGPN + "'");

            if (query.MinDuration != null)
                whereBuilder.AppendFormat(@" AND ( {1} IS NULL  OR {0} >= {1} ) ", duration, query.MinDuration);

            if (query.MaxDuration != null)
                whereBuilder.AppendFormat(@" AND ({1} IS NULL OR {0} <= {1}) ", duration, query.MaxDuration);

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
            rawCDRLog.Alert = GetReaderValue<DateTime>(reader, "Alert");
            rawCDRLog.Attempt = GetReaderValue<DateTime>(reader, "Attempt");
            rawCDRLog.CDPN = reader["CDPN"] as string;
            rawCDRLog.CGPN = reader["CGPN"] as string;
            rawCDRLog.Connect = GetReaderValue<DateTime>(reader, "Connect");
            rawCDRLog.Disconnect = GetReaderValue<DateTime>(reader, "Disconnect");
            rawCDRLog.DurationInSeconds = GetReaderValue<Decimal>(reader, "Duration");
            rawCDRLog.PortIn = reader["PortIn"] as string;
            rawCDRLog.InTrunk = reader["InTrunk"] as string;
            rawCDRLog.PortOut = reader["PortOut"] as string;
            rawCDRLog.OutTrunk = reader["OutTrunk"] as string;
            rawCDRLog.DirectionType = GetReaderValue<Direction>(reader, "Direction");
            rawCDRLog.CDRType = GetReaderValue<Demo.Module.Entities.Type>(reader, "CDRType"); 
            rawCDRLog.DataSourceId = GetReaderValue<int>(reader, "DataSourceID");
            rawCDRLog.ServiceTypeId = GetReaderValue<int>(reader, "ServiceTypeID");

        }
        #endregion
    }
}
