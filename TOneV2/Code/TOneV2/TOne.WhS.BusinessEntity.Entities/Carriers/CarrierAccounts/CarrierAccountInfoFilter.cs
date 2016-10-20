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
	}
}
