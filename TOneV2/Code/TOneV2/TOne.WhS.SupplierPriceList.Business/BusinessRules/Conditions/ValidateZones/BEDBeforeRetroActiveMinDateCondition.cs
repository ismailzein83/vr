﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
	public class BEDBeforeRetroActiveMinDateCondition : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return (target as ImportedDataByZone != null);
		}

		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			ImportedDataByZone zone = context.Target as ImportedDataByZone;

			int retroactiveDayOffset = new TOne.WhS.BusinessEntity.Business.ConfigManager().GetPurchaseAreaRetroactiveDayOffset();
			DateTime minimumRetroActiveDate = DateTime.Today.AddDays(-retroactiveDayOffset).Date;

			foreach (var importedCode in zone.ImportedCodes)
			{
				if (importedCode.BED < minimumRetroActiveDate)
					return false;
			}

			foreach (var importedNormalRate in zone.ImportedNormalRates)
			{
				if (importedNormalRate.BED < minimumRetroActiveDate)
					return false;
			}

			foreach (var importedOtherRate in zone.ImportedOtherRates)
			{
				if (importedOtherRate.Value.Count > 0)
				{
					foreach (var otherRate in importedOtherRate.Value)
					{
						if (otherRate.BED < minimumRetroActiveDate)
							return false;
					}
				}
			}

			foreach (var importedZoneServiceGroup in zone.ImportedZoneServicesToValidate)
			{
				if (importedZoneServiceGroup.Value.Count > 0)
				{
					foreach (var serviceGroup in importedZoneServiceGroup.Value)
					{
						if (serviceGroup.BED < minimumRetroActiveDate)
							return false;
					}
				}
			}


			return true;
		}

		public override string GetMessage(IRuleTarget target)
		{
			return string.Format("Zone {0} has a code, rate, or service with BED less than minimum retro active date", (target as ImportedDataByZone).ZoneName);
		}
	}
}
