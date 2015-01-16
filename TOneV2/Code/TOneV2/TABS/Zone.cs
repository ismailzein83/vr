using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

namespace TABS
{
    [Serializable]
    public class Zone : Components.DateTimeEffectiveFlaggedServicesEntity, System.ComponentModel.IListSource, Interfaces.ICachedCollectionContainer, IComparable<TABS.Zone>
    {
        public override string Identifier { get { return "Zone:" + (ZoneID == 0 ? string.Format("<NEW-FROM:{0}>{1}", Supplier, Name) : ZoneID.ToString()); } }

        #region static


        public static Zone UndefinedZone { get { return TABS.Zone.OwnZones[-1]; } }

        internal static Dictionary<int, Zone> _OwnZones;

        /// <summary>
        /// Our own Zones (The zones we defined in our system)
        /// </summary>
        public static Dictionary<int, Zone> OwnZones
        {
            get
            {
                if (_OwnZones == null)
                {
                    _OwnZones = ObjectAssembler.GetOwnZones(DateTime.Now);
                }
                return _OwnZones;
            }
        }


        internal static Dictionary<int, Zone> _OwnZonesByDate;

        /// <summary>
        /// Our own Zones (The zones we defined in our system)
        /// </summary>
        public static Dictionary<int, Zone> OwnZonesByDate(DateTime when)
        {
            if (_OwnZonesByDate == null)
            {
                _OwnZonesByDate = ObjectAssembler.GetOwnZones(when);
            }
            return _OwnZonesByDate;

        }

        #endregion static

        #region Data Members

