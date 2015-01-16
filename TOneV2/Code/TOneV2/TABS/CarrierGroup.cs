using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    [Serializable]
    public class CarrierGroup : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public override string Identifier { get { return CarrierGroupID < 1 ? "CarrierGroup:" + CarrierGroupName : "CarrierGroup:" + CarrierGroupID.ToString(); } }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(CarrierGroup).FullName);
        }

        #region Data Members
        private static IDictionary<int, CarrierGroup> _All;
        protected IList<CarrierAccount> _Accounts;
        public virtual int CarrierGroupID { get; set; }
        public virtual string CarrierGroupName { get; set; }
        public virtual CarrierGroup ParentGroup { get; set; }

        public static IDictionary<int, CarrierGroup> All
        {
            get
            {
                if (_All == null)
                    _All = ObjectAssembler.GetAllCarrierGroups();
                return _All;
            }
        }

        public static string GetCommaSeperatedGroups(List<CarrierGroup> groupes)
        {
            if (groupes != null && groupes.Count > 0)
                return groupes.Select(k => k.CarrierGroupID.ToString()).Aggregate((id1, id2) => id1 + "," + id2);
            return string.Empty;
        }

        public IList<CarrierAccount> Accounts
        {
            get
            {
                if (_Accounts == null)
                    _Accounts = ObjectAssembler.GetAccountsofGroup(this);
                return _Accounts;
            }

        }

        public static IEnumerable<CarrierGroup> SupplierGroups
        {
            get
            {
                return TABS.CarrierGroup.All.Values
                                        .Where(c => c.Accounts
                                        .Any(cc => cc.AccountType == TABS.AccountType.Termination || cc.AccountType == TABS.AccountType.Exchange));
            }
        }

        public static IEnumerable<CarrierGroup> CustomerGroups
        {
            get
            {
                return TABS.CarrierGroup.All.Values
                                        .Where(c => c.Accounts
                                        .Any(cc => cc.AccountType == TABS.AccountType.Client || cc.AccountType == TABS.AccountType.Exchange));
            }
        }

        public virtual string ParentPath
        {
            get
            {
                // No parent?
                if (ParentGroup == null) return null;
                string greaterPath = ParentGroup.ParentPath;
                string parent = this.ParentGroup.CarrierGroupName;
                // Root Parent?
                if (greaterPath == null) return parent;
                // Non Root Parent
                else return string.Concat(greaterPath, "/", parent);
            }
            set
            {
                // Not Settable!
            }
        }

        public virtual string Path
        {
            get
            {
                // No parent?
                if (ParentPath == null) return CarrierGroupName;
                else return string.Concat(ParentPath, "/", CarrierGroupName);
            }
        }

        protected virtual string FormattedID { get { return this.CarrierGroupID.ToString("000000"); } }

        #endregion Data Members
    }
}
