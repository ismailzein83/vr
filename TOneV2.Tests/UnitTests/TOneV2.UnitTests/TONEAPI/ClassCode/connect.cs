using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;

namespace TONEAPI
{
    class connect
    {


        public DataSet getdata(string Query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=ToneV2ConfigurationStructure;User ID=sa;Password=QAP@ssw0rd");

            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = Query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();
            return ds1;



        }

        public DataSet getdata2(string Query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=TOneV2_QA;User ID=sa;Password=QAP@ssw0rd");

            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = Query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();
            return ds1;



        }


        public DataSet getdatatesting(string Query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=QAP@ssw0rd");

            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = Query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();
            return ds1;



        }
        public void updatedata(string Query)
        {

            using (SqlConnection openCon = new SqlConnection("Server=192.168.110.195;Database=ToneV2testing;User ID=sa;Password=QAP@ssw0rd"))
             {
              string saveStaff = Query;

                      using(SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                       {
                         querySaveStaff.Connection=openCon;
       
                         openCon.Open();
                         querySaveStaff.ExecuteNonQuery();
                         openCon.Close();
                       }
             }
        }
    }
}