        protected int _ZoneID;
        protected string _Name;
        protected CarrierAccount _Supplier;
        protected CodeGroup _CodeGroup = null;
        protected string _IsMobile;
        protected string _IsProper;
        protected string _IsSold;
        public bool _IsCodeGroup = false;
        public bool IsCodeGroup
        {
            get { return _IsCodeGroup; }
            set { _IsCodeGroup = value; }
        }
        public bool _IsHaveMatchingCodeGroup = false;
        public bool IsHaveMatchingCodeGroup
        {
            get { return _IsHaveMatchingCodeGroup; }
            set { _IsHaveMatchingCodeGroup = value; }
        }
        public virtual int ZoneID
        {
            get { return _ZoneID; }
            set { _ZoneID = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual CodeGroup CodeGroup
        {
            get { return _CodeGroup; }
            set { _CodeGroup = value; }
        }

        public virtual bool IsMobile
        {
            get { return "Y".Equals(_IsMobile); }
            set { _IsMobile = value ? "Y" : "N"; }
        }

        public virtual bool IsProper
        {
            get { return "Y".Equals(_IsProper); }
            set { _IsProper = value ? "Y" : "N"; }
        }

        public virtual bool IsSold
        {
            get { return "Y".Equals(_IsSold); }
            set { _IsSold = value ? "Y" : "N"; }
        }


        #endregion Data Members

        #region Business Members

        protected IList<Code> _EffectiveCodes;
        protected IList<Rate> _EffectiveRates;

        public void RecalculateCodeGroup()
        {
            if (_EffectiveCodes != null && _EffectiveCodes.Count > 0)
                CodeGroup = CodeGroup.FindForCode(_EffectiveCodes[0].Value);
            else
            {
                if (this.ZoneID > 0)
                {
                    var FirstcodeValue = TABS.DataHelper.ExecuteScalar(string.Format(@"SELECT TOP 1 c.code FROM Code c WITH(NOLOCK) WHERE c.ZoneID= {0} 
                                                                                            AND c.IsEffective ='Y'
                                                                                            ORDER BY c.Code 
                                                                                                                ", this.ZoneID));
                    if (FirstcodeValue != null)
                        CodeGroup = CodeGroup.FindForCode(FirstcodeValue.ToString());
                    else
                        CodeGroup = CodeGroup.None;
                }
                else
                    CodeGroup = CodeGroup.None;
            }
        }

        /// <summary>
        /// The effective codes of this zone. Note that the system should reset this list 
        /// whenever a new pricelist (or updates in Codes) is triggered.
        /// </summary>
        public IList<Code> EffectiveCodes
        {
            get
            {
                if (_EffectiveCodes == null)
                {
                    //_EffectiveCodes = ObjectAssembler.GetEffectiveCodes(this, DateTime.Now);
                }
                return _EffectiveCodes;
            }
            set
            {
                _EffectiveCodes = value;
            }
        }

        /// <summary>
        /// The effective rate for this zone. Note that the system should reset this rate
        /// whenever a new pricelist is triggered.
        /// </summary>
        public IList<Rate> EffectiveRates
        {
            get
            {
                if (_EffectiveRates == null)
                {
                    //_EffectiveRates = ObjectAssembler.GetEffectiveRates(this, DateTime.Now);
                }
                return _EffectiveRates;
            }
            set
            {
                _EffectiveRates = value;
            }
        }

        /// <summary>
        /// Return the effective
        /// </summary>
        public Rate EffectiveRate
        {
            get
            {
                if (EffectiveRates.Count > 0 && EffectiveRates[0].PriceList.Supplier != CarrierAccount.SYSTEM)
                {
                    return null;// EffectiveRates[0];
                }
                else
                    throw new Exception("There is no single Effective Rate for the system account zones. A single effective rate exists for a zone for each customer.");
            }
            set
            {
                if (_EffectiveRates == null)
                {
                    _EffectiveRates = new List<Rate>();
                }
                EffectiveRates.Clear();
                EffectiveRates.Add(value);
            }
        }

        public string DisplayName
        {
            get
            {
                return EffectiveRate != null ? EffectiveRate.ToString() : this.Name;
            }
        }


        /// <summary>
        /// Returns a comma, dash-range seperated code list from the effective codes.
        /// Example: 96170, 9613, 9617-9619
        /// </summary>
        public string CodeRange
        {
            get
            {
                return string.Empty;// Code.GetCodeRange(EffectiveCodes);
            }
        }

        /// <summary>
        /// Returns the first effective code in this zone, null if none
        /// </summary>
        public Code FirstEffectiveCode
        {
            get
            {
                return null;// EffectiveCodes.Count > 0 ? EffectiveCodes[0] : null;
            }
        }

        /// <summary>
        /// Returns the first effective code value in this zone, null if none
        /// </summary>
        public string FirstEffectiveCodeValue
        {
            get
            {
                return string.Empty;// EffectiveCodes.Count > 0 ? EffectiveCodes[0].Value : null;
            }
        }

        public string GroupName { get { return CodeGroup.Name; } }


        protected IList<SpecialRequest> _EffectiveSpecialRequests;
        /// <summary>
        /// get a list of effective special requests for this zone 
        /// </summary>
        public IList<SpecialRequest> EffectiveSpecialRequests
        {
            get
            {
                if (_EffectiveSpecialRequests == null)
                {
                    //_EffectiveSpecialRequests =  ObjectAssembler.GetSpecialRequests(this, DateTime.Now);
                }
                return _EffectiveSpecialRequests;
            }
            set { _EffectiveSpecialRequests = value; }
        }

        protected IList<ToDConsideration> _EffectiveToDConsiderations;
        /// <summary>
        /// get the effective Tod Considerations for ths zone 
        /// </summary>
        public IList<ToDConsideration> EffectiveToDConsiderations
        {
            get
            {
                if (_EffectiveToDConsiderations == null)
                {
                    //_EffectiveToDConsiderations = ObjectAssembler.GetEffectiveToDConsiderations(this, DateTime.Now);
                }
                return _EffectiveToDConsiderations;
            }
            set { _EffectiveToDConsiderations = value; }
        }


        protected IList<RouteBlock> _EffectiveRouteBlocks;
        /// <summary>
        /// get the effective route blocks for this zone
        /// </summary>
        public IList<RouteBlock> EffectiveRouteBlocks
        {
            get
            {
                if (_EffectiveRouteBlocks == null)
                {
                    //_EffectiveRouteBlocks = ObjectAssembler.GetRouteBlocks(null, this, null, null, null, DateTime.Now);
                }
                return _EffectiveRouteBlocks;
            }
            set { _EffectiveRouteBlocks = value; }
        }



        protected IList<Commission> _EffectiveCommissions;
        /// <summary>
        /// get the effective commissions for this zone 
        /// </summary>
        public IList<Commission> EffectiveCommissions
        {
            get
            {
                if (_EffectiveCommissions == null)
                {
                    //_EffectiveCommissions = ObjectAssembler.GetEffectiveSalesCommisions(this, DateTime.Now);
                }
                return _EffectiveCommissions;
            }
            set { _EffectiveCommissions = value; }
        }


        protected IList<Tariff> _EffectiveTariffs;
        /// <summary>
        /// get the effective Tariffs for the selected zone 
        /// </summary>
        public IList<Tariff> EffectiveTariffs
        {
            get
            {
                if (_EffectiveTariffs == null)
                {
                    //_EffectiveTariffs = ObjectAssembler.GetZoneTariffs(this, DateTime.Now);
                }
                return _EffectiveTariffs;
            }
            set { _EffectiveTariffs = value; }
        }

        protected static Regex NameCleanerRegEx = new Regex("\\s+", RegexOptions.Compiled);

        /// <summary>
        /// Used to clean a zone name from unnecessary white space.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CleanName(string name)
        {
            if (name == null) return string.Empty;
            return NameCleanerRegEx.Replace(name, " ").Trim();
        }

        #endregion Business Members

        public override bool Equals(object obj)
        {
            Zone other = obj as Zone;
            if (other == null)
                return false;
            else
            {
                if (this.ZoneID == 0)
                {
                    if (this.Supplier != other.Supplier)
                        return false;
                    else
                        return this.Name == other.Name;
                }
                else
                    return this.ZoneID == other.ZoneID;
            }
        }
        #region IComparable<Zone> Members

        public int CompareTo(Zone other)
        {
            return (this.Name == null ? "" : this.Name).CompareTo(other.Name == null ? "" : other.Name);
        }
        #endregion
        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return _Name.GetHashCode();
        }

        #region ILifecycle Members

        public override NHibernate.Classic.LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            // If this is one of our own zones, remove it from the "Own Zones Collection"
            if (this.Supplier == null && _OwnZones != null)
            {
                _OwnZones.Remove(this.ZoneID);
            }
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        #endregion

        #region IListSource Members

        public bool ContainsListCollection
        {
            get { return false; }
        }

        public System.Collections.IList GetList()
        {
            return (IList)EffectiveCodes;
        }

        #endregion

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _OwnZones = null;
            TABS.Components.CacheProvider.Clear(typeof(Zone).FullName);
        }

        public override void Deactivate(bool isRecursive, DateTime when)
        {
            // Ending Zone 
            this.EndEffectiveDate = when;

            // Ending Codes 
            foreach (Code code in this.EffectiveCodes)
                code.Deactivate(true, when);

            //Ending Rate
            //this.EffectiveRate.Deactivate(true, when);


            //ending special requests 
            foreach (SpecialRequest req in this.EffectiveSpecialRequests)
                req.Deactivate(true, when);

            //ending ToDs 
            foreach (ToDConsideration tod in this.EffectiveToDConsiderations)
                tod.Deactivate(true, when);

            //ending route blocks 
            foreach (RouteBlock routeblock in EffectiveRouteBlocks)
                routeblock.Deactivate(true, when);

            //ending Commissions 
            foreach (Commission com in EffectiveCommissions)
                com.Deactivate(true, when);

            //ending the effective tariffs for this zone 
            foreach (Tariff tariff in EffectiveTariffs)
                tariff.Deactivate(true, when);

        }
    }
}