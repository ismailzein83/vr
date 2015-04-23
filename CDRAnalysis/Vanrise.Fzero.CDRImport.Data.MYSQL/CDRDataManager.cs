using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;

namespace Vanrise.Fzero.CDRImport.Data.MYSQL
{
    public class CDRDataManager : BaseMySQLDataManager, ICDRDataManager
    {
        public CDRDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void SaveCDRsToDB(List<CDR> cdrs)
        {
            string filename = GetFilePathForBulkInsert();

            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (CDR cdr in cdrs)
                {
                    sw.WriteLine("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                                  new[]  {   cdr.MSISDN.ToString()
                                  , cdr.IMSI.ToString()
                                  , cdr.ConnectDateTime.ToString()
                                  , cdr.Destination.ToString()
                                  , cdr.DurationInSeconds.ToString()
                                  , ""
                                  , cdr.Call_Class.ToString()
                                  , cdr.IsOnNet.ToString()
                                  , cdr.Call_Type.ToString()
                                  , cdr.Sub_Type.ToString()
                                  , cdr.IMEI.ToString()
                                  , ""
                                  , cdr.Cell_Id.ToString()
                                  , ""
                                  , ""
                                  , ""
                                  , cdr.Cell_Latitude.ToString()
                                  , cdr.Cell_Longitude.ToString()
                                  , cdr.In_Trunk.ToString()
                                  , cdr.Out_Trunk.ToString()
                                  , ""
                                  , ""}
                    );
                }
                sw.Close();
            }

            MySqlConnection connection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE NormalCDR FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r';", filename.Replace(@"\", @"\\"));
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
