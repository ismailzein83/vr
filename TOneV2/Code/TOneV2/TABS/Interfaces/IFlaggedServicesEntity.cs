
namespace TABS.Interfaces
{
    public interface IFlaggedServicesEntity
    {
        short ServicesFlag { get; set; }
        Iesi.Collections.Generic.ISet<FlaggedService> FlaggedServices { get; }
    }
}
