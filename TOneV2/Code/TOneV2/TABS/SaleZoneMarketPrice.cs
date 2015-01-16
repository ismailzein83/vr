using System.Collections.Generic;

namespace TABS
{
    public class SaleZoneMarketPrice : Components.FlaggedServicesEntity, Interfaces.ICachedCollectionContainer
    {
        //public override string Identifier { get { return "Sale Zone Market Price:" + ((SaleZoneMarketPriceID == null) ? "<NEW>:" + string.Concat(SaleZone.Name, ",", FlaggedServices) : SaleZoneMarketPriceID.ToString()); } }
        public override string Identifier { get { return "Sale Zone Market Price:" + SaleZoneMarketPriceID; } }
        public virtual int SaleZoneMarketPriceID { get; set; }
        public virtual Zone SaleZone { get; set; }
        public virtual decimal FromRate { get; set; }
        public virtual decimal ToRate { get; set; }

        public static void ClearCachedCollections()
        {
            _All = null;
            _ZoneServiceAll = null;
            TABS.Components.CacheProvider.Clear(typeof(SaleZoneMarketPrice).FullName);
        }
        internal static Dictionary<int, SaleZoneMarketPrice> _All;
        public static Dictionary<int, SaleZoneMarketPrice> All
        {
            get
            {
                if (_All == null)
                {
                    _All = new Dictionary<int, SaleZoneMarketPrice>();
                    IList<SaleZoneMarketPrice> list = ObjectAssembler.GetList<SaleZoneMarketPrice>();
                    foreach (SaleZoneMarketPrice saleZoneMarketPrice in list)
                        _All[saleZoneMarketPrice.SaleZoneMarketPriceID] = saleZoneMarketPrice;
                }
                return _All;
            }
        }
        internal static Dictionary<string, SaleZoneMarketPrice> _ZoneServiceAll;
        public static Dictionary<string, SaleZoneMarketPrice> ZoneServiceAll
        {
            get
            {
                if (_ZoneServiceAll == null)
                {
                    _ZoneServiceAll = new Dictionary<string, SaleZoneMarketPrice>();
                    IList<SaleZoneMarketPrice> list = ObjectAssembler.GetList<SaleZoneMarketPrice>();
                    foreach (SaleZoneMarketPrice saleZoneMarketPrice in list)
                        _ZoneServiceAll[string.Concat(saleZoneMarketPrice.SaleZone.ZoneID.ToString(), "_", saleZoneMarketPrice.ServicesFlag.ToString())] = saleZoneMarketPrice;
                }
                return _ZoneServiceAll;
            }
        }
        public bool ConflictWith(SaleZoneMarketPrice saleZoneMarketPrice)
        {
            return (this.SaleZone.ZoneID == saleZoneMarketPrice.SaleZone.ZoneID
                    &&
                    this.ServicesFlag == saleZoneMarketPrice.ServicesFlag);
        }
        public override bool Equals(object obj)
        {
            SaleZoneMarketPrice Other = obj as SaleZoneMarketPrice;
            return
             (this.SaleZone.ZoneID == Other.SaleZone.ZoneID && this.ServicesFlag == Other.ServicesFlag);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
