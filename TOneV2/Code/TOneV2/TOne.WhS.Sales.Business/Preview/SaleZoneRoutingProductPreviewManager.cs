using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class SaleZoneRoutingProductPreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleZoneRoutingProductPreviewDetail> GetFilteredSaleZoneRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleZoneRoutingProductPreviewRequestHandler());
        }

        #region Private Classes

        private class SaleZoneRoutingProductPreviewRequestHandler : BigDataRequestHandler<RatePlanPreviewQuery, SaleZoneRoutingProductPreview, SaleZoneRoutingProductPreviewDetail>
        {
            public override SaleZoneRoutingProductPreviewDetail EntityDetailMapper(SaleZoneRoutingProductPreview entity)
            { RoutingProductManager routingProductManager = new RoutingProductManager();
                var entityDetail = new SaleZoneRoutingProductPreviewDetail()
                {
                    Entity = entity
                   
                };
                     entityDetail.RoutingProductServicesId = !entity.ZoneId.HasValue
                        ? routingProductManager.GetDefaultServiceIds(entity.NewSaleZoneRoutingProductId)
                        : routingProductManager.GetZoneServiceIds(entity.NewSaleZoneRoutingProductId,
                            entity.ZoneId.Value);
                if (entity.CurrentSaleZoneRoutingProductId.HasValue)
                {
                    int recentRoutingProductId = entity.CurrentSaleZoneRoutingProductId.Value;
                    entityDetail.RecentRouringProductServicesId = !entity.ZoneId.HasValue
                        ? routingProductManager.GetDefaultServiceIds(recentRoutingProductId)
                        : routingProductManager.GetZoneServiceIds(recentRoutingProductId, entity.ZoneId.Value);
                }
                return entityDetail;
            }

            public override IEnumerable<SaleZoneRoutingProductPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
                return dataManager.GetSaleZoneRoutingProductPreviews(input.Query);
            }
        }

        #endregion
    }
}
