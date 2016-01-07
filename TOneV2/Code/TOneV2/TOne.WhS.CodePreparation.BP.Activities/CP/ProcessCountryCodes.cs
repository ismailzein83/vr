using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ProcessCountryCodesInput
    {

        public IEnumerable<CodeToAdd> CodesToAdd { get; set; }

        public IEnumerable<CodeToMove> CodesToMove { get; set; }

        public IEnumerable<CodeToClose> CodesToClose { get; set; }


        public IEnumerable<ExistingCode> ExistingCodes { get; set; }


        public Dictionary<long, ExistingZone> ExistingZonesByZoneId { get; set; }

    }
    public class ProcessCountryCodesOutput
    {
        public IEnumerable<AddedZone> NewZones { get; set; }


        public IEnumerable<ChangedZone> ChangedZones { get; set; }


        public IEnumerable<AddedCode> NewCodes { get; set; }


        public IEnumerable<ChangedCode> ChangedCodes { get; set; }
    }

    public sealed class ProcessCountryCodes : BaseAsyncActivity<ProcessCountryCodesInput, ProcessCountryCodesOutput>
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
        public InOutArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }


        protected override ProcessCountryCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountryCodesInput()
            {
                CodesToAdd = this.CodesToAdd.Get(context),
                CodesToClose = this.CodesToClose.Get(context),
                CodesToMove = this.CodesToMove.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZonesByZoneId = this.ExistingZonesByZoneId.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.NewZones.Get(context) == null)
                this.NewZones.Set(context, new List<AddedZone>());
            if (this.ChangedZones.Get(context) == null)
                this.ChangedZones.Set(context, new List<ChangedZone>());
            if (this.NewCodes.Get(context) == null)
                this.NewCodes.Set(context, new List<AddedCode>());
            if (this.ChangedCodes.Get(context) == null)
                this.ChangedCodes.Set(context, new List<ChangedCode>());
            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessCountryCodesOutput DoWorkWithResult(ProcessCountryCodesInput inputArgument, AsyncActivityHandle handle)
        {
            Dictionary<long, ExistingZone> existingZonesByZoneId = inputArgument.ExistingZonesByZoneId;
            IEnumerable<ExistingZone> existingZones = null;

            if (existingZonesByZoneId != null)
                existingZones = existingZonesByZoneId.Select(item => item.Value);

            ProcessCountryCodesContext processCountryCodesContext = new ProcessCountryCodesContext()
            {
                CodesToAdd = inputArgument.CodesToAdd,
                CodesToMove = inputArgument.CodesToMove,
                CodesToClose = inputArgument.CodesToClose,
                ExistingCodes = inputArgument.ExistingCodes,
                ExistingZones = existingZones,
            };

            PriceListCodeManager plCodeManager = new PriceListCodeManager();
            plCodeManager.ProcessCountryCodes(processCountryCodesContext);

            return new ProcessCountryCodesOutput()
            {
                NewZones = processCountryCodesContext.NewZones,
                ChangedZones = processCountryCodesContext.ChangedZones,
                NewCodes = processCountryCodesContext.NewCodes,
                ChangedCodes = processCountryCodesContext.ChangedCodes
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryCodesOutput result)
        {
            this.NewZones.Set(context, result.NewZones);
            this.ChangedZones.Set(context, result.ChangedZones);
            this.NewCodes.Set(context, result.NewCodes);
            this.ChangedCodes.Set(context, result.ChangedCodes);
        }
    }
}
