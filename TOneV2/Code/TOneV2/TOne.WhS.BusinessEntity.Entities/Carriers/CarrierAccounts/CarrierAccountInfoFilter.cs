using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class CarrierAccountInfoFilter
	{
		public SupplierFilterSettings SupplierFilterSettings { get; set; }

		public CustomerFilterSettings CustomerFilterSettings { get; set; }

		public bool GetCustomers { get; set; }

		public bool GetSuppliers { get; set; }

		public bool GetExchangeCarriers { get; set; }

		public int? AssignableToSellingProductId { get; set; }

		public int? AssignableToUserId { get; set; }

		public int? SellingNumberPlanId { get; set; }
        public int? SellingProductId { get; set; }
        public List<int> ExcludedCarrierAccountIds { get; set; }

        public IEnumerable<ICarrierAccountFilter> Filters { get; set; }
	}

    public interface ICarrierAccountFilter
    {
        bool IsExcluded(ICarrierAccountFilterContext context);
    }

    public interface ICarrierAccountFilterContext
    {
        CarrierAccount CarrierAccount { get; }

        object CustomObject { get; set; }
    }

    public class CarrierAccountFilterContext : ICarrierAccountFilterContext
    {
        public CarrierAccount CarrierAccount { get; set; }

        public object CustomObject { get; set; }
    }

}
