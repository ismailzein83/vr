using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ReadDataFromExcel:CodeActivity
    {
        public InArgument<int> FileId { get; set; }
        public InArgument<DateTime> EffectiveDate { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            Console.WriteLine("Test:" + EffectiveDate.Get(context));
        }
    }
}
