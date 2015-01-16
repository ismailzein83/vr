using System;
using System.Collections.Generic;
using TABS.Addons.Utilities.ProxyCommon;
using System.Linq;

namespace TABS
{
    public class Switch : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {


        public override string Identifier { get { return "Switch:" + SwitchID; } }
        public static log4net.ILog log = log4net.LogManager.GetLogger(typeof(Switch));
        public static object DummyObject { get { return new object(); } }
        private List<ProxyState> _ProxyStates;

        internal static Dictionary<Switch, bool> _AvailableSwitchesToEdit;
        public static Dictionary<Switch, bool> AvailableSwitchesToEdit
        {
            get
            {
                if (_AvailableSwitchesToEdit == null)
                {
                    AvailableSwitchesToEdit = new Dictionary<Switch, bool>();
                    AvailableSwitchesToEdit = All.Values.ToDictionary(c => c, f => false);

                }

                return _AvailableSwitchesToEdit;
            }
            set
            {
                _AvailableSwitchesToEdit = value;
            }
        }
        public List<ProxyState> ProxyStates
        {
            get
            {
                //In case this is the first time this property is read
                if (_ProxyStates == null || _ProxyStates.Count == 0)
                {
                    _ProxyStates = Enable_Routing ? SwitchManager.GetProxyStates(this) : ProxyState.NotAvailable.ToList();
                    //Log the retrieved states
                    foreach (ProxyState state in _ProxyStates)
                    {
                        if (!string.IsNullOrEmpty(state.Error))
                            log.ErrorFormat("Error in {0} Proxy: {1}", Name, state.Error);
                        else
                            log.InfoFormat("Proxy {0} Status: {1}", Name, state.Status);
                    }
                }
                return _ProxyStates;
            }
            set
            {
                _ProxyStates = value;
            }
        }
        public ProxyState LastProxyState
        {
            get
            {
                return ProxyStates[ProxyStates.Count - 1];
            }
        }
        #region static
        internal static Dictionary<int, Switch> _All;
        public static Dictionary<int, Switch> All
        {
            get
            {
                lock (typeof(Switch))
                {

                    if (_All == null)
                    {
                        _All = ADOObjectAssembler.GetSwitchs();// ObjectAssembler.GetSwitches();
                    }
                }
                return _All;
            }
        }
        #endregion static

        #region Data Members

        private int _SwitchID;
        private string _Symbol;
        private string _Name;
        private string _LastCDRImportTag;
        private string _Description;
        private IDictionary<string, SwitchReleaseCode> _ReleaseCodes = new Dictionary<string, SwitchReleaseCode>();
        private Extensibility.SwitchConfigurationBase _Configuration = Extensibility.SwitchConfigurationBase.GetDefaultConfiguration();

        public virtual DateTime? LastImport { get; set; }
        public virtual DateTime? LastAttempt { get; set; }
        public virtual DateTime? LastRouteUpdate { get; set; }
        public virtual bool Enable_CDR_Import { get; set; }
        public virtual bool Enable_Routing { get; set; }

        public virtual IDictionary<string, SwitchReleaseCode> ReleaseCodes
        {
            get
            {
                return _ReleaseCodes;
            }
            set
            {
                _ReleaseCodes = value;
            }
        }

        public virtual int SwitchID
        {
            get { return _SwitchID; }
            set { _SwitchID = value; }
        }

        public virtual string Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }

        public virtual string LastCDRImportTag
        {
            get { return _LastCDRImportTag; }
            set { _LastCDRImportTag = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual int NominalTrunkCapacityInE1s { get; set; }
        public virtual int NominalVoipCapacityInE1s { get; set; }

        public virtual string SwitchManagerName { get { return _Configuration.SwitchManagerName; } set { _Configuration.SwitchManagerName = value; _SwitchManager = null; } }

        Extensibility.ISwitchManager _SwitchManager;
        public virtual Extensibility.ISwitchManager SwitchManager
        {
            get
            {
                if (_SwitchManager == null && SwitchManagerName != "")
                    _SwitchManager = TABS.Addons.AddonManager.GetSwitchManager(SwitchManagerName);
                return _SwitchManager;
            }
            set
            {
                _SwitchManager = value;
            }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public virtual Extensibility.SwitchConfigurationBase Configuration
        {
            get
            {
                if (_Configuration.GetType() == typeof(TABS.Extensibility.SwitchConfigurationBase) && SwitchManager != null)
                    _Configuration = SwitchManager.GetSwitchConfiguration(this, _Configuration.InnerXml);
                return _Configuration;
            }
        }

        public virtual System.Xml.XmlDocument ConfigurationXml
        {
            get { return (_Configuration == null) ? null : _Configuration; }
            set { if (_Configuration != null) _Configuration.InnerXml = (value != null) ? value.InnerXml : null; }
        }

        public virtual string ConfigurationXmlString
        {
            get { return (_Configuration == null) ? null : _Configuration.InnerXml; }
            set { if (_Configuration != null) _Configuration.InnerXml = value; }
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion Data Members


        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            //log.Info("Cleared Cached Switches Collection");
            //lock (typeof(Switch))
            //{
            //    _All = null;
            //    _AvailableSwitchesToEdit = null;
            //}
            //TABS.Components.CacheProvider.Clear(typeof(Switch).FullName);
        }
        public static void ClearSwitchesCollections()
        {
            log.Info("Cleared Cached Switches Collection");
            lock (typeof(Switch))
            {
                _All = null;
                _AvailableSwitchesToEdit = null;
            }
            TABS.Components.CacheProvider.Clear(typeof(Switch).FullName);
        }

        public override int GetHashCode()
        {
            return SwitchID.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Switch swt = obj as Switch;
            if (swt == null)
                return false;
            return this.SwitchID == swt.SwitchID;

        }

    }
}
