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
            var excludedItemsDataManager = SalesDataManagerFactory.GetDataManager<IExcludedItemsDataManager>();
            var dataManager = SalesDataManagerFactory.GetDataManager<ISubscriberPreviewDataManager>();
            var subscriberPreviews = dataManager.GetSubscriberPreviews(processInstanceId);
            IEnumerable<long> subscribersProcessInstanceIds = subscriberPreviews.Select(item => item.SubscriberProcessInstanceId);
            IEnumerable<long> excludedItemsProcessInstanceIds = excludedItemsDataManager.GetExcludedItemsProcessInstanceIds(subscribersProcessInstanceIds);
            IEnumerable<SubscriberPreviewDetail> subscriberPreviewDetails = GetMappedSubscriberPreviewsDetails(subscriberPreviews, excludedItemsProcessInstanceIds);
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
        private IEnumerable<SubscriberPreviewDetail> GetMappedSubscriberPreviewsDetails(IEnumerable<SubscriberPreview> subscriberPreviews, IEnumerable<long> excludedItemsProcessInstanceIds)
        {
            List<SubscriberPreviewDetail> subscriberPreviewDetails = new List<SubscriberPreviewDetail>();
            foreach (var subscriberPreview in subscriberPreviews)
            {
                bool hasProcessInstanceId = excludedItemsProcessInstanceIds.Any(item => subscriberPreview.SubscriberProcessInstanceId == item);
                subscriberPreviewDetails.Add(SubscriberPreviewDetailMapper(subscriberPreview, hasProcessInstanceId));
            }
            return subscriberPreviewDetails;
        }
        private SubscriberPreviewDetail SubscriberPreviewDetailMapper(SubscriberPreview entity, bool hasExcludedItems)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var subscriberPreviewDetail = new SubscriberPreviewDetail();
            subscriberPreviewDetail.Entity = entity;
            subscriberPreviewDetail.SubscriberName = carrierAccountManager.GetCarrierAccountName(entity.SubscriberId);
            subscriberPreviewDetail.NumberOfExcludedCountries = GetNumberOfExcludedCountries(entity.ExcludedCountries);
            subscriberPreviewDetail.HasExcludedItems = hasExcludedItems;
            subscriberPreviewDetail.SubscriberProcessInstanceId = entity.SubscriberProcessInstanceId;
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
