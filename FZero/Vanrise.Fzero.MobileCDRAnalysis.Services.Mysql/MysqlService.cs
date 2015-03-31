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

                Chilkat.SFtp sftp = new Chilkat.SFtp();

                //  Any string automatically begins a fully-functional 30-day trial.
                bool success = sftp.UnlockComponent("Anything for 30-day trial");
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Set some timeouts, in milliseconds:
                sftp.ConnectTimeoutMs = 5000;
                sftp.IdleTimeoutMs = 10000;

                //  Connect to the SSH server.
                //  The standard SSH port = 22
                //  The hostname may be a hostname or IP address.
                int port;
                string hostname;
                hostname = System.Configuration.ConfigurationManager.AppSettings["SERVER"].ToString();
                port = 22;
                success = sftp.Connect(hostname, port);
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Authenticate with the SSH server.  Chilkat SFTP supports
                //  both password-based authenication as well as public-key
                //  authentication.  This example uses password authenication.
                success = sftp.AuthenticatePw(System.Configuration.ConfigurationManager.AppSettings["FTP_Username"].ToString(), System.Configuration.ConfigurationManager.AppSettings["FTP_Pasword"].ToString());
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  After authenticating, the SFTP subsystem must be initialized:
                success = sftp.InitializeSftp();
                if (success != true)
                {
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
                handle = sftp.OpenDir(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString());
                if (handle == null)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Download the directory listing:
                Chilkat.SFtpDir dirListing = null;
                dirListing = sftp.ReadDir(handle);
                if (dirListing == null)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //  Iterate over the files.
                int i;
                int n = dirListing.NumFilesAndDirs;
                if (n == 0)
                {
                    ErrorLogger("No entries found in this directory." + "/n");
                }
                else
                {
                    for (i = 0; i <= n - 1; i++)
                    {
                        Chilkat.SFtpFile fileObj = null;
                        fileObj = dirListing.GetFileObject(i);


                        if (!fileObj.IsDirectory && fileObj.Filename.ToUpper().Contains(".DAT"))
                        {
                            //ErrorLogger(fileObj.Filename);
                            //ErrorLogger(fileObj.FileType);
                            //ErrorLogger("Size in bytes: " + Convert.ToString(fileObj.Size32));
                            //ErrorLogger("----");

                            DBConnect db = new DBConnect();
                            db.Load(fileObj.Filename);




                            //  Rename the file or directory:
                            success = sftp.RenameFileOrDir(System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Filename, System.Configuration.ConfigurationManager.AppSettings["FTP_Path"].ToString() + "/" + fileObj.Filename.Replace(".DAT", ".old"));


                            if (success == false)
                            {
                                ErrorLogger(sftp.LastErrorText + "/n");
                                return;
                            }

                        }




                    }

                }

                //  Close the directory
                success = sftp.CloseHandle(handle);
                if (success != true)
                {
                    ErrorLogger(sftp.LastErrorText + "/n");
                    return;
                }

                //ErrorLogger("Success." + "/n");

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

                //string query = "LOAD DATA INFILE './"+FileName+"' INTO TABLE filesDAT CHARACTER SET UTF8  LINES TERMINATED BY '\n';";

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
                }
            }
            catch (MySqlException ex)
            {

            }


        }

    }

}









