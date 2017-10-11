using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class PriceListZoneRoutingProductsManager
    {
        public void ProcessCountryZonesRoutingProducts(IProcessCountryZonesRoutingProductsContext context)
        {
            ProcessCountryZonesRoutingProducts(context.ZonesToProcess, context.ExistingZonesRoutingProducts, context.ExistingZones, context.NotImportedZones);
            context.ChangedZonesRoutingProducts = context.ExistingZonesRoutingProducts.Where(itm => itm.ChangedZoneRoutingProducts != null).Select(itm => itm.ChangedZoneRoutingProducts);
        }

        private void ProcessCountryZonesRoutingProducts(IEnumerable<ZoneToProcess> zonesToProcess,IEnumerable<ExistingZoneRoutingProducts> existingZonesRoutingProducts, IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones)
        {           
            ExistingZoneRoutingProductGroupByZoneName existingZoneRoutingProductGroupByZoneName = StructureExistingZonesRoutingProductsByZoneName(existingZonesRoutingProducts);
            ProcessNotImportedData(existingZones, notImportedZones, existingZoneRoutingProductGroupByZoneName);
            ProcessImportedData(zonesToProcess, existingZones, existingZoneRoutingProductGroupByZoneName);
        }

        private void ProcessNotImportedData(IEnumerable<ExistingZone> existingZones, IEnumerable<NotImportedZone> notImportedZones, ExistingZoneRoutingProductGroupByZoneName existingZoneRoutingProductGroupByZoneName)
        {
            CloseRoutingProductsForClosedZones(existingZones);
            FillRoutingProductsForNotImportedZones(notImportedZones, existingZoneRoutingProductGroupByZoneName);

        }
        private void CloseRoutingProductsForClosedZones(IEnumerable<ExistingZone> existingZones)
        {
            foreach (var existingZone in existingZones)
            {
                if (existingZone.ChangedZone != null)
                {
                    DateTime zoneEED = existingZone.ChangedZone.EED;
                    if (existingZone.ExistingZonesRoutingProducts != null)
                    {
                        foreach (var existingZoneRoutingProducts in existingZone.ExistingZonesRoutingProducts)
                        {
                            DateTime? rateEED = existingZoneRoutingProducts.EED;
                            if (rateEED.VRGreaterThan(zoneEED))
                            {
                                if (existingZoneRoutingProducts.ChangedZoneRoutingProducts == null)
                                {
                                    existingZoneRoutingProducts.ChangedZoneRoutingProducts = new ChangedZoneRoutingProducts
                                    {
                                        EntityId = existingZoneRoutingProducts.ZoneRoutingProductEntity.SaleEntityRoutingProductId
                                    };
                                }
                                DateTime rateBED = existingZoneRoutingProducts.ZoneRoutingProductEntity.BED;
                                existingZoneRoutingProducts.ChangedZoneRoutingProducts.EED = zoneEED > rateBED ? zoneEED : rateBED;
                            }
                        }
                    }
                }
            }
        }


        #region Imported
        private void ProcessImportedData(IEnumerable<ZoneToProcess> zonesToProcess, IEnumerable<ExistingZone> existingZones,ExistingZoneRoutingProductGroupByZoneName existingZoneRoutingProductGroupByZoneName)
        {
            if (!existingZones.Any())
                return;

            ExistingZoneRoutingProductGroup existingZoneRoutingProductGroup;
            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                existingZoneRoutingProductGroupByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingZoneRoutingProductGroup);
                if (zoneToProcess.ChangeType != ZoneChangeType.New && zoneToProcess.ChangeType != ZoneChangeType.Renamed)
                    PrepareDataForPreview(zoneToProcess, existingZoneRoutingProductGroup);
            }
        }

        private void PrepareDataForPreview(ZoneToProcess zoneToProcess, ExistingZoneRoutingProductGroup existingZoneRoutingProductGroup)
        {
            if (existingZoneRoutingProductGroup == null)
                return;

            IEnumerable<NotImportedZoneRoutingProduct> notImportedZoneRoutingProduct = this.GetNotImportedZonesRPsFromExistingZonesRPsByOwner(zoneToProcess.ZoneName, existingZoneRoutingProductGroup.ZoneRoutingProducts);
            zoneToProcess.NotImportedZoneRoutingProduct.AddRange(notImportedZoneRoutingProduct);
        }
        #endregion

        #region notImported
        private void FillRoutingProductsForNotImportedZones(IEnumerable<NotImportedZone> notImportedZones, ExistingZoneRoutingProductGroupByZoneName existingZoneRoutingProductGroupByZoneName)
        {
            if (notImportedZones == null)
                return;

            ExistingZoneRoutingProductGroup existingZoneRoutingProductGroup;
            foreach (NotImportedZone notImportedZone in notImportedZones)
            {
                if (existingZoneRoutingProductGroupByZoneName.TryGetValue(notImportedZone.ZoneName, out existingZoneRoutingProductGroup))
                {
                    if (existingZoneRoutingProductGroup == null)
                        continue;

                    IEnumerable<NotImportedZoneRoutingProduct> notImportedZoneRoutingProduct = this.GetNotImportedZonesRPsFromExistingZonesRPsByOwner(notImportedZone.ZoneName, existingZoneRoutingProductGroup.ZoneRoutingProducts);
                    notImportedZone.NotImportedZoneRoutingProduct.AddRange(notImportedZoneRoutingProduct);
                }
            }
        }
        private IEnumerable<NotImportedZoneRoutingProduct> GetNotImportedZonesRPsFromExistingZonesRPsByOwner(string zoneName, ExistingZonesRoutingProductsByOwner existingZonesRoutingProductsByOwner)
        {
            List<NotImportedZoneRoutingProduct> notImportedZonesRoutingProducts = new List<NotImportedZoneRoutingProduct>();

            var e = existingZonesRoutingProductsByOwner.GetEnumerator();
            while (e.MoveNext())
            {
                Owner owner = existingZonesRoutingProductsByOwner.GetOwner(e.Current.Key);
                NotImportedZoneRoutingProduct notImportedZoneRoutingProduct = this.GetNotImportedZoneRoutingProduct(zoneName, owner, e.Current.Value);
                if (notImportedZoneRoutingProduct != null)
                    notImportedZonesRoutingProducts.Add(notImportedZoneRoutingProduct);
            }
            return notImportedZonesRoutingProducts;
        }
        private NotImportedZoneRoutingProduct GetNotImportedZoneRoutingProduct(string zoneName, Owner owner, List<ExistingZoneRoutingProducts> existingZoneRoutingProducts)
        {
            ExistingZoneRoutingProducts lastElement = GetLastExistingZoneRPFromConnectedExistingZoneRPs(existingZoneRoutingProducts);
            if (lastElement == null)
                return null;
            SaleRateManager saleRateManager = new SaleRateManager();

            return new NotImportedZoneRoutingProduct()
            {
                ZoneName = zoneName,
                OwnerType = owner.OwnerType,
                OwnerId = owner.OwnerId,
                RoutingProductId=lastElement.ZoneRoutingProductEntity.RoutingProductId,
                BED = lastElement.BED,
                EED = lastElement.EED,
            };
        }
        private ExistingZoneRoutingProducts GetLastExistingZoneRPFromConnectedExistingZoneRPs(List<ExistingZoneRoutingProducts> existingZoneRoutingProducts)
        {
            List<ExistingZoneRoutingProducts> connectedExistingRPs = existingZoneRoutingProducts.GetConnectedEntities(DateTime.Today);
            if (connectedExistingRPs == null)
                return null;

            return connectedExistingRPs.Last();
        }
        private ExistingZoneRoutingProductGroupByZoneName StructureExistingZonesRoutingProductsByZoneName(IEnumerable<ExistingZoneRoutingProducts> existingZoneRoutingProducts)
        {
            ExistingZoneRoutingProductGroupByZoneName existingZoneRoutingProductGroupByZoneName = new ExistingZoneRoutingProductGroupByZoneName();

            if (existingZoneRoutingProducts != null)
            {
                foreach (ExistingZoneRoutingProducts existingZoneRoutingProduct in existingZoneRoutingProducts)
                {

                    string zoneName = existingZoneRoutingProduct.ParentZone.Name;
                    ExistingZoneRoutingProductGroup existingZoneRoutingProductGroup = null;
                    if (!existingZoneRoutingProductGroupByZoneName.TryGetValue(zoneName, out existingZoneRoutingProductGroup))
                    {
                        existingZoneRoutingProductGroup = new ExistingZoneRoutingProductGroup();
                        existingZoneRoutingProductGroup.ZoneName = zoneName;
                        existingZoneRoutingProductGroupByZoneName.Add(zoneName, existingZoneRoutingProductGroup);
                    }
                    existingZoneRoutingProductGroup.ZoneRoutingProducts.TryAddValue((int)existingZoneRoutingProduct.ZoneRoutingProductEntity.OwnerType, existingZoneRoutingProduct.ZoneRoutingProductEntity.OwnerId, existingZoneRoutingProduct);
                }
            }

            return existingZoneRoutingProductGroupByZoneName;
        }
        #endregion


    }
}
