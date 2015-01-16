using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class CodeGroup : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public static readonly CodeGroup None = CodeGroup.All.ContainsKey("-") ? All["-"] : new CodeGroup { _Code = "-", _Name = "Unknown" };
        public static readonly CodeGroup Local = CodeGroup.All.ContainsKey("0") ? All["0"] : new CodeGroup { _Code = "0", _Name = "Local" };

        internal static Dictionary<string, CodeGroup> _All;

        /// <summary>
        /// All the code groups
        /// </summary>
        public static Dictionary<string, CodeGroup> All
        {
            get
            {
                if (_All == null)
                {
                    ADOObjectAssembler ADOObjectAssembler = new ADOObjectAssembler();
                    _All = ADOObjectAssembler.GetCodeGroups();
                    //_All = ObjectAssembler.GetCodeGroups();
                }
                return _All;
            }
        }

        internal static Dictionary<string, string> _ZoneIDs;
        /// <summary>
        /// Code Groups with Zone IDs in comma seperated
        /// </summary>
        public static Dictionary<string, string> ZoneIDs
        {
            get
            {
                if (_ZoneIDs == null)
                {
                    _ZoneIDs = new Dictionary<string, string>();
                    foreach (TABS.Zone zone in TABS.Zone.OwnZones.Values)
                    {
                        string key = zone.CodeGroup.Code;
                        if (_ZoneIDs.Keys.Contains(key))
                            _ZoneIDs[key] = string.Concat(_ZoneIDs[key], ",", zone.ZoneID.ToString());
                        else _ZoneIDs[key] = zone.ZoneID.ToString();
                    }
                }
                return _ZoneIDs;
            }
        }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            _ZoneIDs = null;
            TABS.Components.CacheProvider.Clear(typeof(CodeGroup).FullName);
        }

        private string _Code;
        private string _Name;
        public virtual string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string DisplayName
        {
            get { return string.Format("{0} ({1})", _Name, _Code); }
        }

        public override string Identifier
        {
            get { return "CodeGroup: " + Code; }
        }

        /// <summary>
        /// Find the corresponding CodeGroup for a given code.
        /// Returns CodeGroup.None if not found.
        /// </summary>
        /// <param name="code">The given code</param>
        /// <returns>The code group or CodeGroup.None if not found.</returns>
        public static CodeGroup FindForCode(string code)
        {
            CodeGroup result = CodeGroup.None;
            if (code != null)
            {
                StringBuilder subCode = new StringBuilder(code.Trim());
                while (subCode.Length > 0)
                {
                    if (All.TryGetValue(subCode.ToString(), out result))
                        break;
                    subCode.Length--;
                }
            }
            if (result == null) result = CodeGroup.None;
            return result;
        }

        /// <summary>
        /// override equals 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            CodeGroup other = obj as CodeGroup;
            if (other != null)
                return this.Name.Equals(other.Name);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }

        /// <summary>
        /// Deny saving the "None" CodeGroup.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override NHibernate.Classic.LifecycleVeto OnSave(NHibernate.ISession s)
        {
            if (this == CodeGroup.None) return NHibernate.Classic.LifecycleVeto.Veto;
            else return base.OnSave(s);
        }
    }
}
