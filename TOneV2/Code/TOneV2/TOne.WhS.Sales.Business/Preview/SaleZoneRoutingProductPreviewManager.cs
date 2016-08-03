using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            {
                return new SaleZoneRoutingProductPreviewDetail()
                {
                    Entity = entity
                };
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
