using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Util;
//using NHibernate.Util.LRUMap;
//using NHibernate.Util.SoftLimitMRUCache;
namespace TABS
{
    public class DataConfiguration : NHibernate.Cfg.Configuration, IDisposable
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DataConfiguration));
        public static System.Security.Principal.WindowsIdentity ApplicationIdentity;
        public static event Action<DataConfiguration> Initializing;
        public static event Action<DataConfiguration> Initialized;
        internal static string CurrentSessionKey = "nhibernate.current_session";
        internal static object lockobj = new object();
        /// <summary>
        /// Return an NHibernate open Session
        /// </summary>
        /// <returns></returns>
        public static NHibernate.ISession OpenSession()
        {
            // return Default.SessionFactory.OpenSession(new TABS.Components.Interceptor());
            NHibernate.ISession session = Default.SessionFactory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }

        internal static Dictionary<System.Threading.Thread, NHibernate.ISession> ThreadSessions = new Dictionary<System.Threading.Thread, ISession>();

        /// <summary>
        /// The current "Context" Session for NHibernate.
        /// - For web development, it starts (on demand) and ends with each request
        /// - It is a "statically" open session otherwise.
        /// </summary>
        public static NHibernate.ISession CurrentSession
        {
            get
            {
                //newly added using session key not thread
                //System.Threading.Thread.CurrentThread.ExecutionContext.
                //System.Web.HttpContext context = System.Web.HttpContext.Current;
                //ISession currentSession = context.Items[CurrentSessionKey] as ISession;
                //lock (lockobj)
                //{
                //    if (currentSession == null)
                //    {

                //        currentSession = OpenSession();
                //        context.Items[CurrentSessionKey] = currentSession;
                //    }
                //}
                //return currentSession;
                //end newly added

                if (!TABS.Components.HybridWebSessionContext.HasBind(TABS.DataConfiguration.Default.SessionFactory))
                    TABS.Components.HybridWebSessionContext.Bind(TABS.DataConfiguration.Default.SessionFactory.OpenSession());

                //if (!NHibernate.Context.CurrentSessionContext.HasBind(TABS.DataConfiguration.Default.SessionFactory))
                //    NHibernate.Context.CurrentSessionContext.Bind(TABS.DataConfiguration.Default.SessionFactory.OpenSession());

                return TABS.DataConfiguration.Default.SessionFactory.GetCurrentSession();
                NHibernate.ISession session = null;
                lock (ThreadSessions)
                {
                    System.Threading.Thread currentThread = System.Threading.Thread.CurrentThread;
                    if (!ThreadSessions.TryGetValue(currentThread, out session))
                    {
                        session = OpenSession();
                        ThreadSessions[currentThread] = session;
                    }
                }
                return session;
            }
        }

        public class SessionHandler : System.Web.SessionState.IRequiresSessionState
        {
            public SessionHandler() { }

            public object GetSession()
            { object session = System.Web.HttpContext.Current.Session[CurrentSessionKey]; return session; }
            public void SetSession(object data)
            { System.Web.HttpContext.Current.Session.Add(CurrentSessionKey, data); }

        }
        public static NHibernate.ISession CurrentSessionRequest
        {
            get
            {
                SessionHandler helper = new SessionHandler();
                return helper.GetSession() as NHibernate.ISession;

            }
        }
        /// <summary>
        /// Kill dead threads sessions
        /// </summary>
        protected void ThreadSessionKiller()
        {
            TimeSpan aMinute = TimeSpan.FromMinutes(1);
            while (!IsDisposed)
            {
                System.Threading.Thread.Sleep(aMinute);
                SessionFactory.EvictQueries();
                GC.Collect();
                //reused for plugin issue using session key for now
                lock (ThreadSessions)
                {
                    List<System.Threading.Thread> deadThreads = new List<System.Threading.Thread>();
                    foreach (System.Threading.Thread thread in ThreadSessions.Keys)
                        if (thread.ThreadState == System.Threading.ThreadState.Aborted || thread.ThreadState == System.Threading.ThreadState.Stopped)
                            deadThreads.Add(thread);
                    foreach (System.Threading.Thread thread in deadThreads)
                    {
                        NHibernate.ISession session = ThreadSessions[thread];
                        ThreadSessions.Remove(thread);
                        // if (session.IsDirty()) session.Flush();
                        if (session.IsOpen)
                        {
                            session.Clear();
                            session.Close();
                        }
                        session.Dispose();
                        session = null;
                    }
                }
            }

        }

        /// <summary>
        /// The default configuration (static accross application domain)
        /// </summary>
        protected static DataConfiguration _Default;

        /// <summary>
        /// The session factory associtated with this (instance) configuration...
        /// </summary>
        protected NHibernate.ISessionFactory _SessionFactory;

        /// <summary>
        /// The Default Configuration, a static instance that is used (by default)
        /// for security objects creation and update.
        /// You should always set this instance to **YOUR** configuration especially 
        /// if the table names you ar using are different.
        /// </summary>
        public static DataConfiguration Default
        {
            get
            {
                lock (typeof(DataConfiguration))
                {
                    if (_Default == null)
                    {
                        _Default = new DataConfiguration();

                        if (Initialized != null)
                        {
                            foreach (var invoker in Initialized.GetInvocationList())
                            {
                                try
                                {
                                    invoker.DynamicInvoke(_Default);
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Error invoking Initialized Event for DataConfiguration on " + invoker, ex);
                                }
                            }
                        }
                    }
                }
                return _Default;
            }
        }

        /// <summary>
        /// Sets or gets the default Session Factory for this configuration
        /// </summary>
        public NHibernate.ISessionFactory SessionFactory
        {
            get
            {
                lock (typeof(DataConfiguration))
                {
                    if (_SessionFactory == null)
                    {
                        try
                        {
                            _SessionFactory = Default.BuildSessionFactory();
                            log.Error("Session Factory Initialized");

                        }
                        catch (Exception ex)
                        {
                            if (_SessionFactory == null)
                                throw ex;
                            else
                                log.Error("Error Initializing Session Factory", ex);
                        }
                    }
                }
                return _SessionFactory;
            }
            set
            {
                _SessionFactory = value;
            }
        }

        public DataConfiguration()
        {
            log.Info("DataConfiguration Initialized");

            // Use the reflection optimizer
            NHibernate.Cfg.Environment.UseReflectionOptimizer = true;

            // Add security essentials
            this.AddAssembly(typeof(SecurityEssentials.User).Assembly);

            // Add the TABS assmbly (and its embedded HBM files)
            this.AddAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            if (Initializing != null)
            {
                foreach (var invoker in Initializing.GetInvocationList())
                {
                    try
                    {
                        log.InfoFormat("DataConfiguration Invoking {0}", invoker);
                        invoker.DynamicInvoke(this);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Initializing DataConfiguration", ex);
                    }
                }
            }
            else
            {
                log.Info("DataConfiguration Has no Subscribers Registered for Initializing Event");
            }

            string connectionKey = "connection.connection_string";//hibernate.
            string commanttimeoutKey = "hibernate.command_timeout";

            // Decode Connection String if necessary
            if (ConnectionStringEncrypted)
            {
                string connectionString = this.GetProperty(connectionKey);
                this.SetProperty(connectionKey, WebHelperLibrary.Utility.SimpleDecode(connectionString));
                SecurityEssentials.Configuration.Default.SetProperty(connectionKey, WebHelperLibrary.Utility.SimpleDecode(connectionString));
            }
            //this.SetProperty(commanttimeoutKey, "300");
            //SecurityEssentials.Configuration.Default.SetProperty(commanttimeoutKey, "300");

            // Start the Dead Session Killer
            //System.Threading.Thread deadSessionKiller = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadSessionKiller));
            //deadSessionKiller.Start();
        }

        public static bool? _ConnectionStringEncrypted;

        public static bool ConnectionStringEncrypted
        {
            get
            {
                lock (typeof(DataConfiguration))
                {
                    if (!_ConnectionStringEncrypted.HasValue)
                    {
                        _ConnectionStringEncrypted = TABS.Components.NHibernateHelperModule.IsConnectionLicenceEncrypted();
                    }
                    return _ConnectionStringEncrypted.Value;
                }
            }
        }

        #region IDisposable Members

        public bool IsDisposed { get; protected set; }

        public void Dispose()
        {
            _SessionFactory.Close();
            IsDisposed = true;
        }

        #endregion
    }
}
