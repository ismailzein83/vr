using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSupplierCodesInput
    {
        public int CodePrefixLength { get; set; }

        public CodePrefix CodePrefix { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public IEnumerable<RoutingSupplierInfo> SupplierInfo { get; set; }
    }

    public class GetSupplierCodesOutput
    {
        public IEnumerable<SupplierCode> SupplierCodes { get; set; }
    }

    public sealed class GetSupplierCodes : BaseAsyncActivity<GetSupplierCodesInput, GetSupplierCodesOutput>
    {
        [RequiredArgument]
        public InArgument<CodePrefix> CodePrefix { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingSupplierInfo>> SupplierInfo { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }

        protected override GetSupplierCodesOutput DoWorkWithResult(GetSupplierCodesInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierCodeManager manager = new SupplierCodeManager();
            bool getChildCodes = !inputArgument.CodePrefix.IsCodeDivided;
            IEnumerable<SupplierCode> supplierCodes = 
                manager.GetActiveSupplierCodesByPrefix(inputArgument.CodePrefix.Code, inputArgument.EffectiveOn, inputArgument.IsFuture, getChildCodes, true, inputArgument.SupplierInfo);

            return new GetSupplierCodesOutput
            {
                SupplierCodes = supplierCodes
            };
        }

        protected override GetSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSupplierCodesInput
            {
                CodePrefix = this.CodePrefix.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context),
                SupplierInfo = this.SupplierInfo.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSupplierCodesOutput result)
        {
            this.SupplierCodes.Set(context, result.SupplierCodes);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Supplier Codes is done", null);
        }
    }
}
