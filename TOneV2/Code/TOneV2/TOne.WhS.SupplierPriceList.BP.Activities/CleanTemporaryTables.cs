using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Data;
using Vanrise.BusinessProcess;
namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class CleanTemporaryTables : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            SupplierPriceListManager SupplierPriceListManager = new SupplierPriceListManager();
            SupplierPriceListManager.CleanTemporaryTables(processInstanceId); ;
        }
    }
}
