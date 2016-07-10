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
        public void ProcessSaleZoneRoutingProducts(IProcessSaleZoneRoutingProductsContext context)
        {
            ExistingSaleZoneRoutingProductsByZoneName existingSaleZoneRoutingProductsByZoneName = StructureExistingSaleZoneRoutingProductsByZoneName(context.ExistingSaleZoneRoutingProducts);
            ExistingZonesByName existingZonesByName = StructureExistingZonesByName(context.ExistingZones);

            ProcessSaleZoneRoutingProductsToAdd(context.SaleZoneRoutingProductsToAdd, existingSaleZoneRoutingProductsByZoneName, existingZonesByName);
            ProcessSaleZoneRoutingProductsToClose(context.SaleZoneRoutingProductsToClose, existingSaleZoneRoutingProductsByZoneName);

            SetNewAndChangedSaleZoneRoutingProducts(context);
        }

        private ExistingSaleZoneRoutingProductsByZoneName StructureExistingSaleZoneRoutingProductsByZoneName(IEnumerable<ExistingSaleZoneRoutingProduct> existingSaleZoneRoutingProducts)
        {
            var routingProductsByZoneName = new ExistingSaleZoneRoutingProductsByZoneName();

            if (existingSaleZoneRoutingProducts == null)
                return routingProductsByZoneName;

            List<ExistingSaleZoneRoutingProduct> routingProductList = null;
            var saleZoneManager = new SaleZoneManager();

            foreach (ExistingSaleZoneRoutingProduct routingProduct in existingSaleZoneRoutingProducts)
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

        #region Process Sale Zone Routing Products To Add

        private void ProcessSaleZoneRoutingProductsToAdd(IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd, ExistingSaleZoneRoutingProductsByZoneName existingSaleZoneRoutingProductsByZoneName, ExistingZonesByName existingZonesByName)
        {
            if (saleZoneRoutingProductsToAdd == null)
                return;

            foreach (SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd in saleZoneRoutingProductsToAdd)
            {
                SyncSaleZoneRoutingProductToAddWithExistingSaleZoneRoutingProducts(saleZoneRoutingProductToAdd, existingSaleZoneRoutingProductsByZoneName);
                SyncSaleZoneRoutingProductToAddWithExistingZones(saleZoneRoutingProductToAdd, existingZonesByName);
            }
        }

        private void SyncSaleZoneRoutingProductToAddWithExistingSaleZoneRoutingProducts(SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd, ExistingSaleZoneRoutingProductsByZoneName existingSaleZoneRoutingProductsByZoneName)
        {
            List<ExistingSaleZoneRoutingProduct> matchedExistingSaleZoneRoutingProducts;
            existingSaleZoneRoutingProductsByZoneName.TryGetValue(saleZoneRoutingProductToAdd.ZoneName, out matchedExistingSaleZoneRoutingProducts);

            if (matchedExistingSaleZoneRoutingProducts == null)
                return;

            foreach (ExistingSaleZoneRoutingProduct existingSaleZoneRoutingProduct in matchedExistingSaleZoneRoutingProducts)
            {
                if (existingSaleZoneRoutingProduct.IsOverlapedWith(saleZoneRoutingProductToAdd))
                {
                    DateTime changedSaleZoneRoutingProductEED = Utilities.Max(existingSaleZoneRoutingProduct.BED, saleZoneRoutingProductToAdd.BED);
                    existingSaleZoneRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingSaleZoneRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
                        EED = changedSaleZoneRoutingProductEED
                    };
                    saleZoneRoutingProductToAdd.ChangedExistingSaleZoneRoutingProducts.Add(existingSaleZoneRoutingProduct);
                }
            }
        }

        private void SyncSaleZoneRoutingProductToAddWithExistingZones(SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd, ExistingZonesByName existingZonesByName)
        {
            List<ExistingZone> matchedExistingZones;
            existingZonesByName.TryGetValue(saleZoneRoutingProductToAdd.ZoneName, out matchedExistingZones);

            DateTime newSaleZoneRoutingProductBED = saleZoneRoutingProductToAdd.BED;
            bool shouldAddNewSaleZoneRoutingProducts = true;

            foreach (var existingZone in matchedExistingZones.OrderBy(x => x.BED))
            {
                if (existingZone.EED.VRGreaterThan(existingZone.BED) && existingZone.EED.VRGreaterThan(newSaleZoneRoutingProductBED) && saleZoneRoutingProductToAdd.EED.VRGreaterThan(existingZone.BED))
                {
                    AddNewSaleZoneRoutingProduct(saleZoneRoutingProductToAdd, ref newSaleZoneRoutingProductBED, existingZone, out shouldAddNewSaleZoneRoutingProducts);
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

        #endregion

        #region Process Sale Zone Routing Products To Close

        private void ProcessSaleZoneRoutingProductsToClose(IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose, ExistingSaleZoneRoutingProductsByZoneName existingSaleZoneRoutingProductsByZoneName)
        {
            if (saleZoneRoutingProductsToClose == null)
                return;

            foreach (SaleZoneRoutingProductToClose saleZoneRoutingProductToClose in saleZoneRoutingProductsToClose)
            {
                SyncSaleZoneRoutingProductToCloseWithExistingSaleZoneRoutingProducts(saleZoneRoutingProductToClose, existingSaleZoneRoutingProductsByZoneName);
            }
        }

        private void SyncSaleZoneRoutingProductToCloseWithExistingSaleZoneRoutingProducts(SaleZoneRoutingProductToClose saleZoneRoutingProductToClose, ExistingSaleZoneRoutingProductsByZoneName existingSaleZoneRoutingProductsByZoneName)
        {
            List<ExistingSaleZoneRoutingProduct> matchedExistingSaleZoneRoutingProducts;
            existingSaleZoneRoutingProductsByZoneName.TryGetValue(saleZoneRoutingProductToClose.ZoneName, out matchedExistingSaleZoneRoutingProducts);

            if (matchedExistingSaleZoneRoutingProducts == null)
                return;

            foreach (ExistingSaleZoneRoutingProduct existingSaleZoneRoutingProduct in matchedExistingSaleZoneRoutingProducts)
            {
                if (existingSaleZoneRoutingProduct.EED.VRGreaterThan(saleZoneRoutingProductToClose.CloseEffectiveDate))
                {
                    existingSaleZoneRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
                    {
                        SaleEntityRoutingProductId = existingSaleZoneRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
                        EED = Utilities.Max(existingSaleZoneRoutingProduct.BED, saleZoneRoutingProductToClose.CloseEffectiveDate)
                    };
                    saleZoneRoutingProductToClose.ChangedExistingSaleZoneRoutingProducts.Add(existingSaleZoneRoutingProduct);
                }
            }
        }

        #endregion

        private void SetNewAndChangedSaleZoneRoutingProducts(IProcessSaleZoneRoutingProductsContext context)
        {
            if (context.SaleZoneRoutingProductsToAdd != null)
            {
                context.NewSaleZoneRoutingProducts = context.SaleZoneRoutingProductsToAdd.SelectMany(x => x.NewSaleZoneRoutingProducts);
            }

            if (context.ExistingSaleZoneRoutingProducts != null)
            {
                IEnumerable<ExistingSaleZoneRoutingProduct> filteredEntities = context.ExistingSaleZoneRoutingProducts.Where(x => x.ChangedSaleZoneRoutingProduct != null);

                if (filteredEntities != null)
                    context.ChangedSaleZoneRoutingProducts = filteredEntities.Select(x => x.ChangedSaleZoneRoutingProduct);
            }
        }
    }
}
