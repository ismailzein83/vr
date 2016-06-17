﻿using System.Collections.Generic;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using System.Linq;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareCodesForValidation : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingCode>> NotImportedCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);
            IEnumerable<ExistingCode> existingCodes = ExistingCodes.Get(context);
            
            HashSet<string> importedCodesHashSet = new HashSet<string>(importedCodes.Select(item => item.Code));
            IEnumerable<ExistingCode> notImportedCodes = PrepareNotImportedCodes(existingCodes, importedCodesHashSet);


            NotImportedCodes.Set(context, notImportedCodes);
        }


        private IEnumerable<ExistingCode> PrepareNotImportedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> importedCodes)
        {
            List<ExistingCode> notImportedCodes = new List<ExistingCode>();

            foreach (ExistingCode existingCode in existingCodes)
            {
                if (existingCode.ChangedCode != null &&  !importedCodes.Contains(existingCode.CodeEntity.Code))
                    notImportedCodes.Add(existingCode);
            }

            return notImportedCodes;
        }
    }
}
