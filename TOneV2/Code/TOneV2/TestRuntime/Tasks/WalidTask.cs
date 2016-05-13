﻿using TOne.WhS.DBSync.Business;
using Vanrise.Runtime.Entities;

namespace TestRuntime.Tasks
{
    public class WalidTask : ITask
    {
        public void Execute()
        {
            //var runtimeServices = new List<RuntimeService>();
            //SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 2) };
            //runtimeServices.Add(schedulerService);
            //RuntimeHost host = new RuntimeHost(runtimeServices);
            //host.Start();


            DBSyncTaskActionArgument taskActionArgument = new DBSyncTaskActionArgument();
            taskActionArgument.ConnectionString = "Server=192.168.110.185;Database=MVTSPro;User ID=development;Password=dev!123";
            taskActionArgument.UseTempTables = false;
            DBSyncTaskAction dbSyncTaskAction = new DBSyncTaskAction();
            dbSyncTaskAction.Execute(new SchedulerTask(), taskActionArgument, null);
        }
    }
}




