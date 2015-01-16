using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TABS
{
    /// <summary>
    /// A Persisted Runnable Task. A that can be persisted into the database and reloaded, when needed, at runtime.
    /// </summary>
    public class PersistedRunnableTask : Extensibility.IRunnableTask, NHibernate.Classic.ILifecycle
    {
        #region Members

        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.PersistedRunnableTask");

        protected int _ID;
        protected string _Name;
        protected bool _IsEnabled;
        protected bool? _IsLastRunSuccessful;
        protected Exception _Exception;
        protected System.Xml.XmlDocument _Configuration = GetConfigurationXmlTemplate();

        public virtual RunnableTaskType RunType
        {
            get { return (RunnableTaskType)Enum.Parse(typeof(RunnableTaskType), GetAttribute("RunType"), true); }
            set { SetAttribute("RunType", value.ToString()); }
        }

        static System.Text.RegularExpressions.Regex GroupParser = new System.Text.RegularExpressions.Regex(@"\s*(?<group>[^\[,\]]+)(?<Exclusive>\s*\[X\]){0,1}\s*", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.ExplicitCapture | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public virtual int ID { get { return _ID; } set { _ID = value; } }
        public virtual string Name { get { return _Name; } set { _Name = value; } }
        public virtual bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; } }
        public virtual string ConfigurationXmlString { get { return _Configuration.InnerXml; } set { _Configuration.InnerXml = value; } }
        public virtual string GroupingExpression { get; set; }

        public bool IsService { get; set; }
        public ServiceStatus? ServiceStatus { get; set; } 


        public virtual List<string> ExclusiveGroups
        {
            get
            {
                List<string> groups = new List<string>();
                if (GroupingExpression != null)
                {
                    foreach (System.Text.RegularExpressions.Match match in GroupParser.Matches(GroupingExpression))
                    {
                        if (match.Groups["Exclusive"].Success && match.Groups["Exclusive"].Value.Trim() == "[X]")
                            groups.Add(match.Groups["group"].Value.Trim());
                    }
                }
                return groups;
            }
        }

        public virtual List<string> Groups
        {
            get
            {
                List<string> groups = new List<string>();
                if (GroupingExpression != null)
                {
                    foreach (System.Text.RegularExpressions.Match match in GroupParser.Matches(GroupingExpression))
                    {
                        groups.Add(match.Groups["group"].Value.Trim());
                    }
                }
                return groups;
            }
        }

        /// <summary>
        /// Gets or Sets the Parameter Value, given the parameter name 
        /// </summary>
        /// <param name="name">Parameter Name</param>
        public virtual string this[string name] { get { return GetParameterValue(name); } set { SetParameterValue(name, value); } }

        /// <summary>
        /// The persisted configuration for this Runnable Task
        /// </summary>
        public virtual System.Xml.XmlDocument Configuration { get { return _Configuration; } set { _Configuration = value; } }

        #endregion Members

        #region IRunnableTask Members

        /// <summary>
        /// Get the manager for this task, in this case the system runnable tasks manager.
        /// </summary>
        public virtual Extensibility.IRunnableTaskManager Manager
        {
            get { return TABS.Components.TaskManager.Instance; }
            set { ; }
        }

        protected RunnableTaskSchedule _ScheduleType;
        public virtual RunnableTaskSchedule ScheduleType
        {
            get { return _ScheduleType; }
            set { _ScheduleType = value; }
        }

        protected TimeSpan? _TimeSpan;
        public virtual TimeSpan? TimeSpan { get { return _TimeSpan; } set { _TimeSpan = value; } }

        protected bool _IsRunning = false;
        public virtual bool IsRunning { get { return _IsRunning; } }

        protected bool _IsStopRequested = false;
        public virtual bool IsStopRequested { get { return _IsStopRequested; } }

        public virtual bool PossessesOwnThread
        {
            get { return _Thread != null; }
        }

        protected System.Threading.Thread _Thread;
        public virtual System.Threading.Thread Thread { get { return _Thread; } set { _Thread = value; } }

        protected DateTime? _LastRun;
        public virtual DateTime? LastRun { get { return _LastRun; } }

        protected TimeSpan? _LastRunDuration;
        public virtual TimeSpan? LastRunDuration
        {
            get
            {
                TimeSpan? value = _LastRunDuration;
                if (IsRunning && LastRun.HasValue)
                    value = DateTime.Now.Subtract(LastRun.Value);
                return value;
            }
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

        public string ExceptionString
        {
            get { return _Exception == null ? null : _Exception.ToString(); }
            set { if (value == null) _Exception = null; else _Exception = new Exception(value); }
        }

        protected DateTime? _NextRun;
        public virtual DateTime? NextRun { get { return _NextRun; } set { _NextRun = value; } }

        public virtual string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (IsRunning)
                {
                    if (_IRunnable != null)
                    {
                        return _IRunnable.Status;
                    }
                    else if (_CustomCodeRunner != null)
                    {
                        return _CustomCodeRunner.Status;
                    }
                    else
                        sb.AppendFormat("Running since {0}", LastRun);
                }
                else
                {
                    if (Exception == null)
                    {
                        sb.Append(IsEnabled ? "Enabled and Idle" : "Disabled");
                    }
                    else
                    {
                        sb.Append(Exception.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        public virtual bool Stop()
        {
            if (IsRunning)
            {
                _IsStopRequested = true;

                if (_IRunnable != null)
                {
                    _IRunnable.Stop();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Run the task
        /// </summary>
        /// <returns></returns>
        public virtual void Run()
        {
            lock (this)
            {
                // if not enabled or already running return false
                if (!IsEnabled) { log.InfoFormat("Cannot Run {0}, Task is Disabled", Name); return; }
                if (IsRunning) { log.InfoFormat("Cannot Run {0}, Task is already running", Name); return; }

                // Start running...
                _IsRunning = true;
            }

            bool result = false;
            _LastRun = DateTime.Now;
            _LastRunDuration = null;
            _Exception = null;

            try
            {
                switch (RunType)
                {
                    case RunnableTaskType.DatabaseCommand:
                        result = Run_DatabaseCommand();
                        break;
                    case RunnableTaskType.KnownIRunnableClass:
                        result = Run_KnownIRunnable();
                        break;
                    case RunnableTaskType.CustomCodeRunnableTask:
                        result = Run_CustomCode();
                        break;
                }
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                log.Error(this.Name + " was killed in a thread abort exception", tax);
            }
            finally
            {
                _NextRun = Components.TaskManager.CalculateNextRun(this);
                _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);
                _IsRunning = false;
                _IsStopRequested = false;
                _IsLastRunSuccessful = result;

                // Save information to the database...
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    NHibernate.ITransaction transaction = session.BeginTransaction();
                    try
                    {
                        session.SaveOrUpdate(this);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        log.Error(string.Format("Error Saving the Persisted Task {0}, ID: {1}", Name, ID), ex);
                        this.Exception = ex;
                    }
                }
            }
        }

        public virtual bool Abort()
        {
            if (IsRunning)
            {
                _IsStopRequested = false;
                _IsRunning = false;
                if (_IRunnable != null)
                {
                    _IRunnable.Stop();
                    _IRunnable.Abort();
                }
                return true;
            }
            return false;
        }

        #endregion

        protected IDbConnection GetDatabaseCommandConnection()
        {
            if (DbConnnectionString.Equals("SYSTEM", StringComparison.InvariantCultureIgnoreCase))
            {
                return DataHelper.GetOpenConnection();
            }
            else
            {
                IDbConnection connection = new System.Data.OleDb.OleDbConnection(DbConnnectionString);
                connection.Open();
                return connection;
            }
        }

        protected static System.Text.RegularExpressions.Regex DbCommandParamExtractor = new System.Text.RegularExpressions.Regex("[@]\\w+", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline);

        /// <summary>
        /// Run custom code
        /// </summary>
        /// <returns>Success state</returns>
        protected bool Run_CustomCode()
        {
            bool result = false;
            try
            {
                CustomCodeRunner.Run();
                result = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Run the Database Command Specified in this Task Parameters
        /// </summary>
        /// <returns></returns>
        protected bool Run_DatabaseCommand()
        {
            IDbConnection connection = GetDatabaseCommandConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = DbCommandText;
            command.CommandTimeout = DbCommandTimeOut;
            foreach (KeyValuePair<string, string> paramPair in Parameters())
            {
                IDbDataParameter param = command.CreateParameter();
                param.ParameterName = paramPair.Key;
                param.Value = paramPair.Value;
                command.Parameters.Add(param);
            }
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("** Error ** Running DatabaseCommand: {0}", Name), ex);
               

                Exception = ex;
            }

            return true;
        }

        protected Extensibility.IRunnable _IRunnable;

        /// <summary>
        /// Run a known task using the fully qualified class name (plus assembly)
        /// </summary>
        /// <returns></returns>
        protected bool Run_KnownIRunnable()
        {
            if (_IRunnable == null)
            {
                try
                {
                    _IRunnable = TABS.Addons.AddonManager.GetRunnable(KnownIRunnableClass);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("** Error ** Getting KnownIRunnable {0} for Task {1}", KnownIRunnableClass, Name), ex);
                    Exception = ex;
                    return false;
                }
            }
            if (_IRunnable != null)
            {
                try
                {
                    _IRunnable.Run();
                    return true;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    log.Error(string.Format("** Error ** Running KnownIRunnable {0} for Task {1}", KnownIRunnableClass, Name), ex);
                   
                    return false;
                }
            }
            else
                return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _IRunnable = null;
            _Thread = null;
        }

        #endregion

        /// <summary>
        /// Please do not use this Constructor because it is for internal use only.
        /// </summary>
        public PersistedRunnableTask()
        {

        }

        #region DatabaseCommand

        /// <summary>
        /// The Connection string for the Database Command. When SYSTEM is specified, the TABS general connection string is used.
        /// </summary>
        public string DbConnnectionString { get { return this["$DbConnnectionString"]; } set { this["$DbConnnectionString"] = value; } }

        /// <summary>
        /// The Command Timeout for the Database Command
        /// </summary>
        public int DbCommandTimeOut { get { int result = 0; int.TryParse(this["$DbCommandTimeOut"], out result); return result; } set { this["$DbCommandTimeOut"] = value.ToString(); } }

        /// <summary>
        /// The Command Timeout for the Database Command
        /// </summary>
        public string DbCommandText { get { return this["$DbCommandText"]; } set { this["$DbCommandText"] = value; } }

        #endregion DatabaseCommand

        #region KnownRunnableTask

        public string KnownIRunnableClass { get { return this["$KnownIRunnableClass"]; } set { this["$KnownIRunnableClass"] = value; } }

        #endregion KnownRunnableTask

        #region CustomCodeRunnableTask

        /// <summary>
        /// The CSharp Custom code to run in the Runtime created CustomCodeRunnableTask
        /// </summary>
        public string CustomCode { get { return this["$CustomCode"]; } set { if (this["$CustomCode"] != value) { this["$CustomCode"] = value; _CustomCodeRunner = null; } } }

        /// <summary>
        /// The CSharp Custom code Class Definitions 
        /// </summary>
        public string CustomCodeClassDefinitions { get { return this["$CustomCodeClassDefinitions"]; } set { if (this["$CustomCodeClassDefinitions"] != value) { this["$CustomCodeClassDefinitions"] = value; _CustomCodeRunner = null; } } }

        protected Extensibility.IRunnable _CustomCodeRunner;

        /// <summary>
        /// Gets
        /// </summary>
        protected Extensibility.IRunnable CustomCodeRunner
        {
            get
            {
                if (_CustomCodeRunner == null && this.RunType == RunnableTaskType.CustomCodeRunnableTask)
                {
                    _CustomCodeRunner = Components.CustomCodeRunner.Create(CustomCode, CustomCodeClassDefinitions);
                }
                return _CustomCodeRunner;
            }
        }

        #endregion CustomCodeRunnableTask

        /// <summary>
        /// Constructor for a Persisted, Runnable, Task!
        /// </summary>
        /// <param name="runType"></param>
        public PersistedRunnableTask(RunnableTaskType runType)
            : this()
        {
            // Set the runtype attribute
            SetAttribute("RunType", runType.ToString());
        }

        protected string GetAttribute(string name) { return _Configuration.DocumentElement.Attributes[name].Value; }
        protected void SetAttribute(string name, string value) { _Configuration.DocumentElement.Attributes[name].Value = value; }

        /// <summary>
        /// Gets the parameters node
        /// </summary>
        protected System.Xml.XmlNode ParametersNode { get { return Configuration.DocumentElement.SelectSingleNode("Parameters"); } }

        protected System.Xml.XmlNode GetParameter(string name, bool create)
        {
            System.Xml.XmlNode parameter = ParametersNode.SelectSingleNode("Parameter[@Name='" + name + "']");
            if (create && parameter == null)
            {
                parameter = GetParameter("NullParameter", false).CloneNode(true);
                parameter.Attributes["Name"].Value = name;
                ParametersNode.AppendChild(parameter);
            }
            return parameter;
        }

        protected void RemoveParameter(string name)
        {
            System.Xml.XmlNode parameter = GetParameter(name, false);
            if (parameter != null)
                ParametersNode.RemoveChild(parameter);
        }

        protected string GetParameterValue(string name)
        {
            System.Xml.XmlNode parameter = GetParameter(name, false);
            if (parameter == null) return null;
            else
                return parameter.FirstChild.Value;
        }

        protected System.Xml.XmlNode SetParameterValue(string name, string value)
        {
            System.Xml.XmlNode parameter = GetParameter(name, true);
            parameter.FirstChild.Value = value;
            return parameter;
        }

        /// <summary>
        /// Get the configration Xml Template
        /// </summary>
        /// <returns></returns>
        protected static System.Xml.XmlDocument GetConfigurationXmlTemplate()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.LoadXml(_ConfigurationXmlTemplate);
            return xmlDocument;
        }

        public void ClearParameters()
        {
            List<System.Xml.XmlNode> paramList = new List<System.Xml.XmlNode>();
            foreach (System.Xml.XmlNode node in ParametersNode.ChildNodes)
                if (!IsNullParameter(node))
                    paramList.Add(node);
            foreach (System.Xml.XmlNode node in paramList)
                ParametersNode.RemoveChild(node);
        }

        protected bool IsNullParameter(System.Xml.XmlNode paramNode)
        {
            return (paramNode.Name == "Parameter" && paramNode.Attributes["Name"].Value == "NullParameter");
        }

        public IEnumerable<KeyValuePair<string, string>> Parameters()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            foreach (System.Xml.XmlNode node in ParametersNode.ChildNodes)
            {
                if (!IsNullParameter(node))
                {
                    string name = node.Attributes["Name"].Value;
                    if (!name.StartsWith("$"))
                    {
                        KeyValuePair<string, string> pair = new KeyValuePair<string, string>(name, GetParameterValue(name));
                        parameters.Add(pair);
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// The Configuration XML Template Definition.
        /// </summary>
        protected static readonly string _ConfigurationXmlTemplate =
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>
              <PersistedRunnableTask RunType=""ClassDefined"">
                <Parameters>
                  <Parameter Name=""NullParameter""><![CDATA[]]></Parameter>
                </Parameters>
              </PersistedRunnableTask>";

        #region ILifecycle Members

        public NHibernate.Classic.LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            Manager.Tasks.Remove(this.Name);
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public void OnLoad(NHibernate.ISession s, object id)
        {

        }

        public NHibernate.Classic.LifecycleVeto OnSave(NHibernate.ISession s)
        {
            foreach (KeyValuePair<string, TABS.Extensibility.IRunnableTask> task in TABS.Components.TaskManager.Instance.Tasks)
                if (task.Value == this)
                {
                    TABS.Components.TaskManager.Instance.Remove(task.Key);
                    break;
                }
            TABS.Components.TaskManager.Instance.Add(this);
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public NHibernate.Classic.LifecycleVeto OnUpdate(NHibernate.ISession s)
        {
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        #endregion
    }
}