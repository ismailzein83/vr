
namespace TOne.WhS.Routing.Entities
{
    public class RouteBuildConfiguration
    {
        public CustomerRouteBuildConfiguration CustomerRoute { get; set; }

        public ProductRouteBuildConfiguration ProductRoute { get; set; }
    }


    public class CustomerRouteBuildConfiguration
    {
        public bool AddBlockedOptions { get; set; }

        public int NumberOfUnblockedOptions { get; set; }
    }

    public class ProductRouteBuildConfiguration
    {
        public bool AddBlockedOptions { get; set; }
    }
}
 