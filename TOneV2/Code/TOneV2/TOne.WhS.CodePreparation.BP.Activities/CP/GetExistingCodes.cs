using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GetExistingCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }
       
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleCode>> ExistingCodeEntities { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            int sellingNumberPlanId = context.GetValue(this.SellingNumberPlanID);
            DateTime minDate = context.GetValue(this.MinimumDate);

            SaleCodeManager codeManager = new SaleCodeManager();
            List<SaleCode> saleCodes = codeManager.GetSaleCodesEffectiveAfter(sellingNumberPlanId, minDate);
            ExistingCodeEntities.Set(context, saleCodes);
        }
    }
}
