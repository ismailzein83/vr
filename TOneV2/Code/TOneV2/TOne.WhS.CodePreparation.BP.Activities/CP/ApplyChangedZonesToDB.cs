using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class ApplyChangedZonesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedZone> zonesList = this.ChangedZones.Get(context);
            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            IChangedSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneDataManager>();
            dataManager.Insert(processInstanceID, zonesList);
        }
    }
}
