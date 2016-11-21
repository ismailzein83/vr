using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{

    public sealed class GenerateCodesPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedCode>> NotImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> PreviewCodeQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CodeToAdd> codesToAdd = this.CodesToAdd.Get(context);
            IEnumerable<CodeToMove> codesToMove = this.CodesToMove.Get(context);
            IEnumerable<CodeToClose> codesToClose = this.CodesToClose.Get(context);

            IEnumerable<NotImportedCode> notImportedCodes = this.NotImportedCodes.Get(context);

            BaseQueue<IEnumerable<CodePreview>> previewCodeQueue = this.PreviewCodeQueue.Get(context);

            List<CodePreview> codePreviewList = new List<CodePreview>();

            if (codesToAdd != null)
            {
                foreach (CodeToAdd item in codesToAdd)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = item.Code,
                        ChangeType = CodeChangeType.New,
                        ZoneName = item.ZoneName,
                        BED = item.BED,
                        EED = item.EED
                    });
                }
            }

            if (codesToMove != null)
            {
                foreach (CodeToMove item in codesToMove)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = item.Code,
                        ChangeType = CodeChangeType.Moved,
                        ZoneName = item.ZoneName,
                        RecentZoneName = item.OldZoneName,
                        BED = GetMovedCodeBED(item),
                        EED = item.BED
                    });
                }
            }

            if (codesToClose != null)
            {
                foreach (CodeToClose item in codesToClose)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = item.Code,
                        ChangeType = CodeChangeType.Deleted,
                        ZoneName = item.ZoneName,
                        BED = GetClosedCodeBED(item),
                        EED = item.CloseEffectiveDate
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
                        ChangeType = CodeChangeType.NotChanged,
                        ZoneName = notImportedCode.ZoneName,
                        BED = notImportedCode.BED,
                        EED = notImportedCode.EED
                    });
                }
            }

            previewCodeQueue.Enqueue(codePreviewList);


        }

        private DateTime GetClosedCodeBED(CodeToClose codeToClose)
        {
            ExistingCode existingCode = codeToClose.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToClose.Code);
            return existingCode.BED;
        }


        private DateTime GetMovedCodeBED(CodeToMove codeToMove)
        {
            return codeToMove.ChangedExistingCodes.Select(item => item.CodeEntity.BED).Min();
        }

    }
}
