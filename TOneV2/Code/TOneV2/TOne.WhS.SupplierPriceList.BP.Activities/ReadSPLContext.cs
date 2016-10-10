using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class GetImportSPLContext : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<bool> ProcessHasChanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IImportSPLContext splContext = context.GetSPLParameterContext();
            this.ProcessHasChanges.Set(context, splContext.ProcessHasChanges);
        }
    }
}
