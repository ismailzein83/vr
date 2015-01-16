
namespace TABS
{
    public class RoutingPool : Components.BaseEntity
    {
        public override string Identifier { get { return "RoutingPool:" + (ID == 0 ? Name : ID.ToString()); } }

        public virtual Iesi.Collections.Generic.ISet<CarrierAccount> Customers { get; set; }
        public virtual Iesi.Collections.Generic.ISet<CarrierAccount> Suppliers { get; set; }

        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsEnabled { get; set; }

        public RoutingPool()
        {
            this.Customers = new Iesi.Collections.Generic.HashedSet<CarrierAccount>();
            this.Suppliers = new Iesi.Collections.Generic.HashedSet<CarrierAccount>();
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
