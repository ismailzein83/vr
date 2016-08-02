using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSupplierCodesInput
    {
        public int CodePrefixLength { get; set; }

        public string CodePrefix { get; set; }

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
        public InArgument<int> CodePrefixLength { get; set; }

        [RequiredArgument]
        public InArgument<String> CodePrefix { get; set; }

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
            bool getChildCodes = (inputArgument.CodePrefixLength == inputArgument.CodePrefix.Length);
            IEnumerable<SupplierCode> supplierCodes = 
                manager.GetActiveSupplierCodesByPrefix(inputArgument.CodePrefix, inputArgument.EffectiveOn, inputArgument.IsFuture, getChildCodes, true, inputArgument.SupplierInfo);

            return new GetSupplierCodesOutput
            {
                SupplierCodes = supplierCodes
            };
        }

        protected override GetSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSupplierCodesInput
            {
                CodePrefixLength = this.CodePrefixLength.Get(context),
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
