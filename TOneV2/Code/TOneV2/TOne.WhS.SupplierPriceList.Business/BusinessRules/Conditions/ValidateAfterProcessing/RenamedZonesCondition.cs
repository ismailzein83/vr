using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class RenamedZonesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            //List<string> renamedZones = new List<string>();
            //AllImportedZones allImportedZones = context.Target as AllImportedZones;
            //if (allImportedZones.Zones == null || allImportedZones.Zones.Count() == 0)
            //    return true;
            //foreach (var importedZone in allImportedZones.Zones)
            //{
            //    if (importedZone.ChangeType == ZoneChangeType.Renamed)
            //        renamedZones.Add(importedZone.ZoneName);
            //}
            //if (renamedZones.Count() > 0)
            //{
            //    context.Message = string.Format("The following zone(s) has been renamed :'{0}'", string.Join(",", renamedZones));
            //    return false;
            //}

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }




    public class ModifyingCodesInRateChangePricelistCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            //IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            //if (importSPLContext.SupplierPricelistType == BusinessEntity.Entities.SupplierPricelistType.RateChange)
            //{
            //    List<string> zonesWithCodeChange = new List<string>();
            //    AllImportedZones allImportedZones = context.Target as AllImportedZones;
            //    foreach (var importedZone in allImportedZones.Zones)
            //    {
            //        if (importedZone.ImportedCodes != null && importedZone.ImportedCodes.Count() > 0)
            //            zonesWithCodeChange.Add(importedZone.ZoneName);
            //    }
            //    if (zonesWithCodeChange.Count() > 0)
            //    {
            //        context.Message = string.Format("Cannot change code(s) in RateChange pricelist on following zone(s) :'{0}'", string.Join(",", zonesWithCodeChange));
            //        return false;
            //    }
            //}
            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }




    public class ClosingZonesAboveThresholdCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            //AllImportedZones allImportedZones = context.Target as AllImportedZones;
            //List<NotImportedZone> notImportedZones = context.Target as List<NotImportedZone>;
            //if (allImportedZones != null && notImportedZones != null && (notImportedZones.Count() > allImportedZones.Zones.Count()))
            //    return false;
            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
