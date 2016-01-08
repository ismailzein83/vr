using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class GetExistingCodesInput
    {
        public int SupplierId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingCodesOutput
    {
        public IEnumerable<SupplierCode> ExistingCodeEntities { get; set; }
    }

    public sealed class GetExistingCodes : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingCodesInput, GetExistingCodesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierCode>> ExistingCodeEntities { get; set; }

        protected override GetExistingCodesOutput DoWorkWithResult(GetExistingCodesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SupplierCodeManager codeManager = new SupplierCodeManager();
            List<SupplierCode> suppCodes = codeManager.GetSupplierCodesEffectiveAfter(inputArgument.SupplierId, inputArgument.MinimumDate);

            return new GetExistingCodesOutput()
            {
                ExistingCodeEntities = suppCodes
            };
        }

        protected override GetExistingCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingCodesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SupplierId = this.SupplierId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingCodesOutput result)
        {
            this.ExistingCodeEntities.Set(context, result.ExistingCodeEntities);
        }
    }
}
