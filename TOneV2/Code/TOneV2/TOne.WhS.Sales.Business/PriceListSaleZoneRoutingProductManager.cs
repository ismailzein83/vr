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
    public class PriceListSaleZoneRoutingProductManager
    {
        public void ProcessZoneRoutingProducts(IProcessSaleZoneRoutingProductsContext context)
        {
            Process(context.SaleZoneRoutingProductsToAdd, context.SaleZoneRoutingProductsToClose, context.ExistingSaleZoneRoutingProducts, context.ExistingZones);
            context.NewSaleZoneRoutingProducts = context.SaleZoneRoutingProductsToAdd.SelectMany(x => x.NewSaleZoneRoutingProducts);
            context.ChangedSaleZoneRoutingProducts = context.ExistingSaleZoneRoutingProducts.Where(x => x.ChangedSaleZoneRoutingProduct != null).Select(x => x.ChangedSaleZoneRoutingProduct);
        }
        public void Process(IEnumerable<SaleZoneRoutingProductToAdd> routingProductsToAdd, IEnumerable<SaleZoneRoutingProductToClose> routingProductsToClose, IEnumerable<ExistingSaleZoneRoutingProduct> existingRoutingProducts, IEnumerable<ExistingZone> existingZones)
        {
            ExistingSaleZoneRoutingProductsByZoneName existingRoutingProductsByZoneName = StructureExistingRoutingProductsByZoneName(existingRoutingProducts);
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(existingZones);

            foreach (SaleZoneRoutingProductToAdd routingProductToAdd in routingProductsToAdd)
            {
                List<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts;
                if (existingRoutingProductsByZoneName.TryGetValue(routingProductToAdd.ZoneName, out matchedExistingRoutingProducts))
                {
                    CloseOverlappedExistingRoutingProducts(routingProductToAdd, matchedExistingRoutingProducts);
                }
                ProcessRoutingProductToAdd(routingProductToAdd, existingZonesByName);
            }

            foreach (SaleZoneRoutingProductToClose routingProductToClose in routingProductsToClose)
            {
                List<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProduct;
                if (existingRoutingProductsByZoneName.TryGetValue(routingProductToClose.ZoneName, out matchedExistingRoutingProduct))
                {
                    CloseExistingRoutingProducts(routingProductToClose, matchedExistingRoutingProduct);
                }
            }
        }

        private ExistingSaleZoneRoutingProductsByZoneName StructureExistingRoutingProductsByZoneName(IEnumerable<ExistingSaleZoneRoutingProduct> existingRoutingProducts)
        {
            var routingProductsByZoneName = new ExistingSaleZoneRoutingProductsByZoneName();

            if (existingRoutingProducts == null)
                return routingProductsByZoneName;

            List<ExistingSaleZoneRoutingProduct> routingProductList = null;
            var saleZoneManager = new SaleZoneManager();

            foreach (ExistingSaleZoneRoutingProduct routingProduct in existingRoutingProducts)
            {
                string zoneName = saleZoneManager.GetSaleZoneName(routingProduct.SaleZoneRoutingProductEntity.SaleZoneId);

                if (!routingProductsByZoneName.TryGetValue(zoneName, out routingProductList))
                {
                    routingProductList = new List<ExistingSaleZoneRoutingProduct>();
                    routingProductsByZoneName.Add(zoneName, routingProductList);
                }

                routingProductList.Add(routingProduct);
            }

            return routingProductsByZoneName;
        }
        private ExistingZonesByName StructureExistingZonesByName(IEnumerable<ExistingZone> existingZones)
        {
            ExistingZonesByName existingZonesByName = new ExistingZonesByName();
            List<ExistingZone> existingZoneList = null;

            foreach (ExistingZone item in existingZones)
            {
                if (!existingZonesByName.TryGetValue(item.Name, out existingZoneList))
                {
                    existingZoneList = new List<ExistingZone>();
                    existingZonesByName.Add(item.Name, existingZoneList);
                }

                existingZoneList.Add(item);
            }

            return existingZonesByName;
        }

        private void CloseOverlappedExistingRoutingProducts(SaleZoneRoutingProductToAdd routingProductToAdd, IEnumerable<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts)
        {
            foreach (ExistingSaleZoneRoutingProduct existingRoutingProduct in matchedExistingRoutingProducts)
            {
                if (existingRoutingProduct.IsOverlappedWith(routingProductToAdd))
                {
                    DateTime changedSaleZoneRoutingProductEED = Utilities.Max(existingRoutingProduct.BED, routingProductToAdd.BED);
                    existingRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
                        EED = changedSaleZoneRoutingProductEED
                    };
                    routingProductToAdd.ChangedExistingSaleZoneRoutingProducts.Add(existingRoutingProduct);
                }
            }
        }
        private void ProcessRoutingProductToAdd(SaleZoneRoutingProductToAdd routingProductToAdd, ExistingZonesByName existingZonesByName)
        {
            List<ExistingZone> matchedExistingZones;
            existingZonesByName.TryGetValue(routingProductToAdd.ZoneName, out matchedExistingZones);

            DateTime newSaleZoneRoutingProductBED = routingProductToAdd.BED;
            bool shouldAddNewSaleZoneRoutingProducts = true;

            foreach (var existingZone in matchedExistingZones.OrderBy(x => x.BED))
            {
                if (existingZone.EED.VRGreaterThan(existingZone.BED) && existingZone.EED.VRGreaterThan(newSaleZoneRoutingProductBED) && routingProductToAdd.EED.VRGreaterThan(existingZone.BED))
                {
                    AddNewSaleZoneRoutingProduct(routingProductToAdd, ref newSaleZoneRoutingProductBED, existingZone, out shouldAddNewSaleZoneRoutingProducts);
                    if (!shouldAddNewSaleZoneRoutingProducts)
                        break;
                }
            }
        }
        private void AddNewSaleZoneRoutingProduct(SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd, ref DateTime newSaleZoneRoutingProductBED, ExistingZone existingZone, out bool shouldAddNewSaleZoneRoutingProducts)
        {
            shouldAddNewSaleZoneRoutingProducts = false;

            var newSaleZoneRoutingProduct = new NewSaleZoneRoutingProduct
            {
                RoutingProductId = saleZoneRoutingProductToAdd.ZoneRoutingProductId,
                SaleZoneId = saleZoneRoutingProductToAdd.ZoneId,
                BED = Utilities.Max(existingZone.BED, newSaleZoneRoutingProductBED),
                EED = saleZoneRoutingProductToAdd.EED
            };

            if (newSaleZoneRoutingProduct.EED.VRGreaterThan(existingZone.EED)) // => existingZone.EED != null
            {
                newSaleZoneRoutingProduct.EED = existingZone.EED;
                newSaleZoneRoutingProductBED = newSaleZoneRoutingProduct.EED.Value;
                shouldAddNewSaleZoneRoutingProducts = true;
            }

            //existingZone.NewSaleZoneRoutingProducts.Add(newSaleZoneRoutingProduct);
            saleZoneRoutingProductToAdd.NewSaleZoneRoutingProducts.Add(newSaleZoneRoutingProduct);
        }

        private void CloseExistingRoutingProducts(SaleZoneRoutingProductToClose routingProductToClose, IEnumerable<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts)
        {
            foreach (ExistingSaleZoneRoutingProduct existingRoutingProduct in matchedExistingRoutingProducts)
            {
                if (existingRoutingProduct.EED.VRGreaterThan(routingProductToClose.CloseEffectiveDate))
                {
                    existingRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
                        EED = Utilities.Max(existingRoutingProduct.BED, routingProductToClose.CloseEffectiveDate)
                    };
                    routingProductToClose.ChangedExistingSaleZoneRoutingProducts.Add(existingRoutingProduct);
                }
            }
        }
    }
}
