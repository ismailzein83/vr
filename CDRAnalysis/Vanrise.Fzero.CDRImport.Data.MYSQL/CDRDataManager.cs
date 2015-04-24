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
           

            MySqlConnection connection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE NormalCDR FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r';", filename.Replace(@"\", @"\\"));
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
