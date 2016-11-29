using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public class PrepareExistingSaleZoneRoutingProducts : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<SaleZoneRoutingProduct>> ExistingSaleEntityZoneRoutingProducts { get; set; }

		[RequiredArgument]
		public InArgument<Dictionary<long, ExistingZone>> ExistingZonesById { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<ExistingSaleZoneRoutingProduct>> ExistingSaleZoneRoutingProducts { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IEnumerable<SaleZoneRoutingProduct> existingSaleEntityZoneRoutingProducts = ExistingSaleEntityZoneRoutingProducts.Get(context);
			Dictionary<long, ExistingZone> existingZonesById = ExistingZonesById.Get(context);

			var existingSaleZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();

			if (existingSaleEntityZoneRoutingProducts != null)
			{
				foreach (SaleZoneRoutingProduct saleEntityZoneRoutingProduct in existingSaleEntityZoneRoutingProducts)
				{
					var existingZoneRoutingProduct = new ExistingSaleZoneRoutingProduct() { SaleZoneRoutingProductEntity = saleEntityZoneRoutingProduct };
					existingSaleZoneRoutingProducts.Add(existingZoneRoutingProduct);

					ExistingZone existingZone;
					if (!existingZonesById.TryGetValue(saleEntityZoneRoutingProduct.SaleZoneId, out existingZone))
						throw new NullReferenceException(string.Format("SaleZone '{0}' was not found", saleEntityZoneRoutingProduct.SaleZoneId));
					existingZone.ExistingZoneRoutingProducts.Add(existingZoneRoutingProduct);
				}
			}

			ExistingSaleZoneRoutingProducts.Set(context, existingSaleZoneRoutingProducts);
		}
	}
}
