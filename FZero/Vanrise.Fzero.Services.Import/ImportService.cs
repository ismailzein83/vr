﻿using System;
using System.Collections.Generic;
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
            //Debugger.Launch(); // launch and attach debugger
            watch();
        }


        private void watch()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["UploadPath"]; 
            watcher.NotifyFilter =  NotifyFilters.FileName ;
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
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            try
            {
              

                FileInfo fInfo = new FileInfo(e.FullPath); 
                    while(IsFileLocked(fInfo)){
                         Thread.Sleep(10000);     
                    }
                    FMSServiceReference.FzeroServiceClient myService = new FMSServiceReference.FzeroServiceClient();
                myService.Import(e.FullPath, Path.GetDirectoryName(e.FullPath).Split('\\').ToList<String>().Last());
                myService.Close();

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
