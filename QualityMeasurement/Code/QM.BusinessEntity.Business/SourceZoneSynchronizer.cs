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

        protected override Zone BuildItemFromSource(SourceZone sourceZone)
        {
            Zone zone = new Zone
            {
                Name = sourceZone.Name,
                BeginEffectiveDate = sourceZone.BeginEffectiveDate,
                EndEffectiveDate = sourceZone.EndEffectiveDate
            };
            var country = countryManager.GetCountry(sourceZone.CountryName);
            if (country == null)
            {
                var addResult = countryManager.AddCountry(new Vanrise.Entities.Country
                {
                    Name = sourceZone.CountryName
                });
                if (addResult.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                    throw new Exception(String.Format("Cannot add Country '{0}'", sourceZone.CountryName));
                country = addResult.InsertedObject.Entity;
            }
            zone.CountryId = country.CountryId;
            return zone;
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            ZoneManager.ReserveIDRange(nbOfIds, out startingId);
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceZoneIds)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<long> itemIds)
        {
            throw new NotImplementedException();
        }
    }
}
