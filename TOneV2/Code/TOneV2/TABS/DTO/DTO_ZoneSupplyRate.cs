
namespace TABS.DTO
{
    public class DTO_ZoneSupplyRate : DTO_SupplyRate
    {
        protected Zone _OurZone;
        public Zone OurZone { get { return _OurZone; } set { _OurZone = value; } }
    }
}
