using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class PrepareExistingZonesRoutingProducts : CodeActivity
    {

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProduct>> ExistingZonesRoutingProductsEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingZoneRoutingProducts>> ExistingZonesRoutingProducts { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZoneRoutingProduct> existingZonesRoutingProductsEntities = this.ExistingZonesRoutingProductsEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingZoneRoutingProducts> existingZonesRoutingProducts = existingZonesRoutingProductsEntities.Where(x => existingZonesByZoneId.ContainsKey(x.SaleZoneId)).MapRecords(
                (ZoneRoutingProductsEntity) => ExistingSaleZoneRoutingProductsMapper(ZoneRoutingProductsEntity, existingZonesByZoneId));

            ExistingZonesRoutingProducts.Set(context, existingZonesRoutingProducts);
        }

        ExistingZoneRoutingProducts ExistingSaleZoneRoutingProductsMapper(SaleZoneRoutingProduct ZoneRoutingProductsEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(ZoneRoutingProductsEntity.SaleZoneId, out existingZone))
                throw new Exception(String.Format("Zone Routing Product Entity with Id {0} is not linked to Zone Id {1}", ZoneRoutingProductsEntity.SaleEntityRoutingProductId, ZoneRoutingProductsEntity.SaleZoneId));

            ExistingZoneRoutingProducts existingZonesRoutingProducts = new ExistingZoneRoutingProducts()
            {
                ZoneRoutingProductEntity = ZoneRoutingProductsEntity,
                ParentZone = existingZone
            };

            existingZonesRoutingProducts.ParentZone.ExistingZonesRoutingProducts.Add(existingZonesRoutingProducts);
            return existingZonesRoutingProducts;
        }
    }
}
