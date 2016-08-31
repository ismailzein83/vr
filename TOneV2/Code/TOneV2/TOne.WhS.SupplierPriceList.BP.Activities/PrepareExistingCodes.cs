using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class PrepareExistingCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> ExistingCodeEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierCode> existingCodeEntities = this.ExistingCodeEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingCode> existingCodes = existingCodeEntities.OrderBy(item => item.BED).Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords(
                (codeEntity) => ExistingCodeMapper(codeEntity, existingZonesByZoneId));

            ExistingCodes.Set(context, existingCodes);
        }

        ExistingCode ExistingCodeMapper(SupplierCode codeEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(codeEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Code Entity with Id {0} is not linked to Zone Id {1}", codeEntity.SupplierCodeId, codeEntity.ZoneId));

            ExistingCode existingCode = new ExistingCode()
            {
                CodeEntity = codeEntity,
                ParentZone = existingZone
            };

            existingCode.ParentZone.ExistingCodes.Add(existingCode);
            return existingCode;
        }
    }
}
