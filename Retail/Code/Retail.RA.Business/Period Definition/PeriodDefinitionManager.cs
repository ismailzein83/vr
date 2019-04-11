using System;
using System.Collections.Generic;
using System.Linq;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PeriodDefinitionManager
    {
        static Guid beDefinitionId = new Guid("74A31C7E-E38C-46DE-960C-97A38A7E7107");

        #region Public Methods

        public PeriodDefinition GetPeriodDefinition(int periodDefinitionId)
        {
            var cachedPeriodDefinitions = GetCachedPeriodDefinitions();
            return cachedPeriodDefinitions.GetRecord(periodDefinitionId);
        }

        public PeriodDefinition GetLastPeriod(DateTime date)
        {
            var cachedPeriodDefinition = GetCachedPeriodDefinitions();
            if (cachedPeriodDefinition == null || cachedPeriodDefinition.Count == 0)
                return null;

            var orderedCachedPeriodDefinitions = cachedPeriodDefinition.OrderBy(item => item.Value.FromDate);

            var periodDefinitionList = orderedCachedPeriodDefinitions.Select(item => item.Value).ToList();

            if (periodDefinitionList.Last().ToDate < date)
                return periodDefinitionList.Last();

            for (int i = 0; i < periodDefinitionList.Count(); i++)
            {
                var periodDefiniton = periodDefinitionList[i];

                if (periodDefiniton.FromDate > date || (date > periodDefiniton.FromDate && date < periodDefiniton.ToDate))
                {
                    if (i > 0)
                        return periodDefinitionList[i - 1];
                }
            }
            return null;
        }
        public PeriodDefinition GetCurrentOrNextPeriod(DateTime date)
        {
            var periodDefinitionId = GetPeriodDefinitionIdByDay(date);
            var cachedPeriodDefinition = GetCachedPeriodDefinitions();
            if (cachedPeriodDefinition == null || cachedPeriodDefinition.Count == 0)
                return null;
            if (periodDefinitionId != null)
                return cachedPeriodDefinition.GetRecord(periodDefinitionId.Value);

            var orderedCachedPeriodDefinitions = cachedPeriodDefinition.OrderBy(item => item.Value.FromDate);

            var periodDefinitionList = orderedCachedPeriodDefinitions.Select(item => item.Value).ToList();

            for (int i = 0; i < periodDefinitionList.Count(); i++)
            {
                var periodDefiniton = periodDefinitionList[i];

                if (periodDefiniton.FromDate > date)
                    return periodDefiniton;
            }
            return null;
        }

        public Dictionary<int, PeriodDefinition> GetAllPeriodDefinitionsById()
        {
            var cachedPeriods = GetCachedPeriodDefinitions();
            cachedPeriods.ThrowIfNull("cachedPriods");
            return cachedPeriods;
        }
        public int? GetPeriodDefinitionIdByDay(DateTime day)
        {
            var cachedPeriodDefinition = GetCachedPeriodDefinitions();

            if (cachedPeriodDefinition == null)
                return null;

            foreach (var periodDefinition in cachedPeriodDefinition.Values)
            {
                if (day >= periodDefinition.FromDate && day <= periodDefinition.ToDate)
                    return periodDefinition.PeriodDefinitionId;
            }

            return null;
        }

        public List<PeriodDefinition> GetPeriodDefinitionsBetweenDate(DateTime fromDate, DateTime toDate, out DateTime minDate, out DateTime maxDate)
        {
            var cachedPeriods = GetCachedPeriodDefinitions();
            minDate = new DateTime();
            maxDate = new DateTime();

            if (cachedPeriods == null || cachedPeriods.Count == 0)
                return null;

            var periodDefinitions = new List<PeriodDefinition>();
            foreach (var periodDefinition in cachedPeriods.Values)
            {
                if (periodDefinition.ToDate >= fromDate && periodDefinition.FromDate <= toDate)
                    periodDefinitions.Add(periodDefinition);
            }

            if (periodDefinitions.Count > 0)
            {
                minDate = periodDefinitions.Min(item => item.FromDate);
                maxDate = periodDefinitions.Max(item => item.ToDate);
            }

            return periodDefinitions;
        }
        #endregion

        #region Private Methods
        private Dictionary<int, PeriodDefinition> GetCachedPeriodDefinitions()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedPeriodDefinitions", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<int, PeriodDefinition> periodDefinitionById = new Dictionary<int, PeriodDefinition>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        PeriodDefinition periodDefinition = new PeriodDefinition
                        {
                            PeriodDefinitionId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Period = genericBusinessEntity.FieldValues.GetRecord("Period") as string,
                            FromDate = (DateTime)genericBusinessEntity.FieldValues.GetRecord("From"),
                            ToDate = (DateTime)genericBusinessEntity.FieldValues.GetRecord("To")
                        };
                        periodDefinitionById.Add(periodDefinition.PeriodDefinitionId, periodDefinition);
                    }
                }

                return periodDefinitionById;
            });
        }
        public bool IsPeriodOverlapping(DateTime bed, DateTime eed, int? periodDefinitionId)
        {
            var cacehdPeriodDefintions = GetCachedPeriodDefinitions();
            List<PeriodDefinition> overlappingPeriodDefinitions = new List<PeriodDefinition>();
            if (cacehdPeriodDefintions != null)
            {
                var periodDefinitions = cacehdPeriodDefintions.Values;
                periodDefinitions.ThrowIfNull("periodDefinitions");
                foreach (var periodDefinition in periodDefinitions)
                {
                    if (periodDefinition.ToDate >= bed && eed >= periodDefinition.FromDate && periodDefinition.FromDate != periodDefinition.ToDate && (periodDefinitionId == null || periodDefinitionId.Value != periodDefinition.PeriodDefinitionId))
                        overlappingPeriodDefinitions.Add(periodDefinition);
                }
            }
            if (overlappingPeriodDefinitions.Count() > 0)
                return true;
            return false;
        }

        #endregion
    }
}
