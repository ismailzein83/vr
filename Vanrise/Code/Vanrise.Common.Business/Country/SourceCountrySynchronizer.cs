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
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Country> itemsToUpdate)
        {
            throw new NotImplementedException();
        }

        protected override Country BuildItemFromSource(SourceCountry sourceItem)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            throw new NotImplementedException();
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            throw new NotImplementedException();
        }
    }
}
