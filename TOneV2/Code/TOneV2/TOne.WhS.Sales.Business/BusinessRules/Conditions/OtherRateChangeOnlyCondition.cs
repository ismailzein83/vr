﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business
{
    public class OtherRateChangeOnlyCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
			// Validate if there is only other rate changes
            //var allZoneData = context.Target as AllDataByZone;
            //var ratePlanContext = context.GetExtension<IRatePlanContext>();
            //if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            //{
            //    List<string> zoneWithOtherRateChangeOnly = new List<string>();
            //    foreach (var zoneData in allZoneData.DataByZoneList)
            //    {
            //        var rateTypeIds = Helper.GetRateTypeIds(ratePlanContext.OwnerId,zoneData.ZoneId, DateTime.Now);
            //        foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
            //        {
            //            if (rateTypeIds.Contains(otherRateToChange.RateTypeId.Value) && zoneData.OtherRatesToChange.Count > 0 && zoneData.NormalRateToChange == null )
            //            {
            //                zoneWithOtherRateChangeOnly.Add(zoneData.ZoneName);
            //                break;
            //            }
            //        }
            //    }
            //    if (zoneWithOtherRateChangeOnly.Count() > 0)
            //    {
            //        context.Message = string.Format("Zone(s): '{0}' have other rates changes without normal rate changes", string.Join(",", zoneWithOtherRateChangeOnly));
            //        return false;
            //    }
            //}
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
