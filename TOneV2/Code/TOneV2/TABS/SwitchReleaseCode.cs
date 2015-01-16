using System.Collections.Generic;

namespace TABS
{
    public class SwitchReleaseCode : Interfaces.ICachedCollectionContainer
    {
        public static void ClearCachedCollections()
        {
            lock (typeof(SwitchReleaseCode))
            {
                _All = null;
            }
            TABS.Components.CacheProvider.Clear(typeof(SwitchReleaseCode).FullName);
        }

        private string _IsoCode;
        private string _ReleaseCode;
        private string _Description;
        private bool _IsDelivered;
        private Switch _Switch;

        public virtual Switch Switch
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        public virtual string IsoCode
        {
            get { return _IsoCode; }
            set { _IsoCode = value; }
        }

        public virtual string ReleaseCode
        {
            get { return _ReleaseCode; }
            set { _ReleaseCode = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public virtual bool IsDelivered
        {
            get { return _IsDelivered; }
            set { _IsDelivered = value; }
        }

        public override bool Equals(object obj)
        {
            SwitchReleaseCode other = obj as SwitchReleaseCode;
            if (other != null)
            {
                return (this.Switch == other.Switch && this.ReleaseCode == other.ReleaseCode);
            }
            return false;
        }

        internal static Dictionary<Switch, Dictionary<string, SwitchReleaseCode>> _All;
        public static Dictionary<Switch, Dictionary<string, SwitchReleaseCode>> All
        {
            get
            {
                if (_All == null)
                {
                    var list = TABS.ObjectAssembler.GetList<SwitchReleaseCode>();
                    _All = new Dictionary<Switch, Dictionary<string, SwitchReleaseCode>>();
                    foreach (var switchreleasecode in list)
                    {
                        if (!_All.ContainsKey(switchreleasecode.Switch))
                            _All[switchreleasecode.Switch] = new Dictionary<string, SwitchReleaseCode>();
                        _All[switchreleasecode.Switch][switchreleasecode.ReleaseCode] = switchreleasecode;
                    }
                }
                return _All;
            }

        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Switch, ReleaseCode);
        }

        public string DisplayDescription
        {
            get
            {
                return string.Format("Switch: {0}, ReleaseCode: {1}, Description: {2} ,ISO: {3}", this.Switch, this.ReleaseCode, this.Description,this.IsoCode);
            }
        }
    }
}
