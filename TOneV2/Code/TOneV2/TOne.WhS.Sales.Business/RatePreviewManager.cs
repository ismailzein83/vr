using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
    public class RatePreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RatePreviewDetail> GetFilteredRatePreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RatePreviewRequestHandler());
        }

        #region Private Classes

        private class RatePreviewRequestHandler : BigDataRequestHandler<RatePlanPreviewQuery, RatePreview, RatePreviewDetail>
        {
            public override RatePreviewDetail EntityDetailMapper(RatePreview entity)
            {
                var entityDetail = new RatePreviewDetail()
                {
                    Entity = entity
                };

                entityDetail.ChangeTypeDescription = Utilities.GetEnumAttribute<RateChangeType, DescriptionAttribute>(entity.ChangeType).Description;
                
                return entityDetail;
            }

            public override IEnumerable<RatePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IRatePreviewDataManager>();
                return dataManager.GetRatePreviews(input.Query);
            }
        }

        #endregion
    }
}
