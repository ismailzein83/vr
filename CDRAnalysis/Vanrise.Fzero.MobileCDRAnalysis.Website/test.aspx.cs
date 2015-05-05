using System;
using System.Collections.Generic;
using System.Configuration;
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
using Rebex.Net;
using MySql.Data.MySqlClient;
using System.Diagnostics;

public partial class test : System.Web.UI.Page
{

    private string ExportReportToExcel(string reportName, int ReportID)
    {
        ReportViewer rvToOperator = new ReportViewer();

        rvToOperator.LocalReport.ReportPath = Path.Combine(string.Empty, @"Reports\rptReportedNumbers.rdlc");

        ReportDataSource rptDataSourcedsViewGeneratedCalls = new ReportDataSource("DSReportedNumbers", vwReportedNumber.GetList(ReportID));
        rvToOperator.LocalReport.DataSources.Add(rptDataSourcedsViewGeneratedCalls);

        ReportDataSource rptDataSourceReportedNumberNormalCDRs = new ReportDataSource("DSReportedNumberNormalCDR", vwReportedNumberNormalCDR.GetList(ReportID));
        rvToOperator.LocalReport.DataSources.Add(rptDataSourceReportedNumberNormalCDRs);

        rvToOperator.LocalReport.Refresh();

        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string filenameExtension;
        byte[] bytes = rvToOperator.LocalReport.Render(
           "Excel", null, out mimeType, out encoding, out filenameExtension,
            out streamids, out warnings);

        string filename = Path.Combine(ConfigurationManager.AppSettings["ReportsPath"], reportName);
        using (var fs = new FileStream(filename, FileMode.Create))
        {
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        return filename;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            //try
            //{

            //    var sftp = new Rebex.Net.Sftp();
            //    sftp.Connect(System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString());
            //    sftp.Login(System.Configuration.ConfigurationManager.AppSettings["FTP_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["FTP_Pasword"].ToString());

            //    if (sftp.GetConnectionState().Connected)
            //    {
            //        // set current directory
            //        sftp.ChangeDirectory(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString());
            //        // get items within the current directory
            //        SftpItemCollection currentItems = sftp.GetList();
            //        if (currentItems.Count > 0)
            //        {
            //            foreach (var fileObj in currentItems)
            //            {
            //                if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(".DAT"))
            //                {
            //                    DBConnect db = new DBConnect();
            //                    if (db.Load(fileObj.Name))
            //                    {
            //                        sftp.Rename(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name, System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name.Replace(".DAT", ".old"));
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    sftp.Disconnect();
               

            //}
            //catch (Exception ex)
            //{
            //    //ErrorLogger("OnTimedEvent() " + ex.Message);
            //    //ErrorLogger("OnTimedEvent() " + ex.ToString());
            //    //ErrorLogger("OnTimedEvent() " + ex.InnerException.ToString());
            //}



            Vanrise.Fzero.MobileCDRAnalysis.Report report = Vanrise.Fzero.MobileCDRAnalysis.Report.Load("201412050001");
            string ReportID = "CA" + report.ReportNumber + DateTime.Now.Year.ToString("D2").Substring(2) + DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") + DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2");
            EmailManager.SendReporttoITPC(ExportReportToExcel("201412050001" + ".xls", 5), ReportID, "FMS_Profile");
            report.SentDate = DateTime.Now;
            report.SentBy = 1;
            report.ReportingStatusID = (int)Enums.ReportingStatuses.Sent;
            report.ReportID = ReportID;
            Vanrise.Fzero.MobileCDRAnalysis.Report.Save(report);


           // // walid is testing source control from desktop

           // foreach (string path in Directory.GetFiles("C:\\", "*.dat"))
           // {
           //     // Read all lines into an array
           //     string[] contents = File.ReadAllLines(path);
           //     // Sort the in-memory array


           //     foreach (var i in contents)
           //     {
           //         Response.Write("Source_Type: " + i.Substring(0, 9));
           //         Response.Write("Source_Name: " + i.Substring(0, 9));
           //         Response.Write("Source_File: " + i.Substring(0, 9));
           //         Response.Write("Record_Type: " + i.Substring(0, 9));
           //         Response.Write("Call_Type: " + i.Substring(0, 9));
           //         Response.Write("IMEI: " + i.Substring(0, 9));
           //         Response.Write("IMEI14: " + i.Substring(0, 9));
           //         Response.Write("Entity: " + i.Substring(0, 9));
           //         Response.Write("Substring: " + i.Substring(0, 9));
           //         Response.Write("Substring: " + i.Substring(0, 9));
           //         Response.Write("Substring: " + i.Substring(0, 9));

           //     }


           //                }





           // string cs = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR=YES;FMT=Delimited\"", "C:/Users/Walid/Desktop/CDR/sample-switch-records.dat");

           // StringBuilder sb = new StringBuilder();
           // using (OleDbConnection conn = new OleDbConnection(cs))
           // {
           //     conn.Open();
           //     OleDbDataAdapter da = new OleDbDataAdapter("SELECT * from [sample-switch-records.dat]", conn);
           //     DataSet ds = new DataSet();
           //     da.Fill(ds);
           //     DataTable dt = ds.Tables[0];
           //     foreach (DataRow dr in dt.Rows)
           //     {
           //         foreach (DataColumn dc in dt.Columns)
           //         {
           //             sb.AppendFormat("{0}: {1}|", dc.ColumnName, dr[dc]);
           //         }
           //         sb.Remove(sb.Length - 1, 1);
           //         sb.Append("\r\n");
           //     }
           //     Console.Write(sb.ToString());
           // }




           
           //     // Create an instance of StreamReader to read from a file. 
           //     // The using statement also closes the StreamReader. 
           //     using (StreamReader sr = new StreamReader("C:/FMS_Import/Automatic/CDR/sample-switch-records.DAT"))
           //     {
           //         string line;

           //         // Read and display lines from the file until 
           //         // the end of the file is reached. 
           //         while ((line = sr.ReadLine()) != null)
           //         {
           //             Response.Write(line + "                                                                                 \n\n\n\n\n\n\n\n\n\n   ");
           //         }
           //     }
           
           //// Console.ReadKey();






















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

class DBConnect
{

    private void ErrorLogger(string message)
    {
        string cs = "CDRAnalysisMobile_WF";
        EventLog elog = new EventLog();
        if (!EventLog.SourceExists(cs))
        {
            EventLog.CreateEventSource(cs, cs);
        }
        elog.Source = cs;
        elog.EnableRaisingEvents = true;
        elog.WriteEntry(message);
    }

    private MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;
    public int NewLastCallID = 0;
    public int NewLastCallFailedID = 0;

    private void ErrorLog(string message)
    {
        string cs = "CDRAnalysisMobile_WF";
        EventLog elog = new EventLog();
        if (!EventLog.SourceExists(cs))
        {
            EventLog.CreateEventSource(cs, cs);
        }
        elog.Source = cs;
        elog.EnableRaisingEvents = true;
        elog.WriteEntry(message);
    }

    //Constructor
    public DBConnect()
    {
        try
        {
            Initialize();
        }
        catch (Exception ex)
        {

        }

    }

    //Initialize values
    private void Initialize()
    {
        try
        {
            ErrorLogger("7.25.1");
            server = System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString();
            database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"].ToString();
            uid = System.Configuration.ConfigurationManager.AppSettings["UID"].ToString();
            password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"].ToString();
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            ErrorLogger("7.25.2");
            connection = new MySqlConnection(connectionString);
            ErrorLogger(connectionString);
        }
        catch (Exception ex)
        {
            ErrorLog("Initialize() " + ex.Message);
            ErrorLog("Initialize() " + ex.ToString());
            ErrorLog("Initialize() " + ex.InnerException.ToString());
        }

    }

    //open connection to database
    private bool OpenConnection()
    {
        try
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            return true;
        }
        catch (MySqlException ex)
        {

            return false;
        }
    }

    //Close connection
    private bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException ex)
        {

            return false;
        }
    }


    //Insert statement
    public bool Load(string FileName)
    {
        bool results = false;
        try
        {
            ErrorLogger("7.26.1");
            //string query = "LOAD DATA INFILE './"+FileName+"' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n';";

            string query = "drop table if exists filesDAT; create table filesDAT(content varchar(1000)); ALTER TABLE filesDAT CONVERT TO CHARACTER SET utf8 COLLATE utf8_persian_ci; LOAD DATA INFILE './" + FileName + "' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n'; INSERT INTO NormalCDR (  MSISDN,                                  IMSI ,                                ConnectDateTime ,                                Destination  ,                               DurationInSeconds ,                               Call_Class  ,                                Call_Type  ,                              Sub_Type   ,                              IMEI  ,                              BTS_Id   ,                                            Cell_Id  ,                                Up_Volume  ,                                Down_Volume ,                                 Cell_Latitude  ,                              Cell_Longitude  ,                              In_Trunk  ,                               Out_Trunk  )    select               trim(substr(content, 146, 20)) MSISDN,   trim(substr(content, 126, 20)) IMSI,  trim(substr(content, 222, 14)) ConnectDateTime,  trim(substr(content, 199, 20)) Destination,  trim(substr(content, 236, 5)) DurationInSeconds,  trim(substr(content, 435, 10)) Call_Class,   trim(substr(content, 103, 3)) Call_Type,  trim(substr(content, 166, 10)) Sub_Type,  trim(substr(content, 106, 20)) IMEI, left(trim(substr(content, 253, 22)), char_length(trim(substr(content, 253, 22)))-1)  BTS_Id,  trim(substr(content, 253, 22)) Cell_Id,   trim(substr(content, 589, 10)) Up_Volume,   trim(substr(content, 599, 10)) Down_Volume,   trim(substr(content, 610, 9)) Cell_Latitude,  trim(substr(content, 619, 9)) Cell_Longitude,  trim(substr(content, 415, 20)) In_Trunk,  trim(substr(content, 395, 20)) Out_Trunk from filesDAT ; ";
            ErrorLogger("7.26.2");
            //open connection
            if (this.OpenConnection() == true)
            {
                ErrorLogger("7.26.3");
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                ErrorLogger("7.26.4");
                cmd.CommandTimeout = 0;
                ErrorLogger("7.26.5");
                //Execute command
                cmd.ExecuteNonQuery();
                ErrorLogger("7.26.6");
                //close connection
                this.CloseConnection();
                ErrorLogger("7.26.7");
                results = true;
            }
        }
        catch (MySqlException ex)
        {

        }
        return results;


    }

}