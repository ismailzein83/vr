using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class ProcessCountryCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToAdd>> CodesToAdd { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToMove>> CodesToMove { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<CodeToClose>> CodesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }


        [RequiredArgument]
        public OutArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);
            IEnumerable<ExistingZone> existingZones = null;

            if (existingZonesByZoneId != null)
                existingZones = existingZonesByZoneId.Select(item => item.Value);

            ProcessCountryCodesContext processCountryCodesContext = new ProcessCountryCodesContext()
            {
                CodesToAdd = this.CodesToAdd.Get(context),
                CodesToMove = this.CodesToMove.Get(context),
                CodesToClose = this.CodesToClose.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZones = existingZones,
            };

            PriceListCodeManager plCodeManager = new PriceListCodeManager();
            plCodeManager.ProcessCountryCodes(processCountryCodesContext);
            NewZones.Set(context, processCountryCodesContext.NewZones);
            ChangedZones.Set(context, processCountryCodesContext.ChangedZones);
            NewCodes.Set(context, processCountryCodesContext.NewCodes);
            ChangedCodes.Set(context, processCountryCodesContext.ChangedCodes);
        }
    }
}
