using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Analytics.Entities;
using TOne.Analytics.Data;
using System.Data;
using System.Data.SqlClient;
using TOne.Entities;
namespace TOne.Analytics.Data.SQL
{
    class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {
        public CDRBigResult GetCDRData(string tempTableKey, CDRFilter filter, DateTime fromDate, DateTime toDate, int fromRow, int toRow, int nRecords, string CDROption,BillingCDRMeasures orderBy, bool isDescending)
        {   
            TempTableName tempTableName = null;
            if (tempTableKey != null)
                tempTableName = GetTempTableName(tempTableKey);
            else
                tempTableName = GenerateTempTableName();
            CDRBigResult rslt = new CDRBigResult()
            {
                ResultKey = tempTableName.Key
            };

            CreateTempTableIfNotExists(tempTableName.TableName, filter, fromDate, toDate, nRecords, CDROption);

            int totalDataCount;
            rslt.Data = GetData(tempTableName.TableName, fromRow, toRow,orderBy,isDescending, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt;
            
        }
        #region Methods
        private void CreateTempTableIfNotExists(string tempTableName, CDRFilter filter, DateTime from, DateTime to, int nRecords, string CDROption)
        {
            string queryData = "";
            if (CDROption .Equals( "Successful"))
            {
                queryData = GetSingleQuery("Billing_CDR_Main", "MainCDR");
            }
            else if (CDROption .Equals( "Failed"))
            {
                queryData = GetSingleQuery("Billing_CDR_Invalid", "Invalid");
            }
            else
            {
                queryData = String.Format(@"{0} UNION ALL {1}", GetSingleQuery("Billing_CDR_Main", "MainCDR"), GetSingleQuery("Billing_CDR_Invalid", "Invalid"));
            }
            StringBuilder whereBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder(@"
                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                            BEGIN

                             WITH SwitchName AS (SELECT s.SwitchID, s.Name FROM Switch s WITH (NOLOCK)),
                       
                        OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS'),
                        CarrierInfo AS
                            (
                                Select P.Name + ' (' + A.NameSuffix + ')' AS Name, A.CarrierAccountID AS CarrierAccountID from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                            ),
                            
		                        AllResult AS
		                        (
			                        SELECT Top (@nRecords) newtable.*,SwitchName.Name As switchName,OurZones.Name As OurZoneName,CarrierInfo.Name AS CustomerInfo  FROM (#Query#)as newtable LEFT JOIN SwitchName ON newtable.SwitchID=SwitchName.SwitchID LEFT JOIN OurZones ON newtable.OurZoneID=OurZones.ZoneID LEFT JOIN CarrierInfo ON newtable.CustomerID=CarrierInfo.CarrierAccountID where Attempt between @FromDate AND @ToDate #FILTER#
		                        )
		                        SELECT * INTO #TEMPTABLE# FROM AllResult
                            END");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#Query#", queryData);
            AddFilterToQuery(filter, whereBuilder);
            queryBuilder.Replace("#FILTER#", whereBuilder.ToString());
            //queryBuilder.Replace("#JOINPART#", groupKeysJoinPart.ToString());
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", from));
                cmd.Parameters.Add(new SqlParameter("@ToDate", to));
                cmd.Parameters.Add(new SqlParameter("@nRecords", nRecords));
            });
        }

