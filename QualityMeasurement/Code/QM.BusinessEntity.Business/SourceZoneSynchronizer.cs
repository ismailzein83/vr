using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{  

    public class SourceZone : ISourceItem
    {
        public string SourceId { get; set; }

        public string CountryName { get; set; }

        public string SourceCountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }

    

    public class SourceZoneSynchronizer : SourceItemSynchronizer<SourceZone, Zone, ISourceItemReader<SourceZone>>
    {
        Vanrise.Common.Business.CountryManager countryManager = new Vanrise.Common.Business.CountryManager();
        ZoneManager zoneManager = new ZoneManager();

        public SourceZoneSynchronizer(ISourceItemReader<SourceZone> sourceItemReader)
            : base(sourceItemReader)
        {

        }

        protected override void AddItems(List<Zone> itemsToAdd)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateItems(List<Zone> itemsToUpdate)
        {
            throw new NotImplementedException();
        }

        protected override Zone BuildItemFromSource(SourceZone sourceItem)
        {
            Zone zone = new Zone
            {
                Name = sourceItem.Name,
                BeginEffectiveDate = sourceItem.BeginEffectiveDate,
                EndEffectiveDate = sourceItem.EndEffectiveDate
            };
            var country = countryManager.GetCountry(sourceItem.CountryName);
            if (country == null)
            {
                var addResult = countryManager.AddCountry(new Vanrise.Entities.Country
                {
                    Name = sourceItem.CountryName
                });
                if (addResult.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                    throw new Exception(String.Format("Cannot add Country '{0}'", sourceItem.CountryName));
                country = addResult.InsertedObject.Entity;
            }
            zone.CountryId = country.CountryId;
            return zone;
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            ZoneManager.ReserveIDRange(nbOfIds, out startingId);
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            throw new NotImplementedException();
        }
    }
}
