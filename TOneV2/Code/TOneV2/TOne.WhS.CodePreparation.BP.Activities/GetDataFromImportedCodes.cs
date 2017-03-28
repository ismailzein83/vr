﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using System.IO;
using Aspose.Cells;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class GetDataFromImportedCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = EffectiveDate.Get(context);
            BusinessEntity.Business.CodeGroupManager codeGroupManager = new BusinessEntity.Business.CodeGroupManager();

            List<CodeToAdd> codesToAdd = new List<CodeToAdd>();
            List<CodeToMove> codesToMove = new List<CodeToMove>();
            List<CodeToClose> codesToClose = new List<CodeToClose>();

            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);

            ImportedData importedData = new ImportedData();
            importedData.NewCodes = new List<NewCode>();
            importedData.DeletedCodes = new List<DeletedCode>();

            foreach (ImportedCode importedCode in importedCodes)
            {
                switch (importedCode.Status)
                {
                    case ImportType.New:
                        importedData.NewCodes.Add(new NewCode
                        {
                            Code = importedCode.Code,
                            Zone = importedCode.ZoneName,
                            BED = effectiveDate,
                            EED = null
                        });
                        break;
                    case ImportType.Delete:
                        importedData.DeletedCodes.Add(new DeletedCode
                        {
                            Code = importedCode.Code,
                            Zone = importedCode.ZoneName,
                            BED = effectiveDate,
                            EED = null
                        });
                        break;
                }
            }

            foreach (NewCode code in importedData.NewCodes)
            {
                TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code);
                DeletedCode deletedCode = importedData.DeletedCodes.FirstOrDefault(x => x.Code == code.Code);
                if (deletedCode != null)
                {
                    codesToMove.Add(new CodeToMove
                           {
                               Code = code.Code,
                               CodeGroup = codeGroup,
                               BED = code.BED,
                               EED = code.EED,
                               ZoneName = code.Zone,
                               OldZoneName = deletedCode.Zone,
                               OldCodeBED = deletedCode.BED
                           });
                }
                else
                {
                    codesToAdd.Add(new CodeToAdd
                           {
                               Code = code.Code,
                               CodeGroup = codeGroup,
                               BED = code.BED,
                               EED = code.EED,
                               ZoneName = code.Zone,
                           });
                }

            }
            foreach (DeletedCode code in importedData.DeletedCodes)
            {
                if (codesToMove.Any(x => x.Code == code.Code) && importedData.DeletedCodes.FindAllRecords(item => item.Code == code.Code).Count() == 1)
                    continue;

                TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup = codeGroupManager.GetMatchCodeGroup(code.Code.Trim());
                codesToClose.Add(new CodeToClose
                       {
                           Code = code.Code,
                           CloseEffectiveDate = code.BED,
                           ZoneName = code.Zone,
                           CodeGroup = codeGroup
                       });
            }


            CodesToAdd.Set(context, codesToAdd);
            CodesToMove.Set(context, codesToMove);
            CodesToClose.Set(context, codesToClose);
        }


        private class ImportedData
        {
            public List<NewCode> NewCodes { get; set; }
            public List<DeletedCode> DeletedCodes { get; set; }
        }
        private class NewCode
        {
            public string Code { get; set; }
            public string Zone { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }
        private class DeletedCode
        {
            public string Code { get; set; }
            public string Zone { get; set; }
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }

    }
}
