using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class PrepareExistingCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleCode>> ExistingCodeEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleCode> existingCodeEntities = this.ExistingCodeEntities.Get(context);
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);

            IEnumerable<ExistingCode> existingCodes = existingCodeEntities.MapRecords((codeEntity) => ExistingCodeMapper(codeEntity, existingZonesByZoneId));
            ExistingCodes.Set(context, existingCodes);
        }
        ExistingCode ExistingCodeMapper(SaleCode codeEntity, Dictionary<long, ExistingZone> existingZonesByZoneId)
        {
            ExistingZone existingZone;

            if (!existingZonesByZoneId.TryGetValue(codeEntity.ZoneId, out existingZone))
                throw new Exception(String.Format("Code Entity with Id {0} is not linked to Zone Id {1}", codeEntity.SaleCodeId, codeEntity.ZoneId));

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
