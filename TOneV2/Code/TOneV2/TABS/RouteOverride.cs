using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS
{
    [Serializable]
    public class RouteOverride : Components.DateTimeEffectiveEntity, Interfaces.ICachedCollectionContainer
    {
        public RouteOverride() { }

        public RouteOverride(RouteOverrideCompositeID id) { this.ID = id; }

        public RouteOverride(
            CarrierAccount customer,
            string code,
            bool includeSubCodes,
            Zone ourZone,
            string routeOptions,
            string blockedSuppliers,
            DateTime BED,
            DateTime? EED,
            DateTime? updated,
            SecurityEssentials.User user
            )
        {
            this.ID = new RouteOverrideCompositeID(customer, code, ourZone);
            this.IncludeSubCodes = includeSubCodes;
            this._RouteOptions = routeOptions;
            this._BlockedSuppliers = blockedSuppliers;
            this.BeginEffectiveDate = BED;
            this.EndEffectiveDate = EED;
            this.Updated = updated;
            this.User = user;
        }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(RouteOverride).FullName);
        }

        public virtual RouteOverrideCompositeID ID { get; set; }

        public virtual CarrierAccount Customer { get { return ID != null ? ID.Customer : null; } }
        public virtual Zone OurZone { get { return ID != null ? ID.OurZone : null; } }
        public virtual string Code { get { return ID != null ? ID.Code : null; } }
        public virtual bool IncludeSubCodes { get; set; }
        public bool _IsTransiant = true;
        public virtual DateTime? Updated { get; set; }
        public bool IsCustomerBlockedInactive { get; set; }

        public virtual string _RouteOptions { get; set; }
        public virtual string _BlockedSuppliers { get; set; }
        public virtual string ExcludedCodes { get; set; }

        public virtual string Reason { get; set; }

        internal static Dictionary<CarrierAccount, List<RouteOverride>> _All;

        /// <summary>
        /// Return all the route overrides from database 
        /// </summary>
        public static Dictionary<CarrierAccount, List<RouteOverride>> All
        {
            get
            {
                lock (typeof(RouteOverride))
                {
                    if (_All == null)
                        _All = ObjectAssembler.GetRouteOverrides();
                }
                return _All;
            }
        }

        public virtual bool IsTransiant
        {
            get { return _IsTransiant; }
            set { _IsTransiant = value; }
        }

        /// <summary>
        /// Immediate update of route when updating a new override
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool UpdateRoutes(out Exception ex)
        {
            bool success = true;

            try
            {
                string SQL = @"EXEC bp_UpdateRoutingFromOverrides_WithPercentage_New
	                                @CustomerID = @P1,
	                                @IncludeSubCodes =@P2,
	                                @ExcludedCodes =@P3,
	                                @Code = @P4,
	                                @OurZoneID = @P5,
	                                @RouteOptions = @P6,
	                                @blockedSuppliers = @P7";
                TABS.DataHelper.ExecuteNonQuery(SQL
                                               , this.Customer.CarrierAccountID
                                               , this.IncludeSubCodes ? 'Y' : 'N'
                                               , this.ExcludedCodes
                                               , this.Code
                                               , this.OurZone.ZoneID
                                               , this._RouteOptions != null && this._RouteOptions.Length > 0 ? this._RouteOptions : null
                                               , this._BlockedSuppliers != null && this._BlockedSuppliers.Length > 0 ? this._BlockedSuppliers : null);
                ex = null;
            }
            catch (Exception EX)
            {
                ex = EX;
                success = false;
            }

            return success;
        }


        public class SupplierOptionPercentage
        {
            public TABS.CarrierAccount Supplier { get; set; }
            public short Percentage { get; set; }
        }

        public static List<SupplierOptionPercentage> GetSupplierOptionsWithPercentage(string _RouteOptionString)
        {
            if (string.IsNullOrEmpty(_RouteOptionString)) return new List<SupplierOptionPercentage>();

            List<SupplierOptionPercentage> carrierOptions = new List<SupplierOptionPercentage>();

            var optionList = _RouteOptionString.Split('|');
            foreach (var optionWithPercentage in optionList)
            {
                var splittedOption = optionWithPercentage.Split(',');
                string option = splittedOption[0];
                short percentage = splittedOption.Count() == 2 ? GetSafeShort(splittedOption[1]) : (short)0;

                if (CarrierAccount.All.ContainsKey(option))
                    carrierOptions.Add(new SupplierOptionPercentage() { Percentage = percentage, Supplier = CarrierAccount.All[option.Trim()] });
                //else
                //    carrierOptions.Add(new SupplierOptionPercentage() { Percentage = percentage, Supplier = ObjectAssembler.Get<CarrierAccount>(option.Trim()) });

            }

            return carrierOptions;
        }

        protected static short GetSafeShort(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            return short.Parse(s);
        }
        /// <summary>
        /// return a list of carrieraccount that represents supply options from a pipe line seperated carrier account id
        /// </summary>
        /// <param name="_RouteOptionString"></param>
        /// <returns></returns>
        public static List<TABS.CarrierAccount> GetRouteOptions(string _RouteOptionString)
        {
            return GetSupplierOptionsWithPercentage(_RouteOptionString).Select(s => s.Supplier).ToList();
        }

        /// <summary>
        /// return a list of carrieraccount that represents blocked options from a pipe line seperated carrier account id
        /// </summary>
        /// <param name="_BlockedSupliersString"></param>
        /// <returns></returns>
        public static List<TABS.CarrierAccount> GetBlockedSuppliers(string _BlockedSupliersString)
        {
            if (string.IsNullOrEmpty(_BlockedSupliersString)) return new List<CarrierAccount>();

            List<TABS.CarrierAccount> blockedSuppliers = new List<CarrierAccount>();

            var blockList = _BlockedSupliersString.Split('|');
            foreach (var block in blockList)
            {

                if (CarrierAccount.All.ContainsKey(block))
                    blockedSuppliers.Add(CarrierAccount.All[block]);
                //else
                //    blockedSuppliers.Add(ObjectAssembler.Get<CarrierAccount>(block));
            }

            return blockedSuppliers;
        }

        public virtual string Target
        {
            get
            {
                if (Code != null && Code != "*ALL*")
                {
                    return string.Format("{0} ({1})", Code, ZoneName);
                }
                else
                {
                    return ZoneName;
                }
            }
        }

        public virtual string ZoneName
        {
            get
            {
                if (Code != null && Code != "*ALL*")
                {
                    var ourCode = CodeMap.CurrentOurCodes.Find(Code, CarrierAccount.SYSTEM, DateTime.Now);
                    if (ourCode != null) return ourCode.Zone.Name;
                    else return null;
                }
                else
                {
                    return (OurZone != null ? OurZone.Name : null);
                }
            }
        }

        public override void OnLoad(NHibernate.ISession s, object id)
        {
            // mark the loaded override
            this.IsTransiant = false;
            base.OnLoad(s, id);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string Identifier
        {
            get { return ID.Identifier; }
        }

        public string DefinitionDisplay
        {
            get { return string.Format("{0} - {1} - {2}", this.Customer, this.Code, this.ZoneName); }
        }

        public string SubcodeDisplay
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} ", IncludeSubCodes ? "Y" : "N");
                if (!string.IsNullOrEmpty(ExcludedCodes)) sb.AppendFormat("Exc: {0}", ExcludedCodes);
                return sb.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            RouteOverride that = obj as RouteOverride;
            return this.ID.Equals(that.ID);
        }
    }
}
