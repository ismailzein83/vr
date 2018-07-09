using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;


namespace TOne.WhS.SupplierPriceList.Business
{
    public class ClosingZonesAboveThresholdCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllZones;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            AllZones allZones = context.Target as AllZones;
            if (allZones != null)
            {
                IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
                if (importSPLContext.SupplierPricelistType == SupplierPricelistType.Full)
                {
                    if (allZones.ImportedZones != null && allZones.NotImportedZones != null)
                    {
                        var configManager = new ConfigManager();
                        int acceptableZoneClosingPercentage = 0;// configManager.GetPurchaseAcceptableZoneClosingPercentage();
                        double notImportedZoneCount = allZones.NotImportedZones.Where(a => a.HasChanged).Count();
                        double importedZoneCount = allZones.ImportedZones.Zones.Count();

                        double percentageOfClosing = (notImportedZoneCount / (notImportedZoneCount + importedZoneCount)) * 100;
                        if (percentageOfClosing > acceptableZoneClosingPercentage)
                        {
                            context.Message = string.Format("More than {0}% of zones will be closed", acceptableZoneClosingPercentage);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
