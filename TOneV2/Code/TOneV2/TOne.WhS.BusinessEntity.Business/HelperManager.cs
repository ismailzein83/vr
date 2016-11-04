using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public static class HelperManager
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

        public static void StructureBusinessEntitiesByDate<T>(List<T> businessEntityList, Action<IEnumerable<T>, DateTime, DateTime?> onBusinessEntityMatching) where T : IBusinessEntity
        {
            if (businessEntityList == null)
                return;

            List<DateTime> distinctDateTimes = businessEntityList.FindAll(itm => itm.EED.HasValue).Select(itm => itm.EED.Value).Union(businessEntityList.Select(itm => itm.BED)).Distinct().OrderBy(itm => itm).ToList();

            int distinctDateTimesCount = distinctDateTimes.Count;

            for (var index = 0; index < distinctDateTimesCount; index++)
            {
                var bed = distinctDateTimes[index];
                DateTime? eed = index == distinctDateTimesCount - 1 ? (DateTime?)null : distinctDateTimes[index + 1];

                Func<T, bool> predicate = (itm) =>
                {
                    if (itm.BED > bed)
                        return false;

                    if (itm.EED.HasValue && (!eed.HasValue || itm.EED.Value < eed.Value))
                        return false;

                    if (!itm.EED.HasValue && eed.HasValue)
                        return false;

                    return true;
                };

                IEnumerable<T> matchingBusinessEntities = businessEntityList.FindAllRecords(predicate);
                onBusinessEntityMatching(matchingBusinessEntities, bed, eed);
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