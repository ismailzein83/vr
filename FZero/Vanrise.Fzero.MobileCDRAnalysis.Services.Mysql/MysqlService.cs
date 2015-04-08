using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceProcess;
using System.Timers;


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
                ErrorLogger("MysqlService() " + ex.Message);
            }

        }

        private void ErrorLogger(string message)
        {
            string cs = "CDRAnalysisMobile";
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
                ErrorLogger("1");
                base.RequestAdditionalTime(int.Parse(System.Configuration.ConfigurationManager.AppSettings["RequestAdditionalTime"].ToString()));
                ErrorLogger("2");
                //Debugger.Launch(); // launch and attach debugger

                // Create a timer with a ten second interval.
                aTimer = new System.Timers.Timer(int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString()));// 60 minutes
                ErrorLogger("3");
                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                ErrorLogger("4");
                aTimer.Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"].ToString());// 60 minutes
                ErrorLogger("5");
                aTimer.Enabled = true;
                ErrorLogger("6");
                GC.KeepAlive(aTimer);
                ErrorLogger("7");
                OnTimedEvent(null, null);
                ErrorLogger("8");
            }
            catch (Exception ex)
            {
                ErrorLogger("OnStart() " + ex.Message);
                ErrorLogger("OnStart() " + ex.ToString());
                ErrorLogger("OnStart() " + ex.InnerException.ToString());
            }

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            try
            {
                //  Important: It is helpful to send the contents of the
                //  sftp.LastErrorText property when requesting support.
                ErrorLogger("7.1");
                Chilkat.SFtp sftp = new Chilkat.SFtp();
                ErrorLogger("7.2");
                //  Any string automatically begins a fully-functional 30-day trial.
                bool success = sftp.UnlockComponent("Anything for 30-day trial");
                ErrorLogger("7.3");
                if (success != true)
                {
                    ErrorLogger("7.4");
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                ErrorLogger("7.5");
                //  Set some timeouts, in milliseconds:
                sftp.ConnectTimeoutMs = 5000;

                ErrorLogger("7.6");
                sftp.IdleTimeoutMs = 10000;


                ErrorLogger("7.7");
                //  Connect to the SSH server.
                //  The standard SSH port = 22
                //  The hostname may be a hostname or IP address.
                int port;

                ErrorLogger("7.8");
                string hostname;
                hostname = System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString();
                ErrorLogger("7.9");
                port = 22;
                ErrorLogger("7.10");
                success = sftp.Connect(hostname, port);
                ErrorLogger("7.11");
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                ErrorLogger("7.12");
                //  Authenticate with the SSH server.  Chilkat SFTP supports
                //  both password-based authenication as well as public-key
                //  authentication.  This example uses password authenication.
                success = sftp.AuthenticatePw(System.Configuration.ConfigurationManager.AppSettings["FTP_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["FTP_Pasword"].ToString());
                if (success != true)
                {
                    ErrorLogger("7.13");
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }
                ErrorLogger("7.14");
                //  After authenticating, the SFTP subsystem must be initialized:
                success = sftp.InitializeSftp();
                if (success != true)
                {
                    ErrorLogger("7.15");
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Open a directory on the server...
                //  Paths starting with a slash are "absolute", and are relative
                //  to the root of the file system. Names starting with any other
                //  character are relative to the user's default directory (home directory).
                //  A path component of ".." refers to the parent directory,
                //  and "." refers to the current directory.
                string handle;
                ErrorLogger("7.16");
                handle = sftp.OpenDir(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString());
                if (handle == null)
                {
                    ErrorLogger("7.17");
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Download the directory listing:
                Chilkat.SFtpDir dirListing = null;
                ErrorLogger("7.18");
                dirListing = sftp.ReadDir(handle);
                if (dirListing == null)
                {
                    ErrorLogger("7.19");
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Iterate over the files.
                int i;
                ErrorLogger("7.20");
                int n = dirListing.NumFilesAndDirs;
                if (n == 0)
                {
                    ErrorLogger("7.21");
                    ErrorLogger("No entries found in this directory." + "/n");
                }
                else
                {
                    ErrorLogger("7.22");
                    for (i = 0; i <= n - 1; i++)
                    {
                        ErrorLogger("7.23");
                        Chilkat.SFtpFile fileObj = null;
                        ErrorLogger("7.24");
                        fileObj = dirListing.GetFileObject(i);


                        if (!fileObj.IsDirectory && fileObj.Filename.ToUpper().Contains(".DAT"))
                        {
                            //ErrorLogger(fileObj.Filename);
                            //ErrorLogger(fileObj.FileType);
                            //ErrorLogger("Size in bytes: " + Convert.ToString(fileObj.Size32));
                            //ErrorLogger("----");
                            ErrorLogger("7.25");
                            DBConnect db = new DBConnect();
                            ErrorLogger("7.26");
                            db.Load(fileObj.Filename);
                            ErrorLogger("7.27");




                            //  Rename the file or directory:
                            success = sftp.RenameFileOrDir(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Filename, System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Filename.Replace(".DAT", ".old"));

                            ErrorLogger("7.28");
                            if (success == false)
                            {
                                ErrorLogger(sftp.LastErrorText + "/n");
                                return;
                            }

                        }




                    }

                }
                ErrorLogger("7.29");
                //  Close the directory
                success = sftp.CloseHandle(handle);
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                ErrorLogger("Success." + "/n");
               
            }
            catch (Exception ex)
            {
                ErrorLogger("OnTimedEvent() " + ex.Message);
                ErrorLogger("OnTimedEvent() " + ex.ToString());
                ErrorLogger("OnTimedEvent() " + ex.InnerException.ToString());
            }




        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

    }
    class DBConnect
    {

        private void ErrorLogger(string message)
        {
            string cs = "CDRAnalysisMobile";
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
            string cs = "CDRAnalysisMobile";
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
        public void Load(string FileName)
        {

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
                }
            }
            catch (MySqlException ex)
            {

            }


        }

    }

}









