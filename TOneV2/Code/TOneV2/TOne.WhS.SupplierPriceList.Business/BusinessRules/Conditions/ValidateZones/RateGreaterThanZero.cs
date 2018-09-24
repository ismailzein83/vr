using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
	class RateGreaterThanZero : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return target is AllImportedDataByZone;
		}
		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			AllImportedDataByZone allImportedDataByZone = context.Target as AllImportedDataByZone;
			var invalidZones = new HashSet<string>();

			foreach (var importedZone in allImportedDataByZone.ImportedDataByZoneList)
			{
				if (importedZone != null && importedZone.ImportedNormalRates != null && importedZone.ImportedNormalRates.Count > 0)
				{
					ImportedRate importedRate = importedZone.ImportedNormalRates.First();
					if (importedRate.Rate < 0)
						invalidZones.Add(importedRate.ZoneName);
				}
			}
			if (invalidZones.Count > 0)
			{
				context.Message = string.Format("Rate is negative for the following zone(s): {0}.", string.Join(", ", invalidZones));
				return false;
			}
			return true;
		}
		public override string GetMessage(IRuleTarget target)
		{
			throw new NotImplementedException();
		}
	}
}
