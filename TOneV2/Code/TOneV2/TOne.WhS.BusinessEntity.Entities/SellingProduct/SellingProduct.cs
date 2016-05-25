
namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseSellingProduct
    {
        public int SellingProductId { get; set; }

        public string Name { get; set; }

        public SellingProductSettings Settings { get; set; }
    }

    public class SellingProduct : BaseSellingProduct
    {
        public int SellingNumberPlanId { get; set; }
    }

    public class SellingProductToEdit : BaseSellingProduct
    {

    }
}
