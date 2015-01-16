using System;
using System.Collections.Generic;
using System.Text;

namespace TABS
{
    /// <summary>
    /// An instance of a system Alert
    /// </summary>
    public class Alert : Extensibility.IAlert
    {
        protected long _ID;
        protected DateTime _Created;
        protected string _Source;
        protected string _Description;
        protected AlertLevel _Level;
        protected AlertProgress _Progress;
        protected bool _IsVisible;
        protected string _Tag;

        public virtual long ID { get { return _ID; } set { _ID = value; } }
        public virtual bool IsVisible { get { return _IsVisible; } set { _IsVisible = value; } }

        #region IAlert Members
        public virtual DateTime Created { get { return _Created; } set { _Created = value; } }
        public virtual string Source { get { return _Source; } set { _Source = value; } }
        public virtual string Description { get { return _Description; } set { _Description = value; } }
        public virtual AlertLevel Level { get { return _Level; } set { _Level = value; } }
        public virtual AlertProgress Progress { get { return _Progress; } set { _Progress = value; } }
        public string Tag { get { return _Tag; } set { _Tag = value; } }
        #endregion

        public Alert() { }

        public Alert(string source, string description, string tag, AlertLevel level, AlertProgress progress)
        {
            _Created = DateTime.Now;
            _Source = source;
            _Tag = tag;
            _Description = description;
            _Level = level;
            _Progress = progress;
            _IsVisible = true;
        }

        public Alert(string source, string description, string tag, AlertLevel level)
            : this(source, description, tag, level, AlertProgress.None)
        {
        }

        public static IList<Alert> SystemAlerts
        {
            get
            {
                IList<Alert> list = null;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    list = session.CreateQuery(
                                  String.Format(
                                      @"FROM Alert A WHERE 1=1 
                                                 AND A.IsVisible='Y' 
                                                 AND A.Source NOT LIKE 'PersistedAlertCriteria:%'  
                                                 ORDER BY A.Created, A.Level DESC"))
                                 .List<TABS.Alert>();
                    session.Clear();
                }
                return list;
            }
        }

        public string AlertDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss}] {1}: {2}", Created, Tag, Description);
                return sb.ToString();
            }
        }
    }
}
