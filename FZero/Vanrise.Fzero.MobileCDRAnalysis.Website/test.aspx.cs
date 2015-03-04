using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using Vanrise.Fzero.MobileCDRAnalysis;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


            foreach (string path in Directory.GetFiles("C:\\", "*.dat"))
            {
                // Read all lines into an array
                string[] contents = File.ReadAllLines(path);
                // Sort the in-memory array


                foreach (var i in contents)
                {
                    Response.Write("Source_Type: " + i.Substring(0, 9));
                    Response.Write("Source_Name: " + i.Substring(0, 9));
                    Response.Write("Source_File: " + i.Substring(0, 9));
                    Response.Write("Record_Type: " + i.Substring(0, 9));
                    Response.Write("Call_Type: " + i.Substring(0, 9));
                    Response.Write("IMEI: " + i.Substring(0, 9));
                    Response.Write("IMEI14: " + i.Substring(0, 9));
                    Response.Write("Entity: " + i.Substring(0, 9));
                    Response.Write("Substring: " + i.Substring(0, 9));
                    Response.Write("Substring: " + i.Substring(0, 9));
                    Response.Write("Substring: " + i.Substring(0, 9));

                }


                           }





            string cs = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR=YES;FMT=Delimited\"", "C:/Users/Walid/Desktop/CDR/sample-switch-records.dat");

            StringBuilder sb = new StringBuilder();
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * from [sample-switch-records.dat]", conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        sb.AppendFormat("{0}: {1}|", dc.ColumnName, dr[dc]);
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("\r\n");
                }
                Console.Write(sb.ToString());
            }




           
                // Create an instance of StreamReader to read from a file. 
                // The using statement also closes the StreamReader. 
                using (StreamReader sr = new StreamReader("C:/FMS_Import/Automatic/CDR/sample-switch-records.DAT"))
                {
                    string line;

                    // Read and display lines from the file until 
                    // the end of the file is reached. 
                    while ((line = sr.ReadLine()) != null)
                    {
                        Response.Write(line + "                                                                                 \n\n\n\n\n\n\n\n\n\n   ");
                    }
                }
           
           // Console.ReadKey();






















            //var strAccessSelect = "select * from sample-switch-records.DAT";
            //DataSet myDataSet = new DataSet();
            //OleDbConnection myAccessConn = null;


            //try
            //{
            //    myAccessConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\FMS_Import\\Automatic\\CDR\\sample-switch-records.DAT;Extended Properties=\"text;HDR=NO;FMT=FixedLength\"");
            //    OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, myAccessConn);
            //    OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);

            //    myAccessConn.Open();
            //    myDataAdapter.Fill(myDataSet, "CDRs");

            //}
            //catch (Exception ex)
            //{
            //    Response.Write("Error: Failed to retrieve the required data from the DataBase." + ex.Message);
            //    return;
            //}
            //finally
            //{
            //    myAccessConn.Close();
            //}

            //// A dataset can contain multiple tables, so let's get them
            //// all into an array:
            //DataTableCollection dta = myDataSet.Tables;
            //foreach (DataTable dt in dta)
            //{
            //    Response.Write("Found data table " + dt.TableName);
            //}

            //// The next two lines show two different ways you can get the
            //// count of tables in a dataset:
            //Response.Write(myDataSet.Tables.Count + " tables in data set");
            //Response.Write(dta.Count + " tables in data set");
            //// The next several lines show how to get information on
            //// a specific table by name from the dataset:
            //Response.Write(myDataSet.Tables["Categories"].Rows.Count + " rows in Categories table");
            //// The column info is automatically fetched from the database,
            //// so we can read it here:
            //Response.Write(myDataSet.Tables["Categories"].Columns.Count + " columns in Categories table");
            //DataColumnCollection drc = myDataSet.Tables["Categories"].Columns;
            //int i = 0;
            //foreach (DataColumn dc in drc)
            //{
            //    // Print the column subscript, then the column's name
            //    // and its data type:

            //    Response.Write("Column name[+ " + (i++).ToString() + "] is " + dc.ColumnName + ", of type " + dc.DataType.Name.ToString());
            //}
            //DataRowCollection dra = myDataSet.Tables["Categories"].Rows;
            //foreach (DataRow dr in dra)
            //{
            //    // Print the CategoryID as a subscript, then the CategoryName:
            //    Response.Write("CategoryName[" + dr[0].ToString() + "] is" + dr[1].ToString());
            //}







        }

    }
}