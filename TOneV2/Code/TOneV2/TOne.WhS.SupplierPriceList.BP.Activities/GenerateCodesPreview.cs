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
        public InArgument<IEnumerable<NotImportedCode>> NotImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> PreviewCodeQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);
            IEnumerable<NotImportedCode> notImportedCodes = this.NotImportedCodes.Get(context);

            BaseQueue<IEnumerable<CodePreview>> previewCodeQueue = this.PreviewCodeQueue.Get(context);

            List<CodePreview> codePreviewList = new List<CodePreview>();

            if (importedCodes != null)
            {
                foreach (ImportedCode item in importedCodes)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = item.Code,
                        ChangeType = GetCodeChangeType(item),
                        RecentZoneName = item.ProcessInfo.RecentZoneName,
                        ZoneName = item.ZoneName,
                        BED = item.BED,
                        EED = item.EED,
                        IsExcluded = item.IsExcluded
                    });
                }
            }


            if (notImportedCodes != null)
            {
                foreach (NotImportedCode notImportedCode in notImportedCodes)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = notImportedCode.Code,
                        ChangeType = notImportedCode.HasChanged ? CodeChangeType.Deleted : CodeChangeType.NotChanged,
                        ZoneName = notImportedCode.ZoneName,
                        BED = notImportedCode.BED,
                        EED = notImportedCode.EED
                    });
                }
            }

            previewCodeQueue.Enqueue(codePreviewList);
        }


        private CodeChangeType GetCodeChangeType(ImportedCode importedCode)
        {
            if (importedCode.ChangeType == CodeChangeType.New && importedCode.ChangedExistingCodes.Count() > 0)
            {
                ExistingCode existingCode = importedCode.ChangedExistingCodes.OrderBy(item => item.BED).First();
                if (existingCode.EED.HasValue && existingCode.EED.Value.Equals(importedCode.BED))
                    return CodeChangeType.NotChanged;
            }

            return importedCode.ChangeType;
        }
    }
}
