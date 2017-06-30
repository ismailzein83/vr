using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructureDataByCountry : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CountryData>> DataByCountry { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return;

            var dataByCountry = new List<CountryData>();

            IEnumerable<DataByZone> dataByZone = DataByZone.Get(context);
            Dictionary<int, Dictionary<long, DataByZone>> zoneDataByCountryId = StructureZoneDataByCountryId(dataByZone);

            IEnumerable<CustomerCountryToAdd> newCountries = CustomerCountriesToAdd.Get(context);
            IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetSoldCountries(ratePlanContext.OwnerId, ratePlanContext.EffectiveDate);

            if (soldCountries != null)
            {
                dataByCountry.AddRange(soldCountries.MapRecords(x => new CountryData()
                {
                    CountryId = x.CountryId,
                    CountryBED = x.BED,
                    ZoneDataByZoneId = zoneDataByCountryId.GetRecord(x.CountryId)
                }));
            }

            if (newCountries != null)
            {
                dataByCountry.AddRange(newCountries.MapRecords(x => new CountryData()
                {
                    CountryId = x.CountryId,
                    CountryBED = x.BED,
                    ZoneDataByZoneId = zoneDataByCountryId.GetRecord(x.CountryId)
                }));
            }

            DataByCountry.Set(context, dataByCountry);
        }

        #region Private Methods

        private Dictionary<int, Dictionary<long, DataByZone>> StructureZoneDataByCountryId(IEnumerable<DataByZone> dataByZone)
        {
            var zoneDataByCountryId = new Dictionary<int, Dictionary<long, DataByZone>>();

            if (dataByZone != null)
            {
                foreach (DataByZone zoneData in dataByZone)
                {
                    Dictionary<long, DataByZone> countryData = zoneDataByCountryId.GetOrCreateItem(zoneData.CountryId, () =>
                    {
                        return new Dictionary<long, DataByZone>();
                    });

                    if (!countryData.ContainsKey(zoneData.ZoneId))
                        countryData.Add(zoneData.ZoneId, zoneData);
                }
            }

            return zoneDataByCountryId;
        }

        #endregion
    }
}
