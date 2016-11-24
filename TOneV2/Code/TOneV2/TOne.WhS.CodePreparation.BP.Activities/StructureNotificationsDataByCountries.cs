using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class StructureNotificationsDataByCountries : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> ZonesChanges { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int ,List<SalePLZoneChange>>> ZonesChangesByCountry { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SalePLZoneChange> zonesChanges = this.ZonesChanges.Get(context);
            Dictionary<int, List<SalePLZoneChange>> zonesChangesByCountry = new Dictionary<int, List<SalePLZoneChange>>();

            List<SalePLZoneChange> zonesChangesList;

            foreach (SalePLZoneChange zoneChange in zonesChanges)
            {
                if (!zonesChangesByCountry.TryGetValue(zoneChange.CountryId, out zonesChangesList))
                {
                    zonesChangesList = new List<SalePLZoneChange>();
                    zonesChangesList.Add(zoneChange);
                    zonesChangesByCountry.Add(zoneChange.CountryId, zonesChangesList);
                }
                else
                    zonesChangesList.Add(zoneChange);
            }

            this.ZonesChangesByCountry.Set(context, zonesChangesByCountry);
        }
    }
}
