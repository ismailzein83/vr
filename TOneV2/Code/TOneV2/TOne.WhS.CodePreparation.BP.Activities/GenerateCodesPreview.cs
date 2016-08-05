using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
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
        public InArgument<IEnumerable<ExistingCode>> NotChangedCodes { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<IEnumerable<CodePreview>>> PreviewCodeQueue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CodeToAdd> codesToAdd = this.CodesToAdd.Get(context);
            IEnumerable<CodeToMove> codesToMove = this.CodesToMove.Get(context);
            IEnumerable<CodeToClose> codesToClose = this.CodesToClose.Get(context);

            IEnumerable<ExistingCode> notChangedCodes = this.NotChangedCodes.Get(context);

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


            if (notChangedCodes != null)
            {
                foreach (ExistingCode notChangedCode in notChangedCodes)
                {
                    codePreviewList.Add(new CodePreview()
                    {
                        Code = notChangedCode.CodeEntity.Code,
                        ChangeType = CodeChangeType.NotChanged,
                        ZoneName = notChangedCode.ParentZone.ZoneEntity.Name,
                        BED = notChangedCode.BED,
                        EED = notChangedCode.CodeEntity.EED
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
