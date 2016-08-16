using System.Collections.Generic;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using System.Linq;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareCodesForValidation : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NotImportedCode>> NotImportedCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);
            IEnumerable<ExistingCode> existingCodes = ExistingCodes.Get(context);
            
            HashSet<string> importedCodesHashSet = new HashSet<string>(importedCodes.Select(item => item.Code));
            IEnumerable<NotImportedCode> notImportedCodes = PrepareNotImportedCodes(existingCodes, importedCodesHashSet);


            NotImportedCodes.Set(context, notImportedCodes);
        }


        private IEnumerable<NotImportedCode> PrepareNotImportedCodes(IEnumerable<ExistingCode> existingCodes, HashSet<string> importedCodes)
        {
            Dictionary<string, List<ExistingCode>> notImportedCodesByZoneName = new Dictionary<string, List<ExistingCode>>();

            foreach (ExistingCode existingCode in existingCodes)
            {
                string zoneName = existingCode.ParentZone.Name;
                if (!importedCodes.Contains(existingCode.CodeEntity.Code))
                {
                    List<ExistingCode> existingCodesList = null;
                    if (!notImportedCodesByZoneName.TryGetValue(zoneName, out existingCodesList))
                    {
                        existingCodesList = new List<ExistingCode>();
                        notImportedCodesByZoneName.Add(zoneName, existingCodesList);
                    }
                    existingCodesList.Add(existingCode);
                }
            }

            return notImportedCodesByZoneName.MapRecords(NotImportedCodeInfoMapper);            
        }

        private NotImportedCode NotImportedCodeInfoMapper(List<ExistingCode> existingCodes)
        {
            List<ExistingCode> linkedExistingCodes = existingCodes.GetLinkedEntities();

            NotImportedCode notImportedCode = new NotImportedCode();
            ExistingCode firstElementInTheList = linkedExistingCodes.First();
            ExistingCode lastElementInTheList = linkedExistingCodes.Last();

            notImportedCode.ZoneName = firstElementInTheList.ParentZone.Name;
            notImportedCode.Code = firstElementInTheList.CodeEntity.Code;
            notImportedCode.BED = firstElementInTheList.BED;
            notImportedCode.EED = lastElementInTheList.EED;
            notImportedCode.HasChanged = linkedExistingCodes.Any(x => x.ChangedCode != null);

            return notImportedCode;
        }
    }
}
