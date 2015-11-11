using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetSupplierCodesInput
    {
        public string CodePrefix { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }
    }

    public class GetSupplierCodesOutput
    {
        public IEnumerable<SupplierCode> SupplierCodes { get; set; }
    }

    public sealed class GetSupplierCodes : BaseAsyncActivity<GetSupplierCodesInput, GetSupplierCodesOutput>
    {

        [RequiredArgument]
        public InArgument<String> CodePrefix { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }

        protected override GetSupplierCodesOutput DoWorkWithResult(GetSupplierCodesInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierCodeManager manager = new SupplierCodeManager();
            IEnumerable<SupplierCode> supplierCodes = manager.GetSupplierCodesByPrefix(inputArgument.CodePrefix, inputArgument.EffectiveOn, inputArgument.IsFuture);

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
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetSupplierCodesOutput result)
        {
            this.SupplierCodes.Set(context, result.SupplierCodes);
        }
    }
}
