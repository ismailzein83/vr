using System;
using System.Collections.Generic;

namespace TABS.Addons.Utilities
{
    public abstract class SwitchJob
    {
        public const string JOB_TERMINATION_STRING = "::::";
        public const string JOB_MEMBER_SEPERATOR = "|||";
        public int SwitchID { get; set; }
        public string AccountID { get; set; }
        protected abstract void Initialize(string jobString);
        public abstract void Run();
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        
        /// <summary>
        /// This Factory method creates a job of the right type from a given serialization string.
        /// </summary>
        /// <param name="jobString"></param>
        /// <returns></returns>
        public static SwitchJob Create(string jobString)
        {
            string typeName = jobString.Substring(0, jobString.IndexOf("|||"));
            SwitchJob job = (SwitchJob)
                System.Type.GetType(typeName)
                .GetConstructor(new Type[] { })
                .Invoke(new object[] { });
            job.InitInternal(jobString);
            job.Initialize(jobString);

            return job;
        }
        
        /// <summary>
        /// Initialize the job from the provided string.
        /// </summary>
        /// <param name="jobString"></param>
        protected void InitInternal(string jobString)
        {
            string[] parts = jobString.Split(new string[] { JOB_MEMBER_SEPERATOR }, StringSplitOptions.None);

            this.SwitchID = int.Parse(parts[1]);
            this.AccountID = parts[2];
            this.SuccessMessage = parts[3];
            this.ErrorMessage = parts[4];
        }
        
        public override string ToString()
        {
            return string.Format("{0}|||{1}|||{2}|||{3}|||{4}", this.GetType(), this.SwitchID, this.AccountID, SuccessMessage, ErrorMessage);
        }
        
        public override bool Equals(object obj)
        {
            SwitchJob other = obj as SwitchJob;
            if (other == null) return false;
            return this.ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    public class SimpleSwitchJob : SwitchJob
    {
        public SimpleSwitchJob()
        {
            SwitchID = -1;
        }

        protected override void Initialize(string jobString)
        {
            //Nothing here
        }
        public override string ToString()
        {
            return base.ToString() + JOB_TERMINATION_STRING;
        }
        public override void Run()
        {
            return;
        }
    }

    public class RouteStatusSwitchJob : SwitchJob
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(RouteStatusSwitchJob));
        
        public string OperationType { get; set; }        

        protected override void Initialize(string jobString)
        {
            int baseStringLength = base.ToString().Length + JOB_MEMBER_SEPERATOR.Length;
            int membersLength = jobString.Length - baseStringLength;
            OperationType = jobString.Substring(baseStringLength, membersLength);
        }

        public override string ToString()
        {
            return base.ToString() + JOB_MEMBER_SEPERATOR + OperationType + JOB_TERMINATION_STRING;
        }

        public override void Run()
        {
            try
            {
                Switch s = Switch.All[SwitchID];
                CarrierAccount account = CarrierAccount.All[AccountID];
                bool success = s.SwitchManager.UpdateAccountRoutingStatus(s, account);
                if (!string.IsNullOrEmpty(SuccessMessage))
                    log.Info(SuccessMessage + success.ToString());
                if (success)
                    SwitchJobQueueHandler.DeleteJob(this);

            }
            catch (Exception ex)
            {
                log.Error(ErrorMessage, ex);
            }
        }

        public override bool Equals(object obj)
        {
            RouteStatusSwitchJob other = obj as RouteStatusSwitchJob;
            if (other == null) return false;
            return base.Equals(obj) && this.OperationType == other.OperationType;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SwitchJobQueueHandler
    {
        /// <summary>
        /// Add the Job to the queue of jobs to be executed by the SwitchJobRunner
        /// </summary>
        /// <param name="job"></param>
        public static void QueueSwitchJob(SwitchJob job)
        {
            var param = TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Switch_Job_Queue_String];
            lock (param)
            {
                param.TextValue += job.ToString();
                using (var session = DataConfiguration.OpenSession())
                {
                    session.Update(param);
                    session.Clear();
                    session.Flush();
                }
            }
        }
        /// <summary>
        /// Get the job at the head of the queue
        /// </summary>
        /// <returns></returns>
        public static List<SwitchJob> Jobs
        {
            get
            {
                List<SwitchJob> result = new List<SwitchJob>();
                var param = TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Switch_Job_Queue_String];
                lock (param)
                {
                    string jobQueue = param.TextValue;
                    string[] saJobs = jobQueue.Split(new string[] { SwitchJob.JOB_TERMINATION_STRING }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string sJob in saJobs)
                        result.Add(SwitchJob.Create(sJob));
                }
                return result;
            }
        }

        public static void DeleteJob(SwitchJob job)
        {
            var param = TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Switch_Job_Queue_String];
            lock (param)
            {
                string jobQueue = param.TextValue;
                string[] jobs = jobQueue.Split(new string[] { SwitchJob.JOB_TERMINATION_STRING }, StringSplitOptions.RemoveEmptyEntries);
                int jobCounter = 0;
                for (; jobCounter < jobs.Length; jobCounter++)
                {
                    SwitchJob otherJob = SwitchJob.Create(jobs[jobCounter]);
                    if (job.Equals(otherJob))
                        break;
                }
                if (jobCounter <= jobs.Length)
                {
                    string toRemove = jobs[jobCounter] + "::::";
                    int indexStart = jobQueue.IndexOf(toRemove);
                    int indexEnd = indexStart + toRemove.Length;
                    jobQueue = jobQueue.Substring(0, indexStart) + jobQueue.Substring(indexEnd);
                    SystemParameter queueParam =
                        TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.Switch_Job_Queue_String];
                    queueParam.TextValue = jobQueue;
                    using (var session = DataConfiguration.OpenSession())
                    {
                        session.Update(queueParam);
                        session.Flush();
                        session.Clear();
                    }
                }
            }
        }

    }
}
