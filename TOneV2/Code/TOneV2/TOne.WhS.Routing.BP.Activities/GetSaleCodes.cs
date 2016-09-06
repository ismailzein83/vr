using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSaleCodesInput
    {
        public IEnumerable<CodePrefix> CodePrefixGroup { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }
    }

    public sealed class GetSaleCodes : BaseAsyncActivity<GetSaleCodesInput, List<CodePrefixSaleCodes>>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefix>> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodePrefixSaleCodes>> CodePrefixSaleCodes { get; set; }


        protected override List<CodePrefixSaleCodes> DoWorkWithResult(GetSaleCodesInput inputArgument, AsyncActivityHandle handle)
        {
            List<CodePrefixSaleCodes> output = new List<CodePrefixSaleCodes>();
            SaleCodeManager manager = new SaleCodeManager();
            foreach (CodePrefix codePrefix in inputArgument.CodePrefixGroup)
            {
                bool getChildCodes = !codePrefix.IsCodeDivided;
                IEnumerable<SaleCode> saleCodes = manager.GetSaleCodesByPrefix(codePrefix.Code, inputArgument.EffectiveOn, inputArgument.IsFuture, getChildCodes, true);

                output.Add(new CodePrefixSaleCodes() { CodePrefix = codePrefix, SaleCodes = saleCodes });
            }
            return output;
        }

        protected override GetSaleCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSaleCodesInput
            {
                CodePrefixGroup = this.CodePrefixGroup.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, List<CodePrefixSaleCodes> result)
        {
            this.CodePrefixSaleCodes.Set(context, result);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Sale Codes is done", null);
        }
    }
}