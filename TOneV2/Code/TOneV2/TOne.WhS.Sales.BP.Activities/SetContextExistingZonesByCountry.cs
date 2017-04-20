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
    public class SetContextExistingZonesByCountry : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);

            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
            ratePlanContext.ExistingZonesByCountry = new Dictionary<int, List<ExistingZone>>();

            if (existingZones == null || existingZones.Count() == 0)
                return;

            foreach (ExistingZone existingZone in existingZones)
            {
                List<ExistingZone> countryZones;

                if (!ratePlanContext.ExistingZonesByCountry.TryGetValue(existingZone.CountryId, out countryZones))
                {
                    countryZones = new List<ExistingZone>();
                    ratePlanContext.ExistingZonesByCountry.Add(existingZone.CountryId, countryZones);
                }

                countryZones.Add(existingZone);
            }
        }
    }
}
