using System.Collections.Generic;
using System.Linq;

namespace TABS
{
    public class CarrierSwitchConnectivity : TABS.Components.DateTimeEffectiveEntity, TABS.Interfaces.ICachedCollectionContainer
    {
        public override string Identifier
        {
            get { return "CarrierSwitchConnectivity:" + ID; }
        }

        public static void ClearCachedCollections()
        {
            _Connectivities = null;
            TABS.Components.CacheProvider.Clear(typeof(CarrierSwitchConnectivity).FullName);
        }

        public virtual int ID { get; set; }
        public virtual TABS.CarrierAccount CarrierAccount { get; set; }
        public virtual TABS.Switch Switch { get; set; }
        public virtual string Name { get; set; }
        public virtual string Notes { get; set; }
        public virtual TABS.ConnectionType ConnectionType { get; set; }
        public virtual int? NumberOfChannels_In { get; set; }
        public virtual int? NumberOfChannels_Out { get; set; }
        public virtual int? NumberOfChannels_Total { get; set; }
        public virtual int? NumberOfChannels_Shared { get; set; }
        public virtual string Details { get; set; }

        public virtual List<string> DetailsList
        {
            get
            {
                if (string.IsNullOrEmpty(Details)) return new List<string>();
                return Details.Split(',').ToList();
            }
        }

        public virtual double? Margin_Total { get; set; }

        internal static IList<CarrierSwitchConnectivity> _Connectivities;
        public static IList<CarrierSwitchConnectivity> Connectivities
        {
            get
            {
                if (_Connectivities == null)
                {
                    _Connectivities = TABS.ObjectAssembler.GetList<CarrierSwitchConnectivity>();
                }
                return _Connectivities;
            }
            set { _Connectivities = value; }
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            CarrierSwitchConnectivity other = obj as CarrierSwitchConnectivity;

            if (other == null) result = false;
            if (other.ID == this.ID) result = true;
            if (other.ID == 0) result = other.Description.Equals(this.Description);

            return result;
        }

        public string TextValue { get { return string.Format("{0}           ({1:yyyy-MM-dd}/{2:yyyy-MM-dd})", this.Name, this.BeginEffectiveDate, this.EndEffectiveDate.HasValue ? this.EndEffectiveDate.Value.ToString("yyyy-MM-dd") : "  ---  "); } }

        public override string ToString() { return string.Format("Carrier '{0}', Switch '{1}', Name '{2}'", this.CarrierAccount, this.Switch, this.Name.ToLower()); }

        public string Display { get { return string.Format("Carrier '{0}', Switch '{1}', Name '{2}'", this.CarrierAccount, this.Switch, this.Name); } }

        public string Description { get { return string.Format("{0} ({1:yyyy-MM-dd} - {2:yyyy-MM-dd})", this.ToString(), this.BeginEffectiveDate, this.EndEffectiveDate.HasValue ? this.EndEffectiveDate.Value.ToString("yyyy-MM-dd") : ""); } }

        public override int GetHashCode() { return this.ToString().GetHashCode(); }
    }
}