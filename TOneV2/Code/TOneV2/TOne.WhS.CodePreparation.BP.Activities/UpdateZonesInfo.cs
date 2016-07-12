using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class UpdateZonesInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ZoneToProcess> zonesToProcess = ZonesToProcess.Get(context);
            
            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);
            ZonesByName newAndExistingZones = NewAndExistingZones.Get(context);


            foreach (ZoneToProcess zoneToProcess in zonesToProcess)
            {
                List<IZone> addedZones;
                newAndExistingZones.TryGetValue(zoneToProcess.ZoneName, out addedZones);
               
                if (addedZones != null)
                    zoneToProcess.AddedZones.AddRange(addedZones.Where(itm => itm is AddedZone).Select(item => item as AddedZone));
                
                if (existingZones != null)
                    zoneToProcess.ExistingZones.AddRange(existingZones.FindAllRecords(item => item.Name.Equals(zoneToProcess.ZoneName, StringComparison.InvariantCultureIgnoreCase)));
            }

            ZonesToProcess.Set(context, zonesToProcess);

        }
    }
}
