using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TONEAPI
{
    class connect
    {


        public DataSet getdatas(string Query)
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
        public List<CodeGroup> getcodegroup(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemov2;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<CodeGroup>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new CodeGroup
            {
                CodeGroupId = row.Field<int>("id"),
                CountryId = row.Field<int>("CountryId"),
                Code = row.Field<string>("Code")
               
            }).ToList();


        }
        public List<SupplierZone> getzonedata(string query)
        {
             SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierZone>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();

        
            
            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierZone {
                 Name = row.Field<string>("zonename"),
                SupplierId = row.Field<int>("supplierid"),
                SupplierZoneId = row.Field<int>("zoneid"),
                BED = row.Field<DateTime>("bed"),
                EED = row.Field<DateTime?>("eed"),
                CountryId = row.Field<int>("countryid")
            }).ToList();

          
        }
        public List<SupplierCode> getcodedata(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierCode>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierCode
            {
                SupplierCodeId = row.Field<int>("codeid"),
                Code = row.Field<string>("code"),
                ZoneId = row.Field<int>("zoneid"),
                BED = row.Field<DateTime>("bed"),
                EED=row.Field<DateTime?>("eed")
            }).ToList();


        }
        public List<SupplierRate> getratedata(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierRate>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierRate
            {
                PriceListId=row.Field<int>("rateid"),
                ZoneId=row.Field<int>("zoneid"),
                NormalRate =row.Field<Decimal>("rate"),
                CurrencyId=row.Field<int>("currencyid"),
                SupplierRateId=row.Field<int>("rateid"),
                BED = row.Field<DateTime>("bed"),
              EED = row.Field<DateTime?>("eed")
            }).ToList();


        }
        public List<SupplierZone> getresultzonedata(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierZone>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierZone
            {
                SupplierZoneId = row.Field<int>("zoneid"),
                Name = row.Field<string>("zonename"),
                BED = row.Field<DateTime>("bed"),
                EED = row.Field<DateTime?>("eed")
            }).ToList();


        }
        public List<SupplierCode> getresultcodedata(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierCode>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierCode
            {
                SupplierCodeId = row.Field<int>("codeid"),
                Code = row.Field<string>("code"),
                BED = row.Field<DateTime>("bed"),
                EED = row.Field<DateTime?>("eed")
            }).ToList();


        }
        public List<SupplierRate> getresultratedata(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<SupplierRate>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new SupplierRate
            {
                SupplierRateId = row.Field<int>("rateid"),
                NormalRate = row.Field<Decimal>("rate"),              
                BED = row.Field<DateTime>("bed"),
                EED = row.Field<DateTime?>("eed")
            }).ToList();


        }
        public List<ImportedRate> getnewrate(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<ImportedRate>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new ImportedRate
            {
                ZoneName = row.Field<string>("zonename"),
                NormalRate = row.Field<decimal>("Rate"),
                BED = row.Field<DateTime>("bed"),
                CurrencyId = row.Field<int>("currency")
            }).ToList();


        }
        public List<ImportedCode> getnewcode(string query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");
            var ddd = new List<ImportedCode>();
            SqlDataAdapter da = new SqlDataAdapter();
            myConn.Open();
            DataSet ds1 = new DataSet();
            string sQueryString1 = "";
            sQueryString1 = query;
            da.SelectCommand = new SqlCommand(sQueryString1, myConn);
            da.Fill(ds1, "table");
            myConn.Close();



            return ds1.Tables[0].AsEnumerable().Select(row => new ImportedCode
            {
                ZoneName = row.Field<string>("zonename"),
                Code = row.Field<string>("Code"),
                BED = row.Field<DateTime>("bed"),

            }).ToList();


        }
        public DataSet getdata(string Query)
        {
            SqlConnection myConn = new SqlConnection("Server=192.168.110.195;Database=mvtsprodemox;User ID=sa;Password=QAP@ssw0rd");

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
    }
    }

