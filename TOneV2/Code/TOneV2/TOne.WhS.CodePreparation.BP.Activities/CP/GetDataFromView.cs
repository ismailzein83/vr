using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities.CP;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GetDataFromView : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }
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
            DateTime startReading = DateTime.Now;
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            CodePreparationManager codePreparationManager = new CodePreparationManager();
            Changes changes = codePreparationManager.GetChanges(sellingNumberPlanId);

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
                               OldZoneName = code.OldZoneName,
                               IsExcluded = code.IsExcluded
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
                    codesToClose.Add(new CodeToClose
                           {
                               Code = code.Code,
                               
                               CloseEffectiveDate = minimumDate,
                               ZoneName = code.ZoneName,
                           });
            }

            TimeSpan spent = DateTime.Now.Subtract(startReading);
            context.WriteTrackingMessage(LogEntryType.Information, "Converting Data to Work Flow Stracture done and Takes: {0}", spent);

            CodesToAdd.Set(context, codesToAdd);
            CodesToMove.Set(context, codesToMove);
            CodesToClose.Set(context, codesToClose);
            MinimumDate.Set(context, minimumDate);
        }
    }
}
