using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities.PreviewData;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class ApplyRatePreviewDataToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedRate> importedRatesList = this.ImportedRates.Get(context);
            int priceListId = this.PriceListId.Get(context);

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

            SupplierRatePreviewManager manager = new SupplierRatePreviewManager();
            manager.Insert(priceListId, ratePreviewList);
        }
    }
}
