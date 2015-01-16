using System;

namespace TABS.Addons.Runnables
{
    public abstract class RunnableBase : Extensibility.IRunnable
    {
        protected DateTime? _LastRun;
        protected TimeSpan? _LastRunDuration;
        protected bool _IsStopRequested = false;
        protected bool _IsRunning = false;
        protected bool? _IsLastRunSuccessful;
        protected Exception _Exception;

        #region IRunnable Members

        public abstract void Run();

        public virtual bool Stop()
        {
            if (IsRunning)
            {
                _IsStopRequested = true;
                return true;
            }
            else
                return false;
        }

        public virtual bool Abort()
        {
            if (IsRunning && IsStopRequested)
            {
                _IsStopRequested = false;
                _IsRunning = false;
                return true;
            }
            else
                return false;
        }

        public virtual bool IsRunning
        {
            get { return _IsRunning; }
            protected set { _IsRunning = value; }
        }

        public virtual bool? IsLastRunSuccessful
        {
            get { return _IsLastRunSuccessful; }
            set { _IsLastRunSuccessful = value; }
        }

        public Exception Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }

        public virtual DateTime? LastRun
        {
            get { return _LastRun; }
        }

        public virtual TimeSpan? LastRunDuration
        {
            get { return _LastRunDuration; }
        }

        public virtual bool IsStopRequested
        {
            get { return _IsStopRequested; }
        }

        public abstract string Status { get; }

        #endregion
    }
}
