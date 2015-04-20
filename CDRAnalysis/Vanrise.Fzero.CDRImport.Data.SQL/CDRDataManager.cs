using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public void ApplyCDRsToDB(Object preparedCDRs)
        {
            InsertBulkToTable(preparedCDRs as BulkInsertInfo);
        }

        public Object PrepareCDRsForDBApply(List<CDR> cdrs)
        {
            string filePath = GetFilePathForBulkInsert(); // @"C:\Users\mustafa.hawi\Desktop\test.txt";//

            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (CDR cdr in cdrs)
                {
                    wr.WriteLine(String.Format("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                                   cdr.MSISDN
                                  , cdr.IMSI
                                  , ""//cdr.ConnectDateTime
                                  , cdr.Destination
                                  , cdr.DurationInSeconds
                                  , ""//cdr.DisconnectDateTime.ToString()
                                  , cdr.Call_Class
                                  , cdr.IsOnNet
                                  , cdr.Call_Type
                                  , cdr.Sub_Type
                                  , cdr.IMEI
                                  , cdr.BTS_Id
                                  , cdr.Cell_Id
                                  , cdr.SwitchRecordId
                                  , cdr.Up_Volume
                                  , cdr.Down_Volume
                                  , cdr.Cell_Latitude
                                  , cdr.Cell_Longitude
                                  , cdr.In_Trunk
                                  , cdr.Out_Trunk
                                  , cdr.Service_Type
                                  , cdr.Service_VAS_Name));


                }
                wr.Close();
                wr.Dispose();
            }

            return new BulkInsertInfo
            {
                TableName = "[dbo].[NormalCDR]",
                DataFilePath = filePath,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = ','
            };
        }

    }
}
