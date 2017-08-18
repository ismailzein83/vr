using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class StructZoneDataByCountryIds : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        #endregion

        #region Output Arguments
        [RequiredArgument]
        public OutArgument<ZoneDataByCountryIds> ZoneDataByCountryIds { get; set; }
        #endregion 
        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IEnumerable<DataByZone> dataByZones = DataByZone.Get(context);
            var zoneDataByCountryIds = new ZoneDataByCountryIds();




            if (dataByZones == null || dataByZones.Count() == 0)
                return;

            foreach (DataByZone dataByZone in dataByZones)
            {
                List<DataByZone> countryZones;

                if (!zoneDataByCountryIds.TryGetValue(dataByZone.CountryId, out countryZones))
                {
                    countryZones = new List<DataByZone>();
                    zoneDataByCountryIds.Add(dataByZone.CountryId, countryZones);
                }

                countryZones.Add(dataByZone);

            }
            ZoneDataByCountryIds.Set(context, zoneDataByCountryIds);

        }
    }
}
