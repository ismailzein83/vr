
namespace TOne.WhS.Routing.Entities
{
    public class RouteBuildConfiguration
    {
        public CustomerRouteBuildConfiguration CustomerRoute { get; set; }

        public ProductRouteBuildConfiguration ProductRoute { get; set; }
    }


    public class CustomerRouteBuildConfiguration
    {
        public int NumberOfOptions { get; set; }

        public bool CustomerRouteAddBlockedOptions { get; set; }
    }

    public class ProductRouteBuildConfiguration
    {
        public bool ProductRouteAddBlockedOptions { get; set; }
    }
}
