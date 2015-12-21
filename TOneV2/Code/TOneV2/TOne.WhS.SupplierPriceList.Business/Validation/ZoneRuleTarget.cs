using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ZoneRuleTarget : BusinessRule<ImportedZone>
    {
        public override void SetExecluded()
        {
            CodeGroupManager codeGroupManager = new CodeGroupManager();

            foreach (ImportedZone zone in base.data)
            {
                if (zone.ImportedCodes != null)
                {
                    var firstCode = zone.ImportedCodes.FirstOrDefault();
                    if (firstCode != null && !firstCode.IsExecluded)
                    {
                        int countryIdOfFirstCode = firstCode.CodeGroupId != null ? codeGroupManager.GetCodeGroup(firstCode.CodeGroupId.Value).CountryId : -1;
                        Func<ImportedCode, bool> pred = new Func<ImportedCode, bool>((code) =>
                        !code.IsExecluded && codeGroupManager.GetCodeGroup(code.CodeGroupId.Value) != null
                            && codeGroupManager.GetCodeGroup(code.CodeGroupId.Value).CountryId != countryIdOfFirstCode);

                        if (zone.ImportedCodes.Any(pred))
                        {
                            zone.IsExecluded = true;
                            foreach (ImportedCode code in zone.ImportedCodes)
                                code.IsExecluded = true;
                        }
                    }
                }
            }
        }

        public override bool isValid()
        {
            throw new NotImplementedException();
        }

    }
}
