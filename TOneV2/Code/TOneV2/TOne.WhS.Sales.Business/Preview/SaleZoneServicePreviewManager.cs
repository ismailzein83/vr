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
    public class SaleZoneServicePreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SaleZoneServicePreviewDetail> GetFilteredSaleZoneServicePreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleZoneServicePreviewRequestHandler());
        }

        #region Private Classes

        private class SaleZoneServicePreviewRequestHandler : BigDataRequestHandler<RatePlanPreviewQuery, SaleZoneServicePreview, SaleZoneServicePreviewDetail>
        {
            public override SaleZoneServicePreviewDetail EntityDetailMapper(SaleZoneServicePreview entity)
            {
                return new SaleZoneServicePreviewDetail()
                {
                    Entity = entity
                };
            }

            public override IEnumerable<SaleZoneServicePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<ISaleZoneServicePreviewDataManager>();
                return dataManager.GetSaleZoneServicePreviews(input.Query);
            }
        }

        #endregion
    }
}
