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
            
            string cs = "Import Service";
            EventLog elog = new EventLog();
            elog.WriteEntry("1");
            if (!EventLog.SourceExists(cs))
            {
                elog.WriteEntry("2");
                EventLog.CreateEventSource(cs, cs);
            }
            elog.WriteEntry("3");
            elog.Source = cs;
            elog.WriteEntry("4");
            elog.EnableRaisingEvents = true;
            elog.WriteEntry("5");
            try
            {

                elog.WriteEntry("6");
                FileInfo fInfo = new FileInfo(e.FullPath);
                elog.WriteEntry("7");
                while (IsFileLocked(fInfo))
                {
                    elog.WriteEntry("8");
                    Thread.Sleep(10000);
                }
                elog.WriteEntry("9");
                Vanrise.Fzero.Bypass.Import.ImportCDRs(e.FullPath, Path.GetDirectoryName(e.FullPath).Split('\\').ToList<String>().Last());
                elog.WriteEntry("10");
            }
            catch (Exception ex)
            {
                elog.WriteEntry("WatcherActivity " + ex.Message);
                elog.WriteEntry("WatcherActivity " + ex.InnerException);
                elog.WriteEntry("WatcherActivity " + ex.ToString());
                elog.WriteEntry("WatcherActivity " + ex.InnerException.StackTrace);
            }

        }

    }


}
