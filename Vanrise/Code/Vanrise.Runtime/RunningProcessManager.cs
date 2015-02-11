using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class RunningProcessManager
    {
        static RunningProcessManager()
        {
            _dataManager = RuntimeDataManagerFactory.GetDataManager<IRunningProcessDataManager>();
            s_Timer = new Timer(5000);
            s_Timer.Elapsed += s_Timer_Elapsed;
            s_Timer.Enabled = true;
        }

        static void s_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                UpdateHeartBeat();
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            lock (s_lockObj)
                s_isRunning = false;
        }

        private static void UpdateHeartBeat()
        {
            lock (s_lockObj)
            {
                if (s_isRunning)
                    return;
                s_isRunning = true;
            }

            if (_currentProcess == null)
                InitializeCurrentProcessIfNotInitialized();
            else
            {
                DateTime heartBeatTime;
                if(!_dataManager.UpdateHeartBeat(_currentProcess.ProcessId, out heartBeatTime))
                {
                    var processInfo = _dataManager.InsertProcessInfo(System.Diagnostics.Process.GetCurrentProcess().ProcessName, Environment.MachineName);
                    heartBeatTime = processInfo.LastHeartBeatTime;
                    lock(s_lockObj)
                    {
                        if (_currentProcess == null)
                            _currentProcess = processInfo;
                        else
                        {
                            _currentProcess.ProcessId = processInfo.ProcessId;
                            _currentProcess.StartedTime = processInfo.StartedTime;                            
                        }
                    }
                }
                _currentProcess.LastHeartBeatTime = heartBeatTime;
            }

            //delete timed out processes
            if ((DateTime.Now - s_lastCleanTime).TotalMinutes > 10)
            {
                _dataManager.DeleteTimedOutProcesses(new TimeSpan(0, 2, 0));
                s_lastCleanTime = DateTime.Now;
            }

            lock (s_lockObj)
                s_isRunning = false;
        }

        static object s_lockObj = new object();
        static bool s_isRunning;
        static RunningProcessInfo _currentProcess;
        static IRunningProcessDataManager _dataManager;
        static Timer s_Timer;
        static DateTime s_lastCleanTime;

        public static RunningProcessInfo CurrentProcess
        {
            get
            {
                if(_currentProcess == null)
                {
                    InitializeCurrentProcessIfNotInitialized();
                }
                return _currentProcess;
            }
        }

        private static void InitializeCurrentProcessIfNotInitialized()
        {
            lock(s_lockObj)
            {
                if (_currentProcess == null)
                    _currentProcess = _dataManager.InsertProcessInfo(System.Diagnostics.Process.GetCurrentProcess().ProcessName, Environment.MachineName);
            }
        }

        public List<RunningProcessInfo> GetRunningProcesses(TimeSpan? heartBeatReceivedWithin = null)
        {
            UpdateHeartBeat();
            return _dataManager.GetRunningProcesses(heartBeatReceivedWithin);
        }

        static List<RunningProcessInfo> s_runningProcesses;
        static DateTime s_RunningProcessesRetrievedTime;

        public List<RunningProcessInfo> GetCachedRunningProcesses(TimeSpan maxCacheTime)
        {
            if ((DateTime.Now - s_RunningProcessesRetrievedTime) > maxCacheTime)
            {
                lock (s_lockObj)
                {
                    if ((DateTime.Now - s_RunningProcessesRetrievedTime) > maxCacheTime)
                    {
                        s_runningProcesses = GetRunningProcesses();
                        s_RunningProcessesRetrievedTime = DateTime.Now;
                    }
                }
            }
            return s_runningProcesses;
        }
    }
}
