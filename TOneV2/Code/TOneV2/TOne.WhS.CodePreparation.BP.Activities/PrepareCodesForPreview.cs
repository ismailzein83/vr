using System.Collections.Generic;
using System.Activities;
using System.Linq;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class PrepareCodesForPreview : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingCode>> NotChangedCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CodeToAdd> codesToAdd = CodesToAdd.Get(context);
            IEnumerable<CodeToMove> codesToMove = CodesToMove.Get(context);
            IEnumerable<CodeToClose> codesToClose = CodesToClose.Get(context);
            
            IEnumerable<ExistingCode> existingCodes = ExistingCodes.Get(context);

            HashSet<string> codesToAddHashSet = new HashSet<string>(codesToAdd.Select(item => item.Code));
            HashSet<string> codesToMoveHashSet = new HashSet<string>(codesToMove.Select(item => item.Code));
            HashSet<string> codesToCloseHashSet = new HashSet<string>(codesToClose.Select(item => item.Code));

            IEnumerable<ExistingCode> notImportedCodes = PrepareNotImportedCodes(existingCodes, codesToAddHashSet, codesToMoveHashSet, codesToCloseHashSet);

            NotChangedCodes.Set(context, notImportedCodes);
        }


        private IEnumerable<ExistingCode> PrepareNotImportedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> codesToAddHashSet, HashSet<string> codesToMoveHashSet, HashSet<string> codesToCloseHashSet)
        {
            List<ExistingCode> notChangedCodes = new List<ExistingCode>();

            foreach (ExistingCode existingCode in existingCodes)
            {
                if (!(codesToAddHashSet.Contains(existingCode.CodeEntity.Code) || codesToMoveHashSet.Contains(existingCode.CodeEntity.Code)
                    || codesToCloseHashSet.Contains(existingCode.CodeEntity.Code)))
                    notChangedCodes.Add(existingCode);
            }

            return notChangedCodes;
        }
    }
}
