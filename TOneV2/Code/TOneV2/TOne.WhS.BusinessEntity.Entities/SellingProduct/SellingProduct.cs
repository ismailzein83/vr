
namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProduct
    {
        public int SellingProductId { get; set; }

        public string Name { get; set; }

        public int SellingNumberPlanId { get; set; }

        public SellingProductSettings Settings { get; set; }
    }
}
