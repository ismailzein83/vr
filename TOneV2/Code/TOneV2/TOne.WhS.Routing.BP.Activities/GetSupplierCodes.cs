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
        public Dictionary<string, int> SupplierCodeServiceRuntimeProcessIds { get; set; }

        public int CodePrefixLength { get; set; }

        public IEnumerable<CodePrefix> CodePrefixGroup { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public IEnumerable<RoutingSupplierInfo> SupplierInfo { get; set; }
    }

    public sealed class GetSupplierCodes : BaseAsyncActivity<GetSupplierCodesInput, List<CodePrefixSupplierCodes>>
    {
        [RequiredArgument]
        public InArgument<Dictionary<string, int>> SupplierCodeServiceRuntimeProcessIds { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefix>> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingSupplierInfo>> SupplierInfo { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CodePrefixSupplierCodes>> SupplierCodes { get; set; }

        protected override List<CodePrefixSupplierCodes> DoWorkWithResult(GetSupplierCodesInput inputArgument, AsyncActivityHandle handle)
        {
            List<CodePrefixSupplierCodes> output = new List<CodePrefixSupplierCodes>();

            SupplierCodeManager manager = new SupplierCodeManager();
            foreach (CodePrefix codePrefix in inputArgument.CodePrefixGroup)
            {
                IEnumerable<SupplierCode> supplierCodes =
                    new Vanrise.Runtime.InterRuntimeServiceManager().SendRequest(inputArgument.SupplierCodeServiceRuntimeProcessIds[codePrefix.Code.Substring(0, 1)], new SupplierCodeRequest
                    {
                        ParentProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ParentProcessID.Value,
                        CodePrefix = codePrefix.Code
                    });

                output.Add(new CodePrefixSupplierCodes() { CodePrefix = codePrefix, SupplierCodes = supplierCodes });
            }
            return output;
        }

        protected override GetSupplierCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetSupplierCodesInput
            {
                SupplierCodeServiceRuntimeProcessIds = this.SupplierCodeServiceRuntimeProcessIds.Get(context),
                CodePrefixGroup = this.CodePrefixGroup.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context),
                SupplierInfo = this.SupplierInfo.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, List<CodePrefixSupplierCodes> result)
        {
            this.SupplierCodes.Set(context, result);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Supplier Codes is done", null);
        }
    }
}