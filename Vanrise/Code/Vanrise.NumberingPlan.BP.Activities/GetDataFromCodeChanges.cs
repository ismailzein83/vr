using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Entities;
using Vanrise.NumberingPlan.Business;

namespace Vanrise.NumberingPlan.BP.Activities
{

    public sealed class GetDataFromCodeChanges : CodeActivity
    {

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<Changes> Changes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> MinimumDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Changes changes = Changes.Get(context);
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            List<CodeToAdd> codesToAdd = new List<CodeToAdd>();
            List<CodeToMove> codesToMove = new List<CodeToMove>();
            List<CodeToClose> codesToClose = new List<CodeToClose>();
            DateTime minimumDate = EffectiveDate.Get(context);
            foreach (NewCode code in changes.NewCodes)
            {
                CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);

                if (code.OldZoneName != null)
                {
                    codesToMove.Add(new CodeToMove
                    {
                        Code = code.Code,
                        CodeGroup = codeGroup,
                        BED = minimumDate,
                        EED = null,
                        ZoneName = code.ZoneName,
                        OldZoneName = code.OldZoneName
                    });
                }
                else
                {
                    codesToAdd.Add(new CodeToAdd
                    {
                        Code = code.Code,
                        CodeGroup = codeGroup,
                        BED = minimumDate,
                        EED = null,
                        ZoneName = code.ZoneName,
                    });
                }

            }

            foreach (DeletedCode code in changes.DeletedCodes)
            {
                CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                codesToClose.Add(new CodeToClose
                {
                    Code = code.Code,
                    CodeGroup = codeGroup,
                    CloseEffectiveDate = minimumDate,
                    ZoneName = code.ZoneName,
                });
            }

            CodesToAdd.Set(context, codesToAdd);
            CodesToMove.Set(context, codesToMove);
            CodesToClose.Set(context, codesToClose);
            MinimumDate.Set(context, minimumDate);
        }
    }
}
