using System;

namespace TABS.Components.Routing
{
    public class RouteOption : IComparable<RouteOption>
    {
        public CarrierAccount Supplier;
        public byte Priority;
        public byte NumberOfTries = 0;
        public float Rate;
        public bool IsValid = false;
        public short Percentage { get; set; }
        public int CompareTo(RouteOption other)
        {
            if (other.IsValid != this.IsValid) return (this.IsValid) ? -1 : 1;
            if (other.Priority != this.Priority) return other.Priority.CompareTo(this.Priority);
            return this.Rate.CompareTo(other.Rate);
        }
    }

}
