using System;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace TABS
{
    /// <summary>
    /// A Persisted Alert Criteria
    /// </summary>
    public class PersistedAlertCriteria : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(PersistedAlertCriteria).FullName);
        }

        static Dictionary<int, PersistedAlertCriteria> _All;

        /// <summary>
        /// All the Persisted Alert Criteria defined in the System
        /// </summary>
        public static Dictionary<int, PersistedAlertCriteria> All
        {
            get
            {
                if (_All == null)
                {
                    IList<PersistedAlertCriteria> all = ObjectAssembler.GetList<PersistedAlertCriteria>();
                    _All = new Dictionary<int, PersistedAlertCriteria>(all.Count);
                    using (NHibernate.ISession session = DataConfiguration.OpenSession())
                    {
                        foreach (PersistedAlertCriteria alertCriteria in all)
                        {
                            _All[alertCriteria.ID] = alertCriteria;
                            LoadCriteriaAlerts(session, alertCriteria);
                        }
                    }
                }
                return _All;
            }
        }

        public static void LoadCriteriaAlerts(PersistedAlertCriteria alertCriteria)
        {
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                LoadCriteriaAlerts(session, alertCriteria);
            }
        }
        private static void LoadCriteriaAlerts(NHibernate.ISession session, PersistedAlertCriteria alertCriteria)
        {
            var alerts = new List<Extensibility.IAlert>();
            ((TABS.Addons.Alerts.GeneralAlertCriteria)alertCriteria.AlertCriteria).Alerts = alerts;
            var visibleAlerts =
                session.CreateCriteria(typeof(TABS.Alert))
                .Add(Expression.Eq("Source", string.Format("PersistedAlertCriteria:{0}", alertCriteria.ID)))
                .Add(Expression.Eq("IsVisible", true))
                .List<TABS.Alert>();
            foreach (var alert in visibleAlerts) alerts.Add(alert);
        }

        protected int _ID;
        protected string _ClassName;
        protected Extensibility.IAlertCriteria _AlertCriteria;

        static System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(TABS.Addons.Alerts.GeneralAlertCriteria));

        /// <summary>
        /// The serialization info necessary to persist the AlertCriteria. (Criteria, State, etc...)
        /// </summary>
        public virtual string SerializationInfo
        {
            get
            {
                if (_AlertCriteria == null) return null;
                _AlertCriteria.Source = this.Identifier;
                System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                xmlSerializer.Serialize(stringWriter, _AlertCriteria);
                return stringWriter.ToString();
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _AlertCriteria = null;
                    _ClassName = null;
                }
                else
                {
                    System.IO.StringReader stringReader = new System.IO.StringReader(value);
                    Extensibility.IAlertCriteria alertCriteria = (Extensibility.IAlertCriteria)xmlSerializer.Deserialize(stringReader);
                    _ClassName = alertCriteria.GetType().FullName;
                    _AlertCriteria = alertCriteria;
                    if (_AlertCriteria.Source == null)
                        _AlertCriteria.Source = this.Identifier;
                }
            }
        }

        public virtual int ID { get { return _ID; } set { _ID = value; } }
        public virtual string ClassName { get { return _ClassName; } set { _ClassName = value; } }
        public Extensibility.IAlertCriteria AlertCriteria { get { return _AlertCriteria; } set { _AlertCriteria = value; _ClassName = (value != null) ? _AlertCriteria.GetType().FullName : null; } }
        public virtual bool? IsEnabled { get { return (_AlertCriteria == null) ? null : (bool?)_AlertCriteria.IsEnabled; } set { if (_AlertCriteria != null) _AlertCriteria.IsEnabled = value.HasValue ? value.Value : false; } }
        public virtual DateTime? Updated { get { return (_AlertCriteria == null) ? null : _AlertCriteria.LastChecked; } set { if (_AlertCriteria != null) _AlertCriteria.LastChecked = value; } }
        public virtual string Tag { get { return (_AlertCriteria == null) ? null : _AlertCriteria.Tag; } set { if (_AlertCriteria != null) _AlertCriteria.Tag = value; } }
        //public virtual int? AlertingRunCount { get { return (_AlertCriteria == null) ? null : _AlertCriteria.AlertingRunCount; } set { if (_AlertCriteria != null) _AlertCriteria.AlertingRunCount = value; } }
        public virtual TimeSpan? AlertingTimeSpan { get { return (_AlertCriteria == null) ? null : _AlertCriteria.AlertingTimeSpan; } set { if (_AlertCriteria != null) _AlertCriteria.AlertingTimeSpan = value; } }

        /// <summary>
        /// return Summary of Criteria for the Current Alert
        /// </summary>
        public string FiltersSummary
        {
            get
            {
                return AlertCriteria == null ? "" : AlertCriteria.FiltersSummary;
            }
        }

        /// <summary>
        /// return Summary of State for the Current Alert 
        /// </summary>
        public string StateSummary
        {
            get
            {
                return AlertCriteria == null ? "" : AlertCriteria.StateSummary;
            }
        }

        public override string Identifier
        {
            get
            {
                return "PersistedAlertCriteria:" + (_ID == 0 ?
                    this.AlertCriteria.FiltersSummary + this.AlertCriteria.ThresholdsSummary :
                    _ID.ToString());
            }
        }
    }
}
