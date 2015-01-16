using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;

namespace TABS
{
    [Serializable]
    public class RouteBlock : Components.DateTimeEffectiveEntity, Interfaces.IZoneSupplied, Interfaces.ICachedCollectionContainer
    {
        public override string Identifier { get { return "RouteBlock:" + RouteBlockID; } }

        public static void ClearCachedCollections()
        {
            _SupplierZoneBlocks = null;
            _SupplierCodeBlocks = null;
            _EffectiveBlocks = null;
            TABS.Components.CacheProvider.Clear(typeof(RouteBlock).FullName);
        }

        internal static IList<RouteBlock> _EffectiveBlocks;
        public static IList<RouteBlock> EffectiveBlocks
        {
            get
            {
                if (_EffectiveBlocks == null)
                {

                    NHibernate.ICriteria criteria = DataConfiguration.CurrentSession.CreateCriteria(typeof(RouteBlock))
                            .Add(Expression.Le("BeginEffectiveDate", DateTime.Now))
                            .Add(Expression.Or(
                                Expression.Gt("EndEffectiveDate", DateTime.Now),
                                new NullExpression("EndEffectiveDate"))
                                )
                            .AddOrder(new Order("BeginEffectiveDate", false));

                    _EffectiveBlocks = criteria.List<TABS.RouteBlock>();

                }
                return _EffectiveBlocks;
            }
        }

        private int _RouteBlockID;
        private CarrierAccount _Customer;
        private CarrierAccount _Supplier;
        private Zone _Zone;
        private string _Code;
        private RouteBlockType _BlockType;
        public virtual string Reason { get; set; }
        public virtual RouteChangeHeader RouteChangeHeader { get; set; }
        public virtual DateTime? UpdateDate { get; set; }

        public virtual int RouteBlockID
        {
            get { return _RouteBlockID; }
            set { _RouteBlockID = value; }
        }

        public virtual CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        public virtual string ExcludedCodes { get; set; }

        public virtual RouteBlockType BlockType
        {
            get { return _BlockType; }
            set { _BlockType = value; }
        }
        protected string _IncludeSubCodes;
        public virtual bool IncludeSubCodes
        {
            get { return "Y".Equals(_IncludeSubCodes); }
            set { _IncludeSubCodes = value ? "Y" : "N"; }
        }

        public static IList<RouteBlock> GetEffective()
        {
            return TABS.DataConfiguration.CurrentSession
                  .CreateQuery(@"FROM RouteBlock R WHERE ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)")
                       .SetParameter("when", DateTime.Now)
                  .List<RouteBlock>();
        }
        internal static bool _IsFutureType = false;
        internal static IList<RouteBlock> _SupplierZoneBlocks;
        internal static IList<RouteBlock> _SupplierCodeBlocks;
        //public static IList<RouteBlock> SupplierZoneBlocks
        //{
        //    get
        //    {
        //        if (_SupplierZoneBlocks == null)
        //        {
        //            _SupplierZoneBlocks = GetSupplierZoneBlocks(true);
        //        }
        //        return _SupplierZoneBlocks;
        //    }
        //}
        public static IList<RouteBlock> SupplierZoneBlocksList(bool future)
        {

            if (_SupplierZoneBlocks == null || _IsFutureType!=future)
                {
                    _IsFutureType = future;
                    _SupplierZoneBlocks = null;
                    _SupplierZoneBlocks=GetSupplierZoneBlocks(future);
                }
                return _SupplierZoneBlocks;
            
        }
        public static IList<RouteBlock> SupplierCodeBlocks
        {
            get
            {
                if (_SupplierCodeBlocks == null)
                {
                    _SupplierCodeBlocks = null;
                    _SupplierCodeBlocks=GetSupplierCodeBlocks(true);
                }
                return _SupplierCodeBlocks;
            }
        }
        protected static IList<RouteBlock> GetSupplierZoneBlocks(bool future)
        {
            var noticeDays = future ? (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value : 0;
            DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);//Now
            return TABS.DataConfiguration.CurrentSession
          .CreateQuery(@"FROM RouteBlock R WHERE ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when ) AND R.Zone IS NOT NULL")
               .SetParameter("when", noticeFuture)
          .List<RouteBlock>();
        }
        protected static IList<RouteBlock> GetSupplierCodeBlocks(bool future)
        {
            var noticeDays = future ? (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value : 0;
            DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);
            return TABS.DataConfiguration.CurrentSession
          .CreateQuery(@"FROM RouteBlock R WHERE ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when) AND R.Zone IS NULL")
               .SetParameter("when", noticeFuture)
          .List<RouteBlock>();
        }

        public static IList<TABS.RouteBlock> GetExistingBlocks(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, TABS.Zone zone, string code, DateTime when)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    FROM RouteBlock R
                        WHERE 1=1");

            if (customer != null) sb.Append("AND (R.Customer IS NULL OR R.Customer = :Customer)");
            if (supplier != null) sb.Append("AND (R.Supplier IS NULL OR R.Supplier = :Supplier)");

            if (zone != null) sb.Append(@"AND (R.Zone IS NULL OR R.Zone = :Zone)");

            if (!string.IsNullOrEmpty(code)) sb.Append("AND (R.Code IS NULL OR R.Code LIKE :CodeValue)");

            sb.Append("AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)");


            NHibernate.ISession session = TABS.DataConfiguration.CurrentSession;
            NHibernate.IQuery query = session.CreateQuery(sb.ToString());
            if (customer != null) query.SetParameter("Customer", customer);
            if (supplier != null) query.SetParameter("Supplier", supplier);

            query.SetParameter("Zone", zone);

            if (!string.IsNullOrEmpty(code)) query.SetParameter("CodeValue", code);

            query.SetParameter("when", when);

            return query.List<TABS.RouteBlock>();
        }

        public static IList<TABS.RouteBlock> GetExistingBlocks(TABS.CarrierAccount customer, TABS.Zone zone, string code, DateTime when)
        {
            return TABS.DataConfiguration.CurrentSession
                .CreateQuery(@"
                    FROM RouteBlock R
                        WHERE 
                                (R.Customer IS NULL OR R.Customer = :Customer) 
                            AND (R.Zone IS NULL OR R.Zone = :Zone) 
                            AND (R.Code IS NULL OR R.Code LIKE :CodeValue) 
                            AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR (R.BeginEffectiveDate > :when AND R.EndEffectiveDate > R.BeginEffectiveDate))")
                    .SetParameter("Customer", customer)
                    .SetParameter("Zone", zone)
                    .SetParameter("CodeValue", code == null ? "%%" : code)
                    .SetParameter("when", when)
                .List<TABS.RouteBlock>();
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(" Route Block: ");
            sb.Append(((Customer != null) ? (",Customer: " + Customer.ToString()) : ""));
            sb.Append(((Supplier != null) ? (",Supplier: " + Supplier.ToString()) : ""));
            sb.Append((Zone != null) ? ",Zone: " + Zone.ToString() : "");
            sb.Append(((Code != null) ? (",Code: " + Code.ToString()) : ""));
            sb.Append("  " + BlockType.ToString());
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            RouteBlock other = obj as RouteBlock;
            if (other == null)
                return base.Equals(obj);
            else
            {
                return this.RouteBlockID.Equals(other.RouteBlockID);
            }
        }

        public string KeySupplierZoneBlock
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (this.Supplier != null) sb.AppendFormat("Supplier: {0}", this.Supplier);
                if (this.Zone != null) sb.AppendFormat("Cost Zone: {0}", this.Zone.ZoneID);

                return sb.ToString();
            }
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
