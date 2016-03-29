using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GetExistingCodesInput
    {
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }
        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingCodesOutput
    {
        public IEnumerable<SaleCode> ExistingCodeEntities { get; set; }
    }
    public class GetExistingCodes : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingCodesInput, GetExistingCodesOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }
        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }
        [RequiredArgument]
        public InOutArgument<IEnumerable<SaleCode>> ExistingCodeEntities { get; set; }

        protected override GetExistingCodesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingCodesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SellingNumberPlanId = this.SellingNumberPlanID.Get(context),
                CountryId = this.CountryId.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingCodeEntities.Get(context) == null)
                this.ExistingCodeEntities.Set(context, new List<SaleCode>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingCodesOutput DoWorkWithResult(GetExistingCodesInput inputArgument, AsyncActivityHandle handle)
        {

            SaleCodeManager codeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = codeManager.GetSaleCodesEffectiveAfter(inputArgument.SellingNumberPlanId, inputArgument.CountryId, inputArgument.MinimumDate);
            return new GetExistingCodesOutput()
            {
                ExistingCodeEntities = saleCodes
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingCodesOutput result)
        {
            this.ExistingCodeEntities.Set(context, result.ExistingCodeEntities);
        }
    }
}
