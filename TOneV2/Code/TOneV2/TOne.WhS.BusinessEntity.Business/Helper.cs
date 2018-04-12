using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public static class Helper
    {
        public static DateTimeRange GetDateTimeRangeWithOffset(DateTime effectiveDate)
        {
            DateTime effectiveDateWithNoTime = effectiveDate.Date;
            return new DateTimeRange()
            {
                From = effectiveDateWithNoTime,
                To = effectiveDateWithNoTime.AddDays(1)
            };
        }

        public static IEnumerable<int> GetRateTypeIds(int ownerId, long zoneId, DateTime? effectiveDate)
        {
            var rateTypeRuleManager = new Vanrise.GenericData.Pricing.RateTypeRuleManager();
            var rateTypeRuleDefinitionId = new Guid("8A637067-0056-4BAE-B4D5-F80F00C0141B");

            var genericRuleTarget = new Vanrise.GenericData.Entities.GenericRuleTarget
            {
                TargetFieldValues = new Dictionary<string, object>
                {
                    {"CustomerId", ownerId},
                    {"SaleZoneId", zoneId}
                }
            };
            if (effectiveDate.HasValue)
                genericRuleTarget.EffectiveOn = effectiveDate;

            return rateTypeRuleManager.GetRateTypes(rateTypeRuleDefinitionId, genericRuleTarget);
        }
        public static void StructureBusinessEntitiesByDate<T>(List<T> businessEntityList, DateTime fromDate, DateTime toDate, Action<IEnumerable<T>, DateTime, DateTime> onBusinessEntityMatching) where T : IBusinessEntity
        {
            if (businessEntityList == null)
                return;

            HashSet<DateTime> distinctDateTimes = businessEntityList.FindAll(itm => itm.EED.HasValue && itm.EED.Value <= toDate).Select(itm => itm.EED.Value)
                                                  .Union(businessEntityList.FindAll(itm => itm.BED >= fromDate).Select(itm => itm.BED)).Union(new List<DateTime>() { fromDate, toDate })
                                                  .Distinct().OrderBy(itm => itm).ToHashSet();

            List<DateTime> intervalDates = distinctDateTimes.ToList();
            int distinctDateTimesCount = intervalDates.Count;

            for (var index = 0; index < distinctDateTimesCount - 1; index++)
            {
                var effectiveDate = intervalDates[index];
                var eed = intervalDates[index + 1];
                Func<T, bool> predicate = (itm) =>
                {
                    if (itm.BED > effectiveDate)
                        return false;

                    if (itm.EED.HasValue && itm.EED.Value <= effectiveDate)
                        return false;

                    return true;
                };

                IEnumerable<T> matchingBusinessEntities = businessEntityList.FindAllRecords(predicate);
                onBusinessEntityMatching(matchingBusinessEntities, effectiveDate, eed);
            }
        }

        public static T GetBusinessEntityInfo<T>(List<T> businessEntityInfoList, DateTime effectiveOn) where T : IBusinessEntityInfo
        {
            if (businessEntityInfoList == null)
                return default(T);

            Func<T, bool> predicate = (itm) =>
            {
                if (itm.BED > effectiveOn)
                    return false;

                if (itm.EED.HasValue && itm.EED.Value <= effectiveOn)
                    return false;

                return true;
            };
            return businessEntityInfoList.FindRecord(predicate);
        }
    }
}