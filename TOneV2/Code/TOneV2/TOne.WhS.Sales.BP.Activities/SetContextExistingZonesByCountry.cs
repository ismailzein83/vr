using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetContextExistingZonesByCountry : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        public InArgument<AllCustomerCountriesToAdd> AllCustomerCountriesToAdd { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);
            AllCustomerCountriesToAdd allCustomerCountriesToAdd = AllCustomerCountriesToAdd.Get(context);

            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

            ratePlanContext.ExistingZonesByCountry = new Dictionary<int, List<ExistingZone>>();
            ratePlanContext.EffectiveAndFutureExistingZonesByCountry = new Dictionary<int, List<ExistingZone>>();

            if (existingZones == null || existingZones.Count() == 0)
                return;

            Dictionary<int, CustomerCountryToAdd> countiesToAddByCountryId = StructureCountiesToAddByCountryId(allCustomerCountriesToAdd);
            foreach (ExistingZone existingZone in existingZones)
            {
                List<ExistingZone> countryZones;

                if (!ratePlanContext.ExistingZonesByCountry.TryGetValue(existingZone.CountryId, out countryZones))
                {
                    countryZones = new List<ExistingZone>();
                    ratePlanContext.ExistingZonesByCountry.Add(existingZone.CountryId, countryZones);
                }

                countryZones.Add(existingZone);

                DateTime dateToCompare = ratePlanContext.EffectiveDate;
                var countryToAdd = countiesToAddByCountryId.GetRecord(existingZone.CountryId);
                if (countryToAdd != null)
                    dateToCompare = countryToAdd.BED;

                if (existingZone.EED.HasValue && existingZone.EED.Value <= dateToCompare)
                    continue;

                List<ExistingZone> effectiveAndFutureCountryZones;

                if (!ratePlanContext.EffectiveAndFutureExistingZonesByCountry.TryGetValue(existingZone.CountryId, out effectiveAndFutureCountryZones))
                {
                    effectiveAndFutureCountryZones = new List<ExistingZone>();
                    ratePlanContext.EffectiveAndFutureExistingZonesByCountry.Add(existingZone.CountryId, effectiveAndFutureCountryZones);
                }

                effectiveAndFutureCountryZones.Add(existingZone);
            }
        }
        private Dictionary<int, CustomerCountryToAdd> StructureCountiesToAddByCountryId(AllCustomerCountriesToAdd allCustomerCountriesToAdd)
        {
            var countiesToAddByCountryId = new Dictionary<int, CustomerCountryToAdd>();
            foreach (var countryToAdd in allCustomerCountriesToAdd.CustomerCountriesToAdd)
            {
                if (!countiesToAddByCountryId.ContainsKey(countryToAdd.CountryId))
                    countiesToAddByCountryId.Add(countryToAdd.CountryId, countryToAdd);
            }
            return countiesToAddByCountryId;
        }
    }
}
