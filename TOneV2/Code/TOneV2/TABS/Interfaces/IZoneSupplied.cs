
namespace TABS.Interfaces
{
    public interface IZoneSupplied : Interfaces.IDateTimeEffective
    {
        CarrierAccount Supplier { get; set; }
        CarrierAccount Customer { get; set; }
        Zone Zone { get; set; }
    }
}