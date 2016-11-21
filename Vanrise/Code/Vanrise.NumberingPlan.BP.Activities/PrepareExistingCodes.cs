using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
{
    public class PrepareExistingCodesInput
    {
        public IEnumerable<SaleCode> ExistingCodeEntities { get; set; }
        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }
    }
    public class PrepareExistingCodesOutput
    {
        public IEnumerable<ExistingCode> ExistingCodes { get; set; }
    }
    public sealed class PrepareExistingCodes : BaseAsyncActivity<PrepareExistingCodesInput, PrepareExistingCodesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<SaleCode>> ExistingCodeEntities { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

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

        protected override PrepareExistingCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareExistingCodesInput()
            {
                ExistingCodeEntities = this.ExistingCodeEntities.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingCodes.Get(context) == null)
                this.ExistingCodes.Set(context, new List<ExistingCode>());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareExistingCodesOutput DoWorkWithResult(PrepareExistingCodesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleCode> existingCodeEntities = inputArgument.ExistingCodeEntities;
            Dictionary<long, ExistingZone> existingZonesByZoneId = inputArgument.ExistingZonesByZoneId;

            IEnumerable<ExistingCode> existingCodes = existingCodeEntities.Where(x => existingZonesByZoneId.ContainsKey(x.ZoneId)).MapRecords((codeEntity) => ExistingCodeMapper(codeEntity, existingZonesByZoneId));

            return new PrepareExistingCodesOutput()
            {
                ExistingCodes = existingCodes
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareExistingCodesOutput result)
        {
            this.ExistingCodes.Set(context, result.ExistingCodes);
        }
    }
}
