using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SourceCountrySynchronizer : EntitySynchronization.SourceItemSynchronizer<SourceCountry, Country, SourceCountryReader>
    {
        public SourceCountrySynchronizer(SourceCountryReader sourceCountryReader)
            : base(sourceCountryReader)
        {

        }

        protected override void AddItems(List<Country> itemsToAdd)
        {
            CountryManager countryManager = new CountryManager();
            foreach (var c in itemsToAdd)
            {
                countryManager.AddCountryFromSource(c);
            }
        }

        protected override void UpdateItems(List<Country> itemsToUpdate)
        {
            CountryManager countryManager = new CountryManager();
            foreach (var c in itemsToUpdate)
            {
                countryManager.UpdateCountryFromSource(c);
            }

        }

        protected override Country BuildItemFromSource(SourceCountry sourceItem)
        {

            Country country = new Country
            {
                Name = sourceItem.Name,
                SourceId= sourceItem.SourceId
            };
            return country;
        }
        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            return new CountryManager().GetExistingItemIds(sourceItemIds);
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            CountryManager.ReserveIDRange(nbOfIds, out startingId);
        }
    }
}
