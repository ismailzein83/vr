using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.Queueing;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GenerateCodesPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }


        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> PreviewCodeQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodesList = this.ImportedCodes.Get(context);
            BaseQueue<IEnumerable<CodePreview>> previewCodeQueue = this.PreviewCodeQueue.Get(context);

            List<CodePreview> codePreviewList = new List<CodePreview>();

            if (importedCodesList != null)
            {
                foreach (ImportedCode item in importedCodesList)
                {
                    if (item.ChangeType == CodeChangeType.NotChanged)
                        continue;

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

            previewCodeQueue.Enqueue(codePreviewList);


        }
    }
}
