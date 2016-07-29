using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class UpdateZonesData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingZone> existingZonesList = this.ExistingZones.Get(context);
            IEnumerable<DataByZone> dataByZoneList = this.DataByZone.Get(context);

            Dictionary<string, List<ExistingZone>> existingZonesByZoneName = this.StructureExistingZonesByZoneName(existingZonesList);

            foreach (DataByZone item in dataByZoneList)
            {
                List<ExistingZone> existingZones = existingZonesByZoneName[item.ZoneName];
                item.BED = existingZones.OrderBy(x => x.BED).Min(x => x.BED);
                item.EED = existingZones.Select(x => x.EED).VRMaximumDate();
            }
        }

        private Dictionary<string, List<ExistingZone>> StructureExistingZonesByZoneName(IEnumerable<ExistingZone> allExistingZones)
        {
            Dictionary<string, List<ExistingZone>> existingZonesByZoneName = new Dictionary<string, List<ExistingZone>>();
            List<ExistingZone> innerList;

            foreach (ExistingZone existingZone in allExistingZones)
            {
                if (!existingZonesByZoneName.TryGetValue(existingZone.Name, out innerList))
                {
                    innerList = new List<ExistingZone>();
                    existingZonesByZoneName.Add(existingZone.Name, innerList);
                }

                innerList.Add(existingZone);
            }

            return existingZonesByZoneName;
        }
    }
}
