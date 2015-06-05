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
namespace TOne.Analytics.Data.SQL
{
    class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {
        public List<CDR> GetCDRData(CDRFilter filter,DateTime fromDate, DateTime toDate, int nRecords, string CDROption)
        {
            string queryData = "";
            if (CDROption == "Successful")
            {
                queryData = GetSingleQuery("Billing_CDR_Main","MainCDR");
            }
            else if (CDROption == "Failed")
            {
                queryData = GetSingleQuery("Billing_CDR_Invalid", "Invalid");
            }
            else
            {
                queryData = String.Format(@"{0} UNION ALL {1}", GetSingleQuery("Billing_CDR_Main", "MainCDR"), GetSingleQuery("Billing_CDR_Invalid", "Invalid"));
            }
            string query="";


            query = String.Format(@"SELECT Top (@nRecords) * FROM ({0})as newtable where Attempt between @FromDate AND @ToDate ORDER BY Attempt;", queryData);
                return GetItemsText(query, CDRDataMapper,
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));
                        cmd.Parameters.Add(new SqlParameter("@nRecords", nRecords));
                    });
            
        }
        private string GetSingleQuery(string tableName,string alias){
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
        
        private CDR CDRDataMapper(IDataReader reader)
        {

            CDR instance = new CDR();       
            FillCDRDataFromReader(instance, reader);
            return instance;
        }
        void FillCDRDataFromReader(CDR cdr, IDataReader reader)
        {
                cdr.ID = GetReaderValue<Int64>(reader, "ID");
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
    }
}
