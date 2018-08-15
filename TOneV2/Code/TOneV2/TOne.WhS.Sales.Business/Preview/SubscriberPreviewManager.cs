using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class SubscriberPreviewManager
    {
        public SubscriberPreviewObject GetSubscriberPreview(long processInstanceId)
        {
            var dataManager = SalesDataManagerFactory.GetDataManager<ISubscriberPreviewDataManager>();
            var subscriberPreviews = dataManager.GetSubscriberPreviews(processInstanceId);

            IEnumerable<SubscriberPreviewDetail> subscriberPreviewDetails = subscriberPreviews.MapRecords(SubscriberPreviewDetailMapper);
            SubscriberPreviewSummary subscriberPreviewSummary = new SubscriberPreviewSummary
            {
                NumberOfSubscriberWithSuccessStatus = subscriberPreviews.Count(item => item.Status == SubscriberProcessStatus.Success),
                NumberOfSubscriberWithNoChangeStatus = subscriberPreviews.Count(item => item.Status == SubscriberProcessStatus.NoChange),
                NumberOfSubscriberWithFailedStatus = subscriberPreviews.Count(item => item.Status == SubscriberProcessStatus.Failed),
            };

            return new SubscriberPreviewObject
            {
                SubscriberPreviewDetails = subscriberPreviewDetails,
                SubscriberPreviewSummary = subscriberPreviewSummary
            };
        }

        private SubscriberPreviewDetail SubscriberPreviewDetailMapper(SubscriberPreview entity)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var subscriberPreviewDetail = new SubscriberPreviewDetail();
            subscriberPreviewDetail.Entity = entity;
            subscriberPreviewDetail.SubscriberName = carrierAccountManager.GetCarrierAccountName(entity.SubscriberId);
            subscriberPreviewDetail.NumberOfExcludedCountries = GetNumberOfExcludedCountries(entity.ExcludedCountries);
            return subscriberPreviewDetail;
        }

        private int GetNumberOfExcludedCountries(List<ExcludedChange> excludedCountries)
        {
            int numberOfExcludedCountries = 0;
            if (excludedCountries != null)
            {
                foreach (var excludedCountry in excludedCountries)
                {
                    numberOfExcludedCountries += excludedCountry.CountryIds.Count();
                }
            }
            return numberOfExcludedCountries;
        }

    }
}
