using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

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

        Dictionary<string, List<string>> _codesBySourceZoneId;
        private List<string> GetZoneCodes(Zone zone)
        {
            if(_codesBySourceZoneId == null)
            {
                var allZoneCodes = base.SourceItemReader.GetAllCodes();                
                _codesBySourceZoneId = new Dictionary<string, List<string>>();
                if (allZoneCodes != null)
                {
                    foreach (var zoneCode in allZoneCodes)
                    {
                        List<string> codes = _codesBySourceZoneId.GetOrCreateItem(zoneCode.SourceZoneId);
                        codes.Add(zoneCode.Code);
                    }
                }
            }
            List<string> zoneCodes;
            string zoneId = this.SourceItemReader.UseSourceItemId ? zone.ZoneId.ToString() : zone.SourceId;
            if (_codesBySourceZoneId.TryGetValue(zoneId, out zoneCodes))
                return zoneCodes;
            else
                return null;
        }

        protected override void AddItems(List<Zone> itemsToAdd)
        {
            foreach (var z in itemsToAdd)
            {
                zoneManager.AddZoneFromSource(z, GetZoneCodes(z));
            }
        }

        protected override void UpdateItems(List<Zone> itemsToUpdate)
        {
             foreach (var z in itemsToUpdate)
            {
                zoneManager.UpdateZoneFromSource(z, GetZoneCodes(z));
            }
        }

        protected override void SetItemsDeleted(List<long> itemIds)
        {
            foreach (var z in itemIds)
            {
                Zone zone = zoneManager.GetZone(z);
                zone.EndEffectiveDate = DateTime.Now;
                zoneManager.UpdateZoneFromSource(zone);
            }
        }

        protected override Zone BuildItemFromSource(SourceZone sourceItem)
        {
            Zone zone = new Zone
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId,
                IsFromTestingConnectorZone = sourceItem.IsFromTestingConnectorZone,
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

        protected override Dictionary<string, long> GetExistingItemIds()
        {
            return zoneManager.GetExistingItemIds();
        }
    }
}
