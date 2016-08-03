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
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);
            IEnumerable<DataByZone> dataByZoneList = this.DataByZone.Get(context);

            Dictionary<string, List<ExistingZone>> existingZonesByZoneName = this.StructureExistingZonesByZoneName(existingZones);

            foreach (DataByZone item in dataByZoneList)
            {
                List<ExistingZone> existingZoneList = existingZonesByZoneName[item.ZoneName];
                item.BED = existingZoneList.OrderBy(x => x.BED).Min(x => x.BED);
                item.EED = existingZoneList.Select(x => x.EED).VRMaximumDate();
            }
        }

        #region Private Methods

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
        
        #endregion
    }
}
