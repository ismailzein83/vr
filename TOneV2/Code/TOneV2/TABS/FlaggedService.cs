using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TABS
{
    [Serializable]
    public class FlaggedService : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(FlaggedService).FullName);
        }

        public static TABS.FlaggedService Default { get { return TABS.FlaggedService.All.Values.First(f => f.Symbol.Trim() == "WHS"); } }

        public override string Identifier { get { return "FlaggedService:" + FlaggedServiceID; } }

        //public static readonly FlaggedService Retail = GetByName("Retail");
        //public static readonly FlaggedService Premium = GetByName("Premium");
        //public static readonly FlaggedService CLI = GetByName("CLI");
        //public static readonly FlaggedService Video = GetByName("Video");
        //public static readonly FlaggedService Direct = GetByName("Direct");
        //public static readonly FlaggedService Transit = GetByName("Transit");
        //public static readonly FlaggedService ThreeGM = GetByName("3GM Mobile Service");

        internal static Dictionary<short, FlaggedService> _All;

        public static Dictionary<short, FlaggedService> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {
                    if (_All == null)
                    {
                        _All = ObjectAssembler.GetAllFlaggedServices();
                    }
                }
                return _All;
            }
            set
            {
                _All = value;
            }
        }


        private short _FlaggedServiceID;
        private string _Symbol;
        private string _Name;
        private string _Description;

        public virtual short FlaggedServiceID
        {
            get { return _FlaggedServiceID; }
            set { _FlaggedServiceID = value; }
        }

        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public virtual string Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public override bool Equals(object obj)
        {
            FlaggedService other = obj as FlaggedService;
            if (other != null)
                return other.FlaggedServiceID == this.FlaggedServiceID;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return FlaggedServiceID.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol;
        }

        public static short UpdateServicesFlag(Iesi.Collections.Generic.ISet<FlaggedService> Services)
        {
            short servicesFlag = 0;
            foreach (FlaggedService service in Services)
                servicesFlag = (short)(servicesFlag | service.FlaggedServiceID);
            return servicesFlag;
        }

        public static void UpdateServicesFromFlag(short ServicesFlag, Components.NotifierSet<FlaggedService> services)
        {
            services.AllowNotifications = false;
            services.Clear();
            foreach (FlaggedService service in FlaggedService.All.Values)
                if ((service.FlaggedServiceID & ServicesFlag) == service.FlaggedServiceID)
                    services.Add(service);
            services.AllowNotifications = true;
        }

        public static FlaggedService GetByName(string name)
        {
            foreach (FlaggedService service in All.Values)
                if (service.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return service;
            return null;
        }

        public static FlaggedService GetBySymbol(string symbol)
        {
            return All.Values.Where(s => s.Symbol.Equals(symbol.Trim())).DefaultIfEmpty(null).SingleOrDefault();
        }

        public static short GetBySymbolCommaSeperated(string symbolCommaSeperated)
        {
            short services = 0;
            MatchCollection matchCollection = Regex.Matches(symbolCommaSeperated, "[^WHS]\\w+");
            int count = matchCollection.Count;
            if (count > 0)
            {
                foreach (Match match in matchCollection)
                {
                    FlaggedService ser = GetBySymbol(match.Value.Replace(",", ""));
                    if (ser != null)
                        services = (short)(services | ser.FlaggedServiceID);
                }
            }
            return services;
        }

        /// <summary>
        /// Get a table containing the valid combinations of services flags and their masks.
        /// The mask (M) of a service flag (F) is actually defined as follows:
        /// (M) is any service flag that (F) & (M) == (M) 
        /// In other words all services in (M) are sure to be present in (F)!!
        /// </summary>
        /// <returns></returns>
        public static System.Data.DataTable GetServiceFlagMasks()
        {
            List<short> servicesFlags = new List<short>();
            Dictionary<short, List<short>> combinations = new Dictionary<short, List<short>>();
            for (short i = 0; i < (short)Math.Pow(2, TABS.FlaggedService.All.Count + 1); i++)
                servicesFlags.Add(i);
            foreach (short flag in servicesFlags)
            {
                combinations[flag] = new List<short>();
                foreach (short mask in servicesFlags)
                {
                    if ((flag & mask) == mask)
                        combinations[flag].Add(mask);
                }
            }

            System.Data.DataTable combinationsTable = new System.Data.DataTable("ServiceFlagMask");
            combinationsTable.Columns.Add("ServiceFlag", typeof(short));
            combinationsTable.Columns.Add("Mask", typeof(short));
            foreach (short flag in combinations.Keys)
            {
                foreach (short mask in combinations[flag])
                {
                    System.Data.DataRow row = combinationsTable.NewRow();
                    row[0] = flag;
                    row[1] = mask;
                    combinationsTable.Rows.Add(row);
                }
            }
            return combinationsTable;
        }
    }
}