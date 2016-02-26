using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Queueing;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class GenerateRatesPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }


        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<RatePreview>>> PreviewRateQueue { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedRate> importedRatesList = this.ImportedRates.Get(context);
            BaseQueue<IEnumerable<RatePreview>> previewRateQueue = this.PreviewRateQueue.Get(context);

            List<RatePreview> ratePreviewList = new List<RatePreview>();

            if (importedRatesList != null)
            {
                foreach (ImportedRate item in importedRatesList)
                {
                    if (item.ChangeType == RateChangeType.NotChanged)
                        continue;

                    ratePreviewList.Add(new RatePreview()
                    {
                        ZoneName = item.ZoneName,
                        ChangeType = item.ChangeType,
                        RecentRate = item.ProcessInfo.RecentRate,
                        NewRate = item.NormalRate,
                        BED = item.BED,
                        EED = item.EED
                    });
                }
            }

            previewRateQueue.Enqueue(ratePreviewList);
        }
    }
}
