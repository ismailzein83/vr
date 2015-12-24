using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class ApplyNewZonesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<AddedZone>> NewZones { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<AddedZone> zonesList = this.NewZones.Get(context);
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            INewSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneDataManager>();
            dataManager.Insert(sellingNumberPlanId, processInstanceID, zonesList);
           
        }
    }
}
