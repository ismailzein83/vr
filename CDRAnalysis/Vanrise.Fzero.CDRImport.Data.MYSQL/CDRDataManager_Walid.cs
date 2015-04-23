using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Data;
using MySql.Data;
using MySql.Web;


namespace Vanrise.Fzero.CDRImport.Data.MYSQL
{
    public class CDRDataManager_Walid : BaseDataManager, ICDRDataManager
    {
        public CDRDataManager_Walid()
            : base("CDRDBConnectionString")
        {

        }


        public Object PrepareCDRsForDBApply(List<CDR> cdrs)
        {


            // SET autocommit=0;
            //... SQL import statements ...
            //COMMIT;


            // SET unique_checks=0;
            //... SQL import statements ...
            //SET unique_checks=1;

            //            SET foreign_key_checks=0;
            //... SQL import statements ...
            //SET foreign_key_checks=1;


            //INSERT INTO yourtable VALUES (1,2), (5,5), ...;


            //LOAD DATA LOCAL INFILE 'cities_test.txt' INTO TABLE city FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '"' LINES TERMINATED BY '\r\n' IGNORE 1 LINES;
            //LOAD DATA INFILE 'sqlScript1.txt' INTO TABLE USER FIELDS TERMINATED BY ','  LINES STARTING BY '\r';
            //LOAD DATA LOCAL INFILE 'd:/test.csv' INTO TABLE test FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n';
            //LOAD DATA LOCAL INFILE 'd:/test.csv' INTO TABLE city FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '"' LINES TERMINATED BY '\n' ;
            //LOAD DATA LOCAL INFILE 'myfile.csv' INTO TABLE seriallog FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES (FLEX_PN, FLEX_PLANT, FLEX_ORDID, FLEX_REV, CUST_PN, CUST_REV, SERIALID) SET CREATED = CURRENT_TIMESTAMP;











            //StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            //foreach (CDR cdr in cdrs)
            //{
            //    stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
            //                  new[]  {   cdr.MSISDN.ToString()
            //                      , cdr.IMSI.ToString()
            //                      , cdr.ConnectDateTime.ToString()
            //                      , cdr.Destination.ToString()
            //                      , cdr.DurationInSeconds.ToString()
            //                      , ""
            //                      , cdr.Call_Class.ToString()
            //                      , cdr.IsOnNet.ToString()
            //                      , cdr.Call_Type.ToString()
            //                      , cdr.Sub_Type.ToString()
            //                      , cdr.IMEI.ToString()
            //                      , ""
            //                      , cdr.Cell_Id.ToString()
            //                      , ""
            //                      , ""
            //                      , ""
            //                      , cdr.Cell_Latitude.ToString()
            //                      , cdr.Cell_Longitude.ToString()
            //                      , cdr.In_Trunk.ToString()
            //                      , cdr.Out_Trunk.ToString()
            //                      , ""
            //                      , ""}
            //    );
            //}

            //stream.Close();

            //return new StreamBulkInsertInfo
            //{
            //    TableName = "[dbo].[NormalCDR]",
            //    Stream = stream,
            //    TabLock = false,
            //    KeepIdentity = false,
            //    FieldSeparator = ','
            //};


            throw new NotImplementedException();
        }

        public void ApplyCDRsToDB(object preparedInvalidCDRs)
        {
            throw new NotImplementedException();
        }
    }
}
