using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.Analytics.Entities;
using TOne.Analytics.Data;
using System.Data;
namespace TOne.Analytics.Data.SQL
{
    class CDRDataManager : BaseTOneDataManager, ICDRDataManager
    {
        public List<CDR> GetCDRData(DateTime fromDate, DateTime toDate, int nRecords)
        {
            return GetItemsSP("Analytics.sp_Billing_GetCDRData", CDRDataMapper, fromDate, toDate, nRecords);
        }
        private CDR CDRDataMapper(IDataReader reader)
        {
            CDR instance = new CDR
            {
                ID = GetReaderValue<int>(reader, "ID"),
                Attempt = GetReaderValue<DateTime>(reader, "Attempt"),
                Alert = GetReaderValue<DateTime>(reader, "Alert"),
                Connect = GetReaderValue<DateTime>(reader, "Connect"),
                Disconnect = GetReaderValue<DateTime>(reader, "Disconnect"),
                DurationInSeconds = GetReaderValue<int>(reader, "DurationInSeconds"),
                CustomerID = reader["CustomerID"] as string,
                OurZoneID = GetReaderValue<int>(reader, "OurZoneID"),
                OriginatingZoneID = GetReaderValue<int>(reader, "OriginatingZoneID"),
                SupplierID = reader["SupplierID"] as string,
                SupplierZoneID = GetReaderValue<int>(reader, "SupplierZoneID"),
                CDPN = reader["CDPN"] as string,
                CGPN = reader["CGPN"] as string,
                ReleaseCode = reader["ReleaseCode"] as string,
                ReleaseSource = reader["ReleaseSource"] as string,
                SwitchID = GetReaderValue<int>(reader, "SwitchID"),
                SwitchCdrID = GetReaderValue<int>(reader, "SwitchCdrID"),
                Tag = reader["Tag"] as string,
                Extra_Fields = reader["Extra_Fields"] as string,
                Port_IN = reader["Port_IN"] as string,
                Port_OUT = reader["Port_OUT"] as string,
                OurCode = reader["OurCode"] as string,
                SupplierCode = reader["SupplierCode"] as string,
                CDPNOut = reader["CDPNOut"] as string,
                SubscriberID = GetReaderValue<int>(reader, "SubscriberID"),
                SIP = reader["SIP"] as string,
               
            }; 
            return instance;
        }
    }
}
