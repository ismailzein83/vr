using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
   public class ApplySupplierZonesToDB:CodeActivity
    {
       public InArgument<List<Zone>> Zones { get; set; }
       public InArgument<int> SupplierId { get; set; }

       protected override void Execute(CodeActivityContext context)
       {
           DateTime startApplying = DateTime.Now;

           TimeSpan spent = DateTime.Now.Subtract(startApplying);
           context.WriteTrackingMessage(LogEntryType.Information, "Apply Supplier Zones  done and takes:{0}", spent);
       }
    }
}
