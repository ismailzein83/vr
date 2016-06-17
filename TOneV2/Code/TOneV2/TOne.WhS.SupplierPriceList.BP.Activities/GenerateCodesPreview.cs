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
        public InArgument<IEnumerable<ExistingCode>> NotImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> PreviewCodeQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);
            IEnumerable<ExistingCode> notImportedCodes = this.NotImportedCodes.Get(context);

            BaseQueue<IEnumerable<CodePreview>> previewCodeQueue = this.PreviewCodeQueue.Get(context);

            List<CodePreview> codePreviewList = new List<CodePreview>();

            if (importedCodes != null)
            {
                foreach (ImportedCode item in importedCodes)
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


            if (notImportedCodes != null)
            {
                foreach (ExistingCode notImportedCode in notImportedCodes)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = notImportedCode.CodeEntity.Code,
                        ChangeType = CodeChangeType.Deleted,
                        ZoneName = notImportedCode.ParentZone.ZoneEntity.Name,
                        BED = notImportedCode.BED,
                        EED = notImportedCode.ChangedCode.EED
                    });
                }
            }

            previewCodeQueue.Enqueue(codePreviewList);


        }
    }
}
