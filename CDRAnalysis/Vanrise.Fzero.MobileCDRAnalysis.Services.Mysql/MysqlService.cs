using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Timers;
using Rebex.Net;


namespace Vanrise.Fzero.MobileCDRAnalysis.Services.Mysql
{
    public partial class MysqlService : ServiceBase
    {
        private static System.Timers.Timer aTimer;

        public MysqlService()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                ErrorLog("MysqlService() " + ex.Message);
                ErrorLog("MysqlService() " + ex.ToString());
                ErrorLog("MysqlService() " + ex.InnerException.ToString()); ;
            }

        }

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

        protected override void OnStart(string[] args)
        {

            try
            {
                base.RequestAdditionalTime(int.Parse(System.Configuration.ConfigurationManager.AppSettings["RequestAdditionalTime"].ToString()));
                //Debugger.Launch(); // launch and attach debugger

                // Create a timer with a ten second interval.
                aTimer = new System.Timers.Timer(int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString()));// 60 minutes
                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString());// 60 minutes
                aTimer.Enabled = true;
                GC.KeepAlive(aTimer);
                //OnTimedEvent(null, null);
            }
            catch (Exception ex)
            {
                ErrorLog("OnStart() " + ex.Message);
                ErrorLog("OnStart() " + ex.ToString());
                ErrorLog("OnStart() " + ex.InnerException.ToString());
            }

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            try
            {
                var sftp = new Rebex.Net.Sftp();
                sftp.Connect(System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString());
                sftp.Login(System.Configuration.ConfigurationManager.AppSettings["FTP_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["FTP_Pasword"].ToString());
                if (sftp.GetConnectionState().Connected)
                {
                    // set current directory
                    sftp.ChangeDirectory(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString());
                    // get items within the current directory
                    SftpItemCollection currentItems = sftp.GetList();
                    if (currentItems.Count > 0)
                    {
                        foreach (var fileObj in currentItems)
                        {
                            if (!fileObj.IsDirectory && fileObj.Name.ToUpper().Contains(".DAT"))
                            {
                                DBConnect db = new DBConnect();
                                if (db.Load(fileObj.Name))
                                {
                                    ErrorLog("from path() " + System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name);
                                    ErrorLog("to path() " + System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name.Replace(".DAT", ".old"));

                                    sftp.Rename(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name, System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Name.Replace(".DAT", ".old"));
                                    ErrorLog("XXXXXXXXXXXXXXXXXXXXXXXXXX");
                                }
                            }
                        }
                    }
                    sftp.Disconnect();
                }

                


            }
            catch (Exception ex)
            {
                ErrorLog("OnTimedEvent() " + ex.Message);
                ErrorLog("OnTimedEvent() " + ex.ToString());
                ErrorLog("OnTimedEvent() " + ex.InnerException.ToString());
            }




        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }
    class DBConnect
    {

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

        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public int NewLastCallID = 0;
        public int NewLastCallFailedID = 0;


        //Constructor
        public DBConnect()
        {
            try
            {
                Initialize();
            }
            catch (Exception ex)
            {
                ErrorLog("DBConnect() " + ex.Message);
                ErrorLog("DBConnect() " + ex.ToString());
                ErrorLog("DBConnect() " + ex.InnerException.ToString());
            }

        }

        //Initialize values
        private void Initialize()
        {
            try
            {
                server = System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString();
                database = System.Configuration.ConfigurationManager.AppSettings["DATABASE"].ToString();
                uid = System.Configuration.ConfigurationManager.AppSettings["UID"].ToString();
                password = System.Configuration.ConfigurationManager.AppSettings["PASSWORD"].ToString();
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
                connection = new MySqlConnection(connectionString);
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
                ErrorLog("OpenConnection() " + ex.Message);
                ErrorLog("OpenConnection() " + ex.ToString());
                ErrorLog("OpenConnection() " + ex.InnerException.ToString());
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
                ErrorLog("CloseConnection() " + ex.Message);
                ErrorLog("CloseConnection() " + ex.ToString());
                ErrorLog("CloseConnection() " + ex.InnerException.ToString());
                return false;
            }
        }


        //Insert statement
        public bool Load(string FileName)
        {
            bool results = false;
            try
            {

                string query = "drop table if exists filesDAT; create table filesDAT(content varchar(1000)); ALTER TABLE filesDAT CONVERT TO CHARACTER SET utf8 COLLATE utf8_persian_ci; LOAD DATA INFILE './" + FileName + "' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n'; INSERT INTO NormalCDR (  MSISDN,                                  IMSI ,                                ConnectDateTime ,                                Destination  ,                               DurationInSeconds ,                               Call_Class  ,                                Call_Type  ,                              Sub_Type   ,                              IMEI  ,                              BTS_Id   ,                                            Cell_Id  ,                                Up_Volume  ,                                Down_Volume ,                                 Cell_Latitude  ,                              Cell_Longitude  ,                              In_Trunk  ,                               Out_Trunk  )    select               trim(substr(content, 146, 20)) MSISDN,   trim(substr(content, 126, 20)) IMSI,  trim(substr(content, 222, 14)) ConnectDateTime,  trim(substr(content, 199, 20)) Destination,  trim(substr(content, 236, 5)) DurationInSeconds,  trim(substr(content, 435, 10)) Call_Class,   trim(substr(content, 103, 3)) Call_Type,  trim(substr(content, 166, 10)) Sub_Type,  trim(substr(content, 106, 20)) IMEI, left(trim(substr(content, 253, 22)), char_length(trim(substr(content, 253, 22)))-1)  BTS_Id,  trim(substr(content, 253, 22)) Cell_Id,   trim(substr(content, 589, 10)) Up_Volume,   trim(substr(content, 599, 10)) Down_Volume,   trim(substr(content, 610, 9)) Cell_Latitude,  trim(substr(content, 619, 9)) Cell_Longitude,  trim(substr(content, 415, 20)) In_Trunk,  trim(substr(content, 395, 20)) Out_Trunk from filesDAT ; ";
                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.CommandTimeout = 0;
                    //Execute command
                    cmd.ExecuteNonQuery();
                    //close connection
                    this.CloseConnection();
                    results = true;
                }
            }
            catch (MySqlException ex)
            {
                ErrorLog("Load() " + ex.Message);
                ErrorLog("Load() " + ex.ToString());
                ErrorLog("Load() " + ex.InnerException.ToString());
            }
            return results;


        }

    }

}









