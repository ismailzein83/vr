using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;



namespace Vanrise.Fzero.Services.Import
{
    public partial class ImportService : ServiceBase
    {

        FileSystemWatcher watcher;

        public ImportService()
        {
            InitializeComponent();
        }

        private static void ErrorLog(string message)
        {
            string cs = "Import Service";
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
            base.RequestAdditionalTime(60000); // 10 minutes timeout for startup
            watch();
        }


        private void watch()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["UploadPath"];
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Filter = "*.*";
            watcher.Changed += WatcherActivity;
            watcher.Created += WatcherActivity;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }


        static void WatcherActivity(object sender, FileSystemEventArgs e)
        {
            try
            {
                FileInfo fInfo = new FileInfo(e.FullPath);
                while (IsFileLocked(fInfo))
                {
                    Thread.Sleep(10000);
                }
                Vanrise.Fzero.Bypass.Import.ImportCDRs(e.FullPath, Path.GetDirectoryName(e.FullPath).Split('\\').ToList<String>().Last());
            }
            catch (Exception ex)
            {
                ErrorLog("WatcherActivity " + ex.Message);
                ErrorLog("WatcherActivity " + ex.InnerException);
                ErrorLog("WatcherActivity " + ex.ToString());
                ErrorLog("WatcherActivity " + ex.InnerException.StackTrace);
            }

        }

    }


}
