using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.BP.Activities
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

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<AddedCode> NewCodes { get; set; }

        public IEnumerable<ChangedCode> ChangedCodes { get; set; }

        public Dictionary<string, List<ExistingZone>> ClosedExistingZones { get; set; }

        public IEnumerable<NotImportedCode> NotImportedCodes { get; set; }
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
        public OutArgument<ZonesByName> NewAndExistingZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<AddedZone>> NewZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<AddedCode>> NewCodes { get; set; }

        [RequiredArgument]
        public InOutArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }
       
        [RequiredArgument]
        public OutArgument<Dictionary<string, List<ExistingZone>>> ClosedExistingZones { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<NotImportedCode>> NotImportedCodes { get; set; }


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
                NewAndExistingZones= processCountryCodesContext.NewAndExistingZones,
                NewCodes = processCountryCodesContext.NewCodes,
                ChangedCodes = processCountryCodesContext.ChangedCodes,
                ClosedExistingZones = processCountryCodesContext.ClosedExistingZones,
                NotImportedCodes = processCountryCodesContext.NotImportedCodes
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountryCodesOutput result)
        {
            this.NewAndExistingZones.Set(context, result.NewAndExistingZones);
            this.NewZones.Set(context, result.NewZones);
            this.ChangedZones.Set(context, result.ChangedZones);
            this.NewCodes.Set(context, result.NewCodes);
            this.ChangedCodes.Set(context, result.ChangedCodes);
            this.ClosedExistingZones.Set(context, result.ClosedExistingZones);
            this.NotImportedCodes.Set(context, result.NotImportedCodes);
        }
    }
}
