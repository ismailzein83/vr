using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class SourceZoneSynchronizer : Vanrise.Common.Business.EntitySynchronization.SourceItemSynchronizer<SourceZone, Zone, SourceZoneReader>
    {
        Vanrise.Common.Business.CountryManager countryManager = new Vanrise.Common.Business.CountryManager();
        ZoneManager zoneManager = new ZoneManager();
        Vanrise.Entities.SourceCountryReader _sourceCountryReader;

        public SourceZoneSynchronizer(SourceZoneReader sourceItemReader, Vanrise.Entities.SourceCountryReader sourceCountryReader)
            : base(sourceItemReader)
        {
            _sourceCountryReader = sourceCountryReader;
        }

        public override void Synchronize()
        {
            if (_sourceCountryReader != null)
            {
                Vanrise.Common.Business.SourceCountrySynchronizer sourceCountrySync = new Vanrise.Common.Business.SourceCountrySynchronizer(_sourceCountryReader);
                sourceCountrySync.Synchronize();
            }
            base.Synchronize();
        }

        protected override void AddItems(List<Zone> itemsToAdd)
        {
            ZoneManager zoneManager = new ZoneManager();
            foreach (var z in itemsToAdd)
            {
                zoneManager.AddZoneFromeSource(z);
            }
        }

        protected override void UpdateItems(List<Zone> itemsToUpdate)
        {
             ZoneManager zoneManager = new ZoneManager();
             foreach (var z in itemsToUpdate)
            {
                zoneManager.UpdateZoneFromeSource(z);
            }
        }

        protected override Zone BuildItemFromSource(SourceZone sourceItem)
        {
            Zone zone = new Zone
            {
                Name = sourceItem.Name,
                BeginEffectiveDate = sourceItem.BeginEffectiveDate,
                EndEffectiveDate = sourceItem.EndEffectiveDate
            };
            SetCountryId(sourceItem, zone);
            return zone;
        }

        private void SetCountryId(SourceZone sourceZone, Zone zone)
        {
            Vanrise.Entities.Country country;
            if (sourceZone.SourceCountryId != null)
                country = countryManager.GetCountryBySourceId(sourceZone.SourceCountryId);
            else if (sourceZone.CountryName != null)
                country = countryManager.GetCountry(sourceZone.CountryName);
            else
                throw new Exception(String.Format("Country for Source Zone '{0}' not specified!", sourceZone.Name));

            if (country == null)
            {
                country = new Vanrise.Entities.Country
                {
                    Name = sourceZone.CountryName,
                    SourceId = sourceZone.SourceCountryId
                };
                countryManager.AddCountryFromSource(country);
            }
            if (country.CountryId == 0)
                throw new Exception(String.Format("Could not add Country Name '{0}', Source Id '{1}', Zone Name '{2}'", sourceZone.CountryName, sourceZone.SourceCountryId, sourceZone.Name));
            zone.CountryId = country.CountryId;
        }

        protected override void ReserveIdRange(int nbOfIds, out long startingId)
        {
            ZoneManager.ReserveIDRange(nbOfIds, out startingId);
        }

        protected override Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            ZoneManager zonemanager = new ZoneManager();
            return zoneManager.GetExistingItemIds(sourceItemIds);
        }
    }
}
