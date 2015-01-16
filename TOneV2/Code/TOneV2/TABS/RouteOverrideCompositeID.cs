using System;

namespace TABS
{
    [Serializable]
    public class RouteOverrideCompositeID : Components.DateTimeEffectiveEntity
    {
        public RouteOverrideCompositeID() { }

        // *****Composite ID******
        public virtual CarrierAccount Customer { get; protected set; }
        public virtual string Code { get; protected set; }
        public virtual Zone OurZone { get; protected set; }
        // *****Composite ID******

        public RouteOverrideCompositeID(CarrierAccount customer, string code, Zone ourZone)
        {
            Customer = customer;
            Code = code;
            OurZone = ourZone;
        }

        protected string CompositIdentifier { get { return string.Format("{0}|{1}|{2}", this.Customer.CarrierAccountID, this.Code, this.OurZone.ZoneID); } }

        public override string ToString() { return CompositIdentifier.ToString(); }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj == null) return false;

            RouteOverrideCompositeID that = obj as RouteOverrideCompositeID;
            if (that == null)
            {
                return false;
            }
            else
            {
                if (this.Customer.CarrierAccountID != that.Customer.CarrierAccountID) return false;
                if (this.Code != that.Code) return false;
                if (this.OurZone.ZoneID != that.OurZone.ZoneID) return false;

                return true;
            }
        }

        public override int GetHashCode() { return CompositIdentifier.GetHashCode(); }

        public override string Identifier { get { return string.Concat("RouteOverride:", this.CompositIdentifier); } }
    }
}
