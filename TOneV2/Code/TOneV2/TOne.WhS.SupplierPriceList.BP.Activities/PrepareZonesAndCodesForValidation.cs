using System.Collections.Generic;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using System.Linq;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareZonesAndCodesForValidation : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<ImportedCode>> ImportedCodeHashSet { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);
            HashSet<ImportedCode> importedCodesHashSet = ToHashSet(importedCodes);
            ZonesByName newAndExistingZones = NewAndExistingZones.Get(context);
            IEnumerable<IZone> importedZonesEnumerable = PrepareZones(newAndExistingZones);
            ImportedZones.Set(context, importedZonesEnumerable);
            ImportedCodeHashSet.Set(context, importedCodesHashSet);
        }

        private IEnumerable<IZone> PrepareZones(ZonesByName newAndExistingZones)
        {
            List<IZone> zones = new List<IZone>();
            foreach (var elt in newAndExistingZones)
                zones.AddRange(elt.Value);
            return zones;
        }
        private HashSet<T> ToHashSet<T>(IEnumerable<T> list)
        {
            HashSet<T> result = new HashSet<T>();
            result.UnionWith(list);
            return result;
        }
    }
}
