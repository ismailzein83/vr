using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSaleCodesInput
    {
        public int CodePrefixLength { get; set; }

        public string CodePrefix { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }
    }

    public class GetSaleCodesOutput
    {
        public IEnumerable<SaleCode> SaleCodes { get; set; }
    }


    public sealed class GetSaleCodes : BaseAsyncActivity<GetSaleCodesInput, GetSaleCodesOutput>
    {
        [RequiredArgument]
        public InArgument<int> CodePrefixLength { get; set; }

        [RequiredArgument]
        public InArgument<String> CodePrefix { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleCode>> SaleCodes { get; set; }


        protected override GetSaleCodesOutput DoWorkWithResult(GetSaleCodesInput inputArgument, AsyncActivityHandle handle)
        {
            SaleCodeManager manager = new SaleCodeManager();

            bool getChildCodes = (inputArgument.CodePrefixLength == inputArgument.CodePrefix.Length);
            IEnumerable<SaleCode> saleCodes = manager.GetSaleCodesByPrefix(inputArgument.CodePrefix, inputArgument.EffectiveOn, inputArgument.IsFuture, getChildCodes, true);

            return new GetSaleCodesOutput
            {
                SaleCodes = saleCodes
            };
        }

        protected override GetSaleCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSaleCodesInput
            {
                CodePrefixLength = this.CodePrefixLength.Get(context),
                CodePrefix = this.CodePrefix.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSaleCodesOutput result)
        {
            this.SaleCodes.Set(context, result.SaleCodes);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Sale Codes is done", null);
        }
    }
}
