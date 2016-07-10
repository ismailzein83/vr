using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListDefaultRoutingProductManager
    {
        public void ProcessDefaultRoutingProduct(IProcessDefaultRoutingProductContext context)
        {
            if (context.DefaultRoutingProductToAdd != null)
                ProcessDefaultRoutingProductToAdd(context.DefaultRoutingProductToAdd, context.ExistingDefaultRoutingProducts);
            else if (context.DefaultRoutingProductToClose != null)
                ProcessDefaultRoutingProductToClose(context.DefaultRoutingProductToClose, context.ExistingDefaultRoutingProducts);
            SetNewAndChangedDefaultRoutingProducts(context);
        }

        private void ProcessDefaultRoutingProductToAdd(DefaultRoutingProductToAdd defaultRoutingProductToAdd, IEnumerable<ExistingDefaultRoutingProduct> existingDefaultRoutingProducts)
        {
            if (existingDefaultRoutingProducts == null)
                return;

            foreach (ExistingDefaultRoutingProduct existingDefaultRoutingProduct in existingDefaultRoutingProducts)
            {
                if (existingDefaultRoutingProduct.IsOverlapedWith(defaultRoutingProductToAdd))
                {
                    DateTime existingDefaultRoutingProductEED = Utilities.Max(existingDefaultRoutingProduct.BED, defaultRoutingProductToAdd.BED);
                    existingDefaultRoutingProduct.ChangedDefaultRoutingProduct = new ChangedDefaultRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingDefaultRoutingProduct.DefaultRoutingProductEntity.RoutingProductId,
                        EED = existingDefaultRoutingProductEED
                    };
                    defaultRoutingProductToAdd.ChangedExistingDefaultRoutingProducts.Add(existingDefaultRoutingProduct);
                }
            }
        }
        
        private void ProcessDefaultRoutingProductToClose(DefaultRoutingProductToClose defaultRoutingProductToClose, IEnumerable<ExistingDefaultRoutingProduct> existingDefaultRoutingProducts)
        {
            if (existingDefaultRoutingProducts == null)
                return;

            foreach (ExistingDefaultRoutingProduct existingDefaultRoutingProduct in existingDefaultRoutingProducts)
            {
                if (existingDefaultRoutingProduct.EED.VRGreaterThan(defaultRoutingProductToClose.CloseEffectiveDate))
                {
                    existingDefaultRoutingProduct.ChangedDefaultRoutingProduct = new ChangedDefaultRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingDefaultRoutingProduct.DefaultRoutingProductEntity.SaleEntityRoutingProductId,
                        EED = Utilities.Max(existingDefaultRoutingProduct.BED, defaultRoutingProductToClose.CloseEffectiveDate)
                    };
                    defaultRoutingProductToClose.ChangedExistingDefaultRoutingProducts.Add(existingDefaultRoutingProduct);
                }
            }
        }
        
        private void SetNewAndChangedDefaultRoutingProducts(IProcessDefaultRoutingProductContext context)
        {
            if (context.DefaultRoutingProductToAdd != null)
            {
                context.NewDefaultRoutingProduct = context.DefaultRoutingProductToAdd.NewDefaultRoutingProduct;
            }

            if (context.ExistingDefaultRoutingProducts != null)
            {
                IEnumerable<ExistingDefaultRoutingProduct> filteredExistingDefaultRoutingProducts =
                    context.ExistingDefaultRoutingProducts.Where(x => x.ChangedDefaultRoutingProduct != null);

                if (filteredExistingDefaultRoutingProducts != null)
                    context.ChangedDefaultRoutingProducts = filteredExistingDefaultRoutingProducts.Select(x => x.ChangedDefaultRoutingProduct);
            }
        }
    }
}
