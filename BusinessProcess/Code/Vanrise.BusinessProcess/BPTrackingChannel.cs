using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Timers;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Data;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPTrackingChannel
    {
        #region Singleton

        static BPTrackingChannel()
        {
            _current = new BPTrackingChannel();
        }

        static BPTrackingChannel _current;
        public static BPTrackingChannel Current
        {
            get
            {
                return _current;
            }
        }

        #endregion

        IBPTrackingDataManager _trackingDataManager;
        Timer _timer;

        private BPTrackingChannel()
        {
            _trackingDataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            _timer = new Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }

        

        bool _isRunning;
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }

            try
            {
                BPTrackingMessage trackingMsg;
                List<BPTrackingMessage> lstTrackingMsgs = new List<BPTrackingMessage>();
                while (_qPendingTrackingMessages.TryDequeue(out trackingMsg))
                {
                    lstTrackingMsgs.Add(trackingMsg);
                    if (lstTrackingMsgs.Count >= 10000)
                    {
                        WriteToDB(lstTrackingMsgs);
                        lstTrackingMsgs.Clear();
                    }
                }
                if (lstTrackingMsgs.Count > 0)
                    WriteToDB(lstTrackingMsgs);
            }
            catch(Exception ex)
            {
                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
            }

            lock (this)
            {
                _isRunning = false;
            }
        }

        private void WriteToDB(List<BPTrackingMessage> lstTrackingMsgs)
        {
            _trackingDataManager.WriteTrackingMessagesToDB(lstTrackingMsgs);
        }

        ConcurrentQueue<BPTrackingMessage> _qPendingTrackingMessages = new ConcurrentQueue<BPTrackingMessage>();

        public void WriteTrackingMessage(BPTrackingMessage trackingMessage)
        {
            Console.WriteLine("{0}: {1}", trackingMessage.EventTime, trackingMessage.TrackingMessage);
            _qPendingTrackingMessages.Enqueue(trackingMessage);
        }

        public void WriteException(long processInstanceId, long? parentProcessId, Exception ex, bool isWorkflowAborted = true)
        {
            WriteTrackingMessage(new BPTrackingMessage
                {
                    ProcessInstanceId = processInstanceId,
                    ParentProcessId = parentProcessId,
                    Severity = isWorkflowAborted ? LogEntryType.Error : LogEntryType.Warning,
                    TrackingMessage = Common.Utilities.GetExceptionBusinessMessage(ex),
                    ExceptionDetail = ex.ToString(),
                    EventTime = DateTime.Now
                });
        }
    }
}
