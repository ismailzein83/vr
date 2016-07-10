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

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingSaleZoneRoutingProduct>> ExistingSaleZoneRoutingProducts { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZoneRoutingProduct> existingSaleEntityZoneRoutingProducts = ExistingSaleEntityZoneRoutingProducts.Get(context);

            if (existingSaleEntityZoneRoutingProducts == null || existingSaleEntityZoneRoutingProducts.Count() == 0)
                return;

            var existingSaleZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();

            foreach (SaleZoneRoutingProduct saleEntityZoneRoutingProduct in existingSaleEntityZoneRoutingProducts)
            {
                existingSaleZoneRoutingProducts.Add(new ExistingSaleZoneRoutingProduct()
                {
                    SaleZoneRoutingProductEntity = saleEntityZoneRoutingProduct
                });
            }

            ExistingSaleZoneRoutingProducts.Set(context, existingSaleZoneRoutingProducts);
        }
    }
}
