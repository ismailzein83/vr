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

            IEnumerable<SubscriberPreviewDetail> subscriberPreviewDetails = subscriberPreviews.MapRecords(SubscriberPreviewDetail);
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

        private SubscriberPreviewDetail SubscriberPreviewDetail(SubscriberPreview entity)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var subscriberPreviewDetail = new SubscriberPreviewDetail();
            subscriberPreviewDetail.Entity = entity;
            subscriberPreviewDetail.SubscriberName = carrierAccountManager.GetCarrierAccountName(entity.SubscriberId);
            return subscriberPreviewDetail;
        }

    }
}
