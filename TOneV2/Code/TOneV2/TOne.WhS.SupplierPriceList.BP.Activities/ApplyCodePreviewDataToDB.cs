using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyCodePreviewDataToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<NewCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }


        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewCode> newCodeList = this.NewCodes.Get(context);
            IEnumerable<ImportedCode> importedCodesList = this.ImportedCodes.Get(context);
            int priceListId = this.PriceListId.Get(context);

            List<CodePreview> codePreviewList = new List<CodePreview>();

            if(importedCodesList != null)
            {
                foreach (ImportedCode item in importedCodesList)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                         Code = item.Code,
                         ChangeType = item.ChangeType,
                         RecentZoneName = item.ProcessInfo.RecentZoneName,
                         ZoneName = item.ZoneName,
                         BED = item.BED,
                         EED = item.EED
                    });
                }
            }

            SupplierCodePreviewManager manager = new SupplierCodePreviewManager();
            manager.Insert(priceListId, codePreviewList);
        }
    }
}
