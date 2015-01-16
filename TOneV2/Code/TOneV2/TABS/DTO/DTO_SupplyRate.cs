using System;

namespace TABS.DTO
{
    public class DTO_SupplyRate : DTO_Rate, IComparable<DTO_SupplyRate>
    {
        public TABS.CarrierAccount _Supplier;
        public TABS.Zone _Zone;
        public TABS.CarrierAccount Supplier { get { return _Supplier; } set { _Supplier = value; } }
        public TABS.Zone Zone { get { return _Zone; } set { _Zone = value; } }
        public TABS.Rate Rate { get; set; }
        protected bool _IsRouteOverrideAffected = false;
        public bool IsRouteOverrideAffected
        {
            get { return _IsRouteOverrideAffected; }
            set { _IsRouteOverrideAffected = value; }
        }

        public bool IsBlockAffected { get; set; }

        #region IComparable<DTO_SupplyRate> Members

        public int CompareTo(DTO_SupplyRate other)
        {
            return base.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            DTO_SupplyRate other = obj as DTO_SupplyRate;
            if (!other.Supplier.Equals(this.Supplier)) return false;
            if (!other.Zone.Equals(this.Zone)) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string Identifier
        {
            get
            {
                return string.Concat("SupplyRate:", Supplier.CarrierAccountID, "->", Zone.ZoneID);
            }
        }

        #endregion
    }
}