        private IEnumerable<BillingCDR> GetData(string tempTableName, int fromRow, int toRow, BillingCDRMeasures orderBy, bool isDescending, out int totalCount)
        {
            string query = String.Format(@"WITH OrderedResult AS (SELECT *, ROW_NUMBER()  OVER(ORDER BY {1} {2})  AS rowNumber FROM {0})
	                                    SELECT * FROM OrderedResult WHERE rowNumber between @FromRow AND @ToRow", tempTableName,GetColumnName(orderBy), isDescending ? "DESC" : "ASC");

            totalCount = (int)ExecuteScalarText(String.Format("SELECT COUNT(*) FROM {0}", tempTableName), null);
            return GetItemsText(query,
                (reader) =>
                {
                    var obj = new BillingCDR();
                    FillCDRDataFromReader(obj, reader);
                    return obj;
                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                });
        }

        private string GetColumnName(BillingCDRMeasures column)
        {
            switch (column)
            {
                //case BillingCDRMeasures.DurationsInMinutes: return "DurationsInSeconds";
                //case BillingCDRMeasures.MaxDurationInMinutes: return "MaxDurationsInSeconds";
                default: return column.ToString();
            }
        }
        private string GetSingleQuery(string tableName,string alias){
            StringBuilder whereBuilder = new StringBuilder();
            return String.Format(@"
                        SELECT 
                        {1}.ID,
                        {1}.Attempt,
                        {1}.Alert,
                        {1}.Connect,
                        {1}.Disconnect ,
                        {1}.DurationInSeconds,
                        {1}.CustomerID,
                        {1}.OurZoneID,
                        {1}.OriginatingZoneID ,
                        {1}.SupplierID,
                        {1}.SupplierZoneID,
                        {1}.CDPN,
                        {1}.CGPN,
                        {1}.ReleaseCode,
                        {1}.ReleaseSource,
                        {1}.SwitchID ,
                        {1}.SwitchCdrID,
                        {1}.Tag,
                        {1}.Extra_Fields,
                        {1}.Port_IN,
                        {1}.Port_OUT,
                        {1}.OurCode,
                        {1}.SupplierCode,
                        {1}.CDPNOut ,
                        {1}.SubscriberID,
                        {1}.SIP FROM {0} {1} 
                            ", tableName, alias);
                        }
        
        private BillingCDR CDRDataMapper(IDataReader reader)
        {

            BillingCDR instance = new BillingCDR();       
            FillCDRDataFromReader(instance, reader);
            return instance;
        }
        private void AddFilterToQuery(CDRFilter filter, StringBuilder whereBuilder)
        {
            AddFilter(whereBuilder, filter.SwitchIds, "newtable.SwitchId");
            AddFilter(whereBuilder, filter.CustomerIds, "newtable.CustomerID");
            AddFilter(whereBuilder, filter.SupplierIds, "newtable.SupplierID");
            AddFilter(whereBuilder, filter.ZoneIds, "newtable.OurZoneID");
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

        void FillCDRDataFromReader(BillingCDR cdr, IDataReader reader)
        {


                cdr.SwitchName = reader["switchName"] as string;
                cdr.CustomerInfo = reader["CustomerInfo"] as string;
                cdr.OurZoneName = reader["OurZoneName"] as string;
                cdr.Attempt = GetReaderValue<DateTime>(reader,"Attempt");
                cdr.Alert = GetReaderValue<DateTime>(reader, "Alert");
                cdr.Connect = GetReaderValue<DateTime>(reader, "Connect");
                cdr.Disconnect = GetReaderValue<DateTime>(reader, "Disconnect");
                cdr.DurationInSeconds = GetReaderOfNumeric(reader, "DurationInSeconds");
                cdr.CustomerID = reader["CustomerID"] as string;
                cdr.OurZoneID = GetReaderValue<int>(reader, "OurZoneID");
                cdr.OriginatingZoneID = GetReaderValue<int>(reader, "OriginatingZoneID");
                cdr.SupplierID = reader["SupplierID"] as string;
                cdr.SupplierZoneID = GetReaderValue<int>(reader, "SupplierZoneID");
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.ReleaseCode = reader["ReleaseCode"] as string;
                cdr.ReleaseSource = reader["ReleaseSource"] as string;
                cdr.SwitchID = GetReaderValueOfTinyInt(reader,"SwitchID");
                cdr.SwitchCdrID = GetReaderValue<Int64>(reader, "SwitchCdrID");
                cdr.Tag = reader["Tag"] as string;
                cdr.Extra_Fields = reader["Extra_Fields"] as string;
                cdr.Port_IN = reader["Port_IN"] as string;
                cdr.Port_OUT = reader["Port_OUT"] as string;
                cdr.OurCode = reader["OurCode"] as string;
                cdr.SupplierCode = reader["SupplierCode"] as string;
                cdr.CDPNOut = reader["CDPNOut"] as string;
                cdr.SubscriberID = GetReaderValue<Int64>(reader, "SubscriberID");
                cdr.SIP = reader["SIP"] as string;
        }
        public int GetReaderOfNumeric(IDataReader reader, String value)
        {
            return reader[value] != DBNull.Value ? Convert.ToInt32(reader[value]) : 0;
        }
        public int GetReaderValueOfTinyInt(IDataReader reader, String value)
        {
            return reader[value] != DBNull.Value ? Convert.ToInt32(reader[value]) : 0;
        }
        #endregion
    }
}
